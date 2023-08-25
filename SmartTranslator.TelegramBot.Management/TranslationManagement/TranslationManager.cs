﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartTranslator.Contracts.Dto;
using SmartTranslator.Contracts.Requests;
using SmartTranslator.DataAccess;
using SmartTranslator.DataAccess.Entities;
using SmartTranslator.Enums;
using SmartTranslator.TranslationCore;
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

    public TranslationManager(TelegramTranslationDbContext dbContext, 
                              IMapper mapper, 
                              IGptTranslator translator, 
                              ILanguageManager languageManager,
                              ITextMistakeManager textMistakeManager)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _translator = translator;
        _languageManager = languageManager;
        _textMistakeManager = textMistakeManager;
    }

    public async Task<TelegramTranslationDto?> GetLatest(string username, long chatId)
    {
        var entity = await _dbContext.TelegramTranslations
                               .Where(t => t.UserName == username && t.ChatId == chatId)
                               .OrderByDescending(t => t.CreatedAt)
                               .FirstOrDefaultAsync();

        return entity == null ? null : _mapper.Map<TelegramTranslationDto>(entity);
    }


    public TelegramTranslationState DetermineState(TelegramTranslationEntity entity)
    {
        // 1. If the translation is done
        if (entity.Translation != null)
            return TelegramTranslationState.Finished;

        // 2. If translation style is filled out
        if (entity.TranslationStyle != null)
            return TelegramTranslationState.WaitingForTranslation;

        // 3. If all contexts are filled
        if (entity.Contexts.All(context => context.Response != null))
            return TelegramTranslationState.WaitingForStyle;

        // 4. If there are contexts awaiting a response
        if (entity.Contexts.Any(context => context.Response == null))
            return TelegramTranslationState.WaitingForContext;

        // 5. If languages are not determined
        if (entity.LanguageFrom == null || entity.LanguageTo == null)
            return TelegramTranslationState.WaitingForLanguage;

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

        _dbContext.TelegramTranslations.Add(entity);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<TelegramTranslationDto>(entity);
    }


    private async Task<TelegramTranslationEntity> ExecuteEntityProcessingPipeline(TelegramTranslationEntity entity)
    {
        var text = entity.BaseText;
        var languagePair = _languageManager.GetLanguagePair();
        // Определить язык
        var baseTextLanguage = await _languageManager.DetermineLanguage(text);
        if (baseTextLanguage == null)
        {
            entity.State = DetermineState(entity);
            return entity;
        }

        var targetLanguage = baseTextLanguage == languagePair.Item1 ? languagePair.Item2 : languagePair.Item1;
        entity.LanguageFrom = baseTextLanguage;
        entity.LanguageTo = targetLanguage;

        // Определить достаточно ли контекста
        var contextEvaluation = await _translator.EvaluateContext(text, targetLanguage);
        if (contextEvaluation.Percent == 0)
        {
            entity.Contexts.Add(new Context
            {
                Question = contextEvaluation.Request.ClarifyingQuestion
            });
            entity.State = DetermineState(entity);
            return entity;
        }

        // Определить стиль перевода
        var contextString = "";
        foreach (var context in entity.Contexts)
        {
            contextString += $@"-{context.Question}
-{context.Response}";
        }

        var styleProbabilities = await _translator.DefineStyle(text, contextString, baseTextLanguage, targetLanguage);
        var style = GetMostProbableStyle(styleProbabilities);

        if (style == null)
        {
            entity.State = DetermineState(entity);
            return entity;
        }

        entity.TranslationStyle = style;

        // отправка на перевод
        var translation = await _translator.Translate(text, contextString, baseTextLanguage.Value, targetLanguage, style.Value);
        // исправление орфографии
        var correctedTranslation = await _textMistakeManager.Correct(translation);
        entity.Translation = correctedTranslation;

        return entity;
    }


    private static TranslationStyle? GetMostProbableStyle(StyleDefinitionResult result)
    {
        var maxProbability = result.ProbabilityOfSuccess.Max(x => x.Probability);
        var mostProbableStyles = result.ProbabilityOfSuccess.Where(x => x.Probability == maxProbability).ToList();

        if (mostProbableStyles.Count != 1)
            return null;

        return mostProbableStyles.Single().Style;
    }
}
