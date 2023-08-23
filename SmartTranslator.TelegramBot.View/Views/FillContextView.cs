using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class FillContextView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;

    public FillContextView(CoupleLanguageTranslatorController coupleLanguageTranslatorController)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
    }


    public async Task<MessageView> Render(Update update)
    {
        await _coupleLanguageTranslatorController.AddAnswerToContextQuestion(update);
        var message = "Received answer to question about context";

        return new MessageView
        {
            Text = message,
            Markup = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton(TelegramBotButtons.Translate)
            })
        };
    }
}
