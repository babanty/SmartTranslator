using SmartTranslator.Contracts.Dto;
using SmartTranslator.Contracts.Requests;
using SmartTranslator.DataAccess.Entities;
using SmartTranslator.TranslationCore.Enums;

namespace SmartTranslator.TelegramBot.Management.TranslationManagement;

public interface ITranslationManager
{
    /// <summary>
    /// Gets the latest translation entity for the specified user in a given chat.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="chatId">The ID of the chat.</param>
    /// <returns>The latest <see cref="TelegramTranslationEntity"/> in the chat with the specified user and chat ID.</returns>
    Task<TelegramTranslationDto?> GetLatest(string username, long chatId);
    Task<TelegramTranslationDto> Create(CreateTelegramTranslationEntityRequest request);
    Task FinishTranslation(string translationId);
    Task<Language?> DetermineLanguage(string text);
    Task<(TelegramTranslationDto, string?)> DetermineContext(string translationId);
    Task<TelegramTranslationDto> SetLanguages(string translationId, Language baseLanguage);
    Task<TelegramTranslationDto> SetStyle(string translationId, TranslationStyle style);
}
