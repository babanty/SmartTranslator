using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class ClarifyContextView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;

    public ClarifyContextView(CoupleLanguageTranslatorController coupleLanguageTranslatorController)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
    }


    public Task<MessageView> Render(Update update)
    {
        if (update.Message == null)
            throw new ArgumentException("ClarifyContextView got incorrect update (Message == null)");

        var response = _coupleLanguageTranslatorController.EvaluateContext(update.Message).Result;

        if (response.Percent != 0)
        {
            return Task.FromResult(new MessageView
            {
                Text = "Enough context was provided",
                Markup = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton(TelegramBotButtons.Translate)
                })
            });
        }
        else
        {
            return Task.FromResult(new MessageView
            {
                Text = $"Too little context provided, please, answer the following question: " +
                $"{response.Request.ClarifyingQuestion}",
                Markup = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton(TelegramBotButtons.Translate)
                })
            });
        }
    }
}
