using SmartTranslator.Api.Exceptions;
using SmartTranslator.DataAccess.Entities;
using SmartTranslator.TelegramBot.Management.TranslationManagement;
using SmartTranslator.TranslationCore.Abstractions.Models;
using SmartTranslator.TranslationCore.Enums;
using Telegram.Bot.Types;

namespace SmartTranslator.Api.TelegramControllers;

public class CoupleLanguageTranslatorController
{
    public async Task NewUser(ChatMemberUpdated chatMemberUpdated)
    {
        return;
    }

    public Task<string> Translate(Message message)
    {
        return Task.FromResult("I don't know how to translate yet");
    }

    public Task<Language> DetermineLanguage(Message message)
    {
        return Task.FromResult(Language.Unknown);
    }


    public async Task SetLanguage(Language language)
    {
        return;
    }


    public async Task<EvaluationResponse> EvaluateContext(Message message)
    {
        var response = new EvaluationResponse
        {
            Percent = 0,
            Request = new ClarificationRequest
            {
                ClarifyingQuestion = "What is the meaning of life?"
            }
        };

        return await Task.FromResult(response);
    }


    public Task<TranslationStyle> DetermineStyle(Message message)
    {
        return Task.FromResult(TranslationStyle.Unknown);
    }


    public async Task SetStyle(TranslationStyle style)
    {
        return;
    }


    public async Task<string> GiveFinalAnswer(Message message)
    {
        return $"Your message was: {message.Text}";
    }


    public async Task<TelegramTranslationEntity?> GetLatest(Update update, ITranslationManager manager)
    {
        if (update?.Message?.From == null)
            throw new ChannelsNotSupportedException();

        var userName = update.Message.From.ToString(); 
        var chatId = update.Message.Chat.Id;

        return await manager.GetLatest(userName, chatId);
    }
}
