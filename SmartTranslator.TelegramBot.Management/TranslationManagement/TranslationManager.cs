using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartTranslator.Contracts.Dto;
using SmartTranslator.Contracts.Requests;
using SmartTranslator.DataAccess;
using SmartTranslator.DataAccess.Entities;
using SmartTranslator.Enums;
using SmartTranslator.TelegramBot.Management.Exceptions;
using SmartTranslator.TelegramBot.Management.GptTelegramBots.Events;
using SmartTranslator.TranslationCore.Abstractions;
using SmartTranslator.TranslationCore.Abstractions.Models;
using SmartTranslator.TranslationCore.Enums;

namespace SmartTranslator.TelegramBot.Management.TranslationManagement;

public class TranslationManager : ITranslationManager
{
    private readonly TelegramTranslationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IGptTranslator _translator;
    private readonly ILanguageManager _languageManager;
    private readonly ITextMistakeManager _textMistakeManager;
    private readonly IPublisher _domainEventDistributor;

    public TranslationManager(TelegramTranslationDbContext dbContext,
                              IMapper mapper,
                              IGptTranslator translator,
                              ILanguageManager languageManager,
                              ITextMistakeManager textMistakeManager,
                              IPublisher domainEventDistributor)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _translator = translator;
        _languageManager = languageManager;
        _textMistakeManager = textMistakeManager;
        _domainEventDistributor = domainEventDistributor;
    }

    public async Task<TelegramTranslationDto?> GetLatest(string username, long chatId)
    {
        var entity = await _dbContext.TelegramTranslations
                               .Where(t => t.UserName == username && t.ChatId == chatId)
                               .OrderByDescending(t => t.CreatedAt)
                               .FirstOrDefaultAsync();

        return entity == null ? null : _mapper.Map<TelegramTranslationDto>(entity);
    }


    public async Task FinishTranslation(string translationId)
    {
        var entity = _dbContext.TelegramTranslations.Find(translationId);

        if (entity == null)
            return;

        entity.State = TelegramTranslationState.Finished;
        entity.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }


    public async Task<TelegramTranslationDto> SetLanguages(string translationId, Language baseLanguage)
    {
        var entity = _dbContext.TelegramTranslations.Find(translationId);

        if (entity == null)
            throw new EntityNotFoundException();

        var targetLanguage = GetTargetLanguage(baseLanguage);
        entity.LanguageFrom = baseLanguage;
        entity.LanguageTo = targetLanguage;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.State = DetermineState(entity);

        entity = await ExecuteEntityProcessingPipeline(entity);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<TelegramTranslationDto>(entity);
    }


    public async Task<TelegramTranslationDto> DetermineContext(string translationId)
    {
        var entity = _dbContext.TelegramTranslations.Find(translationId);

        if (entity == null)
            throw new EntityNotFoundException();

        var text = entity.BaseText;
        var language = entity.LanguageTo.Value;
        var contextString = GetContexts(entity);
        var evaluation = await _translator.EvaluateContext(text, language, contextString);

        if (evaluation.Percent < 0.7f)
        {
            entity.Contexts.Add(new Context
            {
                Question = evaluation.Request.ClarifyingQuestion
            });
        }

        entity.UpdatedAt = DateTime.UtcNow;
        entity.State = DetermineState(entity);

        entity = await ExecuteEntityProcessingPipeline(entity);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<TelegramTranslationDto>(entity);
    }


    public Task<Context> GetLatestContext(string translationId)
    {
        var entity = _dbContext.TelegramTranslations.Find(translationId);

        if (entity == null)
            throw new EntityNotFoundException();

        return Task.FromResult(entity.Contexts.Last());
    }


    public async Task<TelegramTranslationDto> AddAnswerToContextQuestion(string translationId, string answer)
    {
        var entity = _dbContext.TelegramTranslations.Find(translationId);

        if (entity == null)
            throw new EntityNotFoundException();

        entity.Contexts.Last().Response = answer;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.State = DetermineState(entity);
        entity = await ExecuteEntityProcessingPipeline(entity);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<TelegramTranslationDto>(entity);
    }


    public async Task<TelegramTranslationDto> SetStyle(string translationId, TranslationStyle style)
    {
        var entity = _dbContext.TelegramTranslations.Find(translationId);

        if (entity == null)
            throw new EntityNotFoundException();

        entity.TranslationStyle = style;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.State = DetermineState(entity);
        entity = await ExecuteEntityProcessingPipeline(entity);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<TelegramTranslationDto>(entity);
    }


    public Task<string> GetLatestTranslatedText(string translationId)
    {
        var entity = _dbContext.TelegramTranslations.Find(translationId);

        if (entity == null)
            throw new EntityNotFoundException();

        return Task.FromResult(entity.Translation);
    }


    public async Task<TelegramTranslationDto> AddExtraContext(string translationId, string context)
    {
        var entity = _dbContext.TelegramTranslations.Find(translationId);

        if (entity == null)
            throw new EntityNotFoundException();

        entity.Contexts.Add(new Context
        {
            Question = "-",
            Response = context
        });
        entity.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<TelegramTranslationDto>(entity);
    }


    public TelegramTranslationState DetermineState(TelegramTranslationEntity entity)
    {
        // 1. If the translation is done
        if (entity.Translation != null)
            return TelegramTranslationState.Finished;

        // 2. If translation style is filled out
        if (entity.TranslationStyle != null)
            return TelegramTranslationState.WaitingForTranslation;

        // 3. If languages are not determined
        if (entity.LanguageFrom == null || entity.LanguageTo == null)
            return TelegramTranslationState.WaitingForLanguage;

        // 4. If all contexts are filled
        if (entity.Contexts.All(context => context.Response != null))
            return TelegramTranslationState.WaitingForStyle;

        // 5. If there are contexts awaiting a response
        if (entity.Contexts.Any(context => context.Response == null))
            return TelegramTranslationState.WaitingForContext;

        // If none of the conditions were met
        return entity.State;
    }

    public async Task<TelegramTranslationDto> Create(CreateTelegramTranslationEntityRequest request)
    {
        var entity = new TelegramTranslationEntity
        {
            Id = Guid.NewGuid().ToString(),
            ChatId = request.ChatId,
            UserName = request.UserName,
            State = TelegramTranslationState.Created,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BaseText = request.BaseText,
        };

        var filledEntity = await ExecuteEntityProcessingPipeline(entity);

        _dbContext.TelegramTranslations.Add(filledEntity);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<TelegramTranslationDto>(filledEntity);
    }


    public async Task<TelegramTranslationEntity> ExecuteEntityProcessingPipeline(TelegramTranslationEntity entity)
    {
        if (entity.LanguageFrom == null || entity.LanguageTo == null)
        {
            entity = await AddLanguage(entity);
            if (entity.State == TelegramTranslationState.WaitingForLanguage)
                return entity;
        }

        if (entity.Contexts.Count < 3)
        {
            entity = await AddContext(entity);
            if (entity.State == TelegramTranslationState.WaitingForContext)
                return entity;
        }

        if (entity.TranslationStyle == null)
        {
            entity = await AddStyle(entity);
            if (entity.State == TelegramTranslationState.WaitingForStyle)
                return entity;
        }

        var contextString = GetContexts(entity);
        var translation = await _translator.Translate(entity.BaseText, contextString, entity.LanguageFrom!.Value, entity.LanguageTo!.Value, entity.TranslationStyle!.Value);

        var correctedTranslation = await _textMistakeManager.Correct(translation);
        entity.Translation = correctedTranslation;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.State = TelegramTranslationState.Finished;

        await _domainEventDistributor.Publish(new TextWasTranslatedEvent(entity));

        return entity;
    }


    public async Task Block(string username)
    {
        await _domainEventDistributor.Publish(new UserHasBlockedBotEvent(username));
    }


    public async Task Activate(string username)
    {
        await _domainEventDistributor.Publish(new UserHasActivatedBotEvent(username));
    }


    private async Task<TelegramTranslationEntity> AddLanguage(TelegramTranslationEntity entity)
    {
        var text = entity.BaseText;
        var baseTextLanguage = await _languageManager.DetermineLanguage(text);
        if (baseTextLanguage == null)
        {
            entity.State = DetermineState(entity);
            return entity;
        }

        var targetLanguage = GetTargetLanguage(baseTextLanguage.Value);
        entity.LanguageFrom = baseTextLanguage;
        entity.LanguageTo = targetLanguage;
        entity.UpdatedAt = DateTime.UtcNow;

        return entity;
    }


    private async Task<TelegramTranslationEntity> AddContext(TelegramTranslationEntity entity)
    {
        var contexts = GetContexts(entity);
        var contextEvaluation = await _translator.EvaluateContext(entity.BaseText, entity.LanguageTo!.Value, contexts);
        if (contextEvaluation.Percent < 0.7f)
        {
            entity.Contexts.Add(new Context
            {
                Question = contextEvaluation.Request.ClarifyingQuestion
            });
            entity.UpdatedAt = DateTime.UtcNow;
            entity.State = DetermineState(entity);
            return entity;
        }

        return entity;
    }


    private async Task<TelegramTranslationEntity> AddStyle(TelegramTranslationEntity entity)
    {
        var contextString = GetContexts(entity);

        var styleProbabilities = await _translator.DefineStyle(entity.BaseText, contextString, entity.LanguageFrom!.Value, entity.LanguageTo!.Value);
        var style = GetMostProbableStyle(styleProbabilities);

        if (style == null)
        {
            entity.State = DetermineState(entity);
            return entity;
        }

        entity.TranslationStyle = style;
        entity.UpdatedAt = DateTime.UtcNow;

        return entity;
    }


    private static TranslationStyle? GetMostProbableStyle(StyleDefinitionResult result)
    {
        var maxProbability = result.ProbabilityOfSuccess.Max(x => x.Probability);
        var mostProbableStyles = result.ProbabilityOfSuccess.Where(x => x.Probability == maxProbability).ToList();

        if (mostProbableStyles.Count != 1)
            return null;

        if (mostProbableStyles.Single().Probability < 0.8)
            return null;

        return mostProbableStyles.Single().Style;
    }


    private Language GetTargetLanguage(Language baseLanguage)
    {
        var languagePair = _languageManager.GetLanguagePair();
        var targetLanguage = baseLanguage == languagePair.Item1 ? languagePair.Item2 : languagePair.Item1;

        return targetLanguage;
    }


    private string GetContexts(TelegramTranslationEntity entity)
    {
        var contextString = string.Join(". ", entity.Contexts.Select(c => $"{c.Question} - {c.Response}"));

        return contextString;
    }
}
