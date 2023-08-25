using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartTranslator.Contracts.Dto;
using SmartTranslator.Contracts.Requests;
using SmartTranslator.DataAccess;
using SmartTranslator.DataAccess.Entities;
using SmartTranslator.Enums;

namespace SmartTranslator.TelegramBot.Management.TranslationManagement;

public class TranslationManager : ITranslationManager
{
    private readonly TelegramTranslationDbContext _dbContext;
    private readonly IMapper _mapper;

    public TranslationManager(TelegramTranslationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
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
            LanguageFrom = request.LanguageFrom,
            LanguageTo = request.LanguageTo,
            TranslationStyle = request.TranslationStyle,
            Contexts = new List<Context>(),
            Translation = null
        };

        _dbContext.TelegramTranslations.Add(entity);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<TelegramTranslationDto>(entity);
    }
}
