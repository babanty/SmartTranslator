using SmartTranslator.Api.TelegramControllers;
using Telegram.Bot.Types;

namespace SmartTranslator.TelegramBot.View.Views;

public class DefaultTranslateButtonView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;

    public DefaultTranslateButtonView(CoupleLanguageTranslatorController coupleLanguageTranslatorController)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
    }

    public async Task<MessageView> Render(Update update)
    {
        var text = update?.Message is null ? string.Empty : await _coupleLanguageTranslatorController.Translate(update.Message);

        return new MessageView
        {
            Text = text,
            Markup = null
        };
    }
}

