using Microsoft.EntityFrameworkCore;
using SmartTranslator.DataAccess;
using SmartTranslator.DataAccess.Entities;
using SmartTranslator.Enums;

namespace SmartTranslator.TelegramBot.Management.TranslationManagement;

public class TranslationManager : ITranslationManager
{
    private readonly TelegramTranslationDbContext _dbContext;

    public TranslationManager(TelegramTranslationDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<TelegramTranslationEntity?> GetLatest(string username, long chatId)
    {
        return await _dbContext.TelegramTranslations
                               .Where(t => t.UserName == username && t.ChatId == chatId)
                               .OrderByDescending(t => t.CreatedAt)
                               .FirstOrDefaultAsync();
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
