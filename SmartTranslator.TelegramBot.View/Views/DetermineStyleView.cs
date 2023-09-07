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
        var text = "Failed to determine style of request, please choose one of the options provided";

        var buttons = (new TelegramBotStyleButtons()).Buttons.Select(button => new KeyboardButton(button)).ToArray();
        buttons.Append(new KeyboardButton(TelegramBotButtons.Translate));
        var markup = new ReplyKeyboardMarkup(buttons);

        return new MessageView
        {
            Text = text,
            Markup = markup
        };
    }
}
