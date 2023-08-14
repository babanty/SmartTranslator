using SmartTranslator.DataAccess.Entities;

namespace SmartTranslator.TelegramBot.Management.TranslationManagement;

public interface ITranslationManager
{
    /// <summary>
    /// Gets the latest translation entity for the specified user in a given chat.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="chatId">The ID of the chat.</param>
    /// <returns>The latest <see cref="TelegramTranslationEntity"/> in the chat with the specified user and chat ID.</returns>
    Task<TelegramTranslationEntity> GetLatest(string username, long chatId);
}
