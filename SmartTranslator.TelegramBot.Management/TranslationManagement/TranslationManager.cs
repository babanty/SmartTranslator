using Microsoft.EntityFrameworkCore;
using SmartTranslator.DataAccess;
using SmartTranslator.DataAccess.Entities;

namespace SmartTranslator.TelegramBot.Management.TranslationManagement;

public class TranslationManager : ITranslationManager
{
    private readonly TelegramTranslationDbContext _dbContext;

    public TranslationManager(TelegramTranslationDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<TelegramTranslationEntity> GetLatest(string username, long chatId)
    {
        return await _dbContext.TelegramTranslations
                               .Where(t => t.UserName == username && t.ChatId == chatId)
                               .OrderByDescending(t => t.CreatedAt)
                               .FirstOrDefaultAsync();
    }

}
