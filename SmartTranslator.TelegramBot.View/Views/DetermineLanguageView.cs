using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.TelegramBot.View.Controls;
using SmartTranslator.TranslationCore.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class DetermineLanguageView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;
    private readonly TranslationViewProvider _viewProvider;

    public DetermineLanguageView(CoupleLanguageTranslatorController coupleLanguageTranslatorController,
                                 TranslationViewProvider viewProvider)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
        _viewProvider = viewProvider;
    }


    public Task<MessageView> Render(Update update)
    {
        if (update.Message == null)
            throw new ArgumentException("DetermineLanguageView got incorrect update (Message == null)");

        var text = "Failed to determine request language, please choose one of the options provided";

        var languageButtons = (new TelegramBotLanguageButtons()).Buttons.Select(button => new KeyboardButton(button)).ToArray();
        var translateButton = new KeyboardButton(TelegramBotButtons.Translate);
        var keyboard = new ReplyKeyboardMarkup(new[] { languageButtons, new[] { translateButton } });


        return Task.FromResult(new MessageView
        {
            Text = text,
            Markup = keyboard
        });
    }

    private Task<MessageView> UnknownLanguageView()
    {
        var text = "Failed to determine request language, please choose one of the options provided";

        var languageButtons = (new TelegramBotLanguageButtons()).Buttons.Select(button => new KeyboardButton(button)).ToArray();
        var translateButton = new KeyboardButton(TelegramBotButtons.Translate);
        var keyboard = new ReplyKeyboardMarkup(new[] { languageButtons, new[] { translateButton } });


        return Task.FromResult(new MessageView
        {
            Text = text,
            Markup = keyboard
        });
    }
}
