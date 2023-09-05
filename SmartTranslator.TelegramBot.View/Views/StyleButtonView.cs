using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.TelegramBot.View.Controls;
using SmartTranslator.TranslationCore.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class StyleButtonView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;
    private readonly TranslationViewProvider _viewProvider;

    public StyleButtonView(CoupleLanguageTranslatorController coupleLanguageTranslatorController,
                              TranslationViewProvider viewProvider)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
        _viewProvider = viewProvider;
    }


    public async Task<MessageView> Render(Update update)
    {
        if (update.Message == null)
            throw new ArgumentException("StyleButtonView got incorrect update (Message == null)");
        if (update.Message?.Text == null)
            throw new ArgumentException("StyleButtonView got incorrect update (Text == null)");

        var style = ButtonToStyle(update.Message.Text);

        var dto = await _coupleLanguageTranslatorController.SetStyle(update, style);

        return await _viewProvider.GetTranslationView(dto).Render(update);
    }


    private static TranslationStyle ButtonToStyle(string text)
    {
        return text switch
        {
            TelegramBotStyleButtons.OfficialStyle => TranslationStyle.OfficialStyle,
            TelegramBotStyleButtons.ConversationalStyle => TranslationStyle.ConversationalStyle,
            TelegramBotStyleButtons.TeenageStyle => TranslationStyle.TeenageStyle,
            _ => throw new Exception("Unknown style")
        };
    }
}