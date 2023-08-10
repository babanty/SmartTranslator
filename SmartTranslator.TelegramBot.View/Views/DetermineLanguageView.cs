using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.TelegramBot.View.Controls;
using SmartTranslator.TranslationCore.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class DetermineLanguageView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;

    public DetermineLanguageView(CoupleLanguageTranslatorController coupleLanguageTranslatorController)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
    }


    public async Task<MessageView> Render(Update update)
    {
        var language = await _coupleLanguageTranslatorController.DetermineLanguage(update.Message);

        if (language == Language.Unknown) 
        {
            return await UnknownLanguageView();
        }

        return await Task.FromResult(new MessageView
        {
            Text = "Language determined successfully"
        });
    }


    private Task<MessageView> UnknownLanguageView()
    {
        var text = "Failed to determine request language, please choose one of the options provided";

        var replyKeyboard = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton(TelegramBotLanguageButtons.Russian),
            new KeyboardButton(TelegramBotLanguageButtons.English)
        });

        return Task.FromResult(new MessageView
        {
            Text = text,
            Markup = replyKeyboard
        });
    }
}
