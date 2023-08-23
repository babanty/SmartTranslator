using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.TelegramBot.View.Controls;
using SmartTranslator.TranslationCore.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class DetermineStyleView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;

    public DetermineStyleView(CoupleLanguageTranslatorController coupleLanguageTranslatorController)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
    }


    public async Task<MessageView> Render(Update update)
    {
        if (update.Message == null)
            throw new ArgumentException("DetermineStyleView got incorrect update (Message == null)");

        var language = await _coupleLanguageTranslatorController.DetermineLanguage(update.Message);

        if (language == Language.Unknown)
        {
            return await UnknownStyleView();
        }

        return await Task.FromResult(new MessageView
        {
            Text = "Style determined successfully"
        });
    }


    private Task<MessageView> UnknownStyleView()
    {
        var text = "Failed to determine style of request, please choose one of the options provided";

        var buttons = (new TelegramBotLanguageButtons()).Buttons.Select(button => new KeyboardButton(button)).ToArray();
        var markup = new ReplyKeyboardMarkup(buttons);


        return Task.FromResult(new MessageView
        {
            Text = text,
            Markup = markup
        });
    }
}
