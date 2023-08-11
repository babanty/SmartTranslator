using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.TelegramBot.View.Controls;
using SmartTranslator.TranslationCore.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class StyleButtonView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;

    public StyleButtonView(CoupleLanguageTranslatorController coupleLanguageTranslatorController)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
    }


    public async Task<MessageView> Render(Update update)
    {
        var style = ButtonToStyle(update.Message.Text);

        await _coupleLanguageTranslatorController.SetStyle(style);

        return await Task.FromResult(new MessageView
        {
            Text = $"Style set to {update.Message.Text}",
            Markup = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton(TelegramBotButtons.Translate)
            })
        });
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