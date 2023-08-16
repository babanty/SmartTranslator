using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartTranslator.Contracts.Dto;
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


    private TelegramTranslationState DetermineState(TelegramTranslationEntity entity)
    {
        var state = TelegramTranslationState.Unknown;

        if (entity.Translation == null)
            state = TelegramTranslationState.WaitingForTranslation;

        if (entity.TranslationStyle == null)
            state = TelegramTranslationState.WaitingForStyle;

        if (entity.Contexts.Any(context => context.Response == null))
            state = TelegramTranslationState.WaitingForContext;

        if (entity.LanguageFrom != null && entity.LanguageTo != null)
            state = TelegramTranslationState.LanguageDetermined;

        if (entity.CreatedAt == entity.UpdatedAt)
            state = TelegramTranslationState.Created;

        if (entity.Translation != null)
            state = TelegramTranslationState.Finished;

        return state;
    }
}
