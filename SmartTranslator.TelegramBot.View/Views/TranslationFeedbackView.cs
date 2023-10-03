using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.Enums;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class TranslationFeedbackView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;

    public TranslationFeedbackView(CoupleLanguageTranslatorController coupleLanguageTranslatorController)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
    }


    public async Task<MessageView> Render(Update update)
    {
        var data = update.CallbackQuery?.Data;

        if (data == null)
        {
            throw new ArgumentException("TranslationFeedbackView got incorrect update");
        }

        var feedback = StringToFeedback(data);
        await _coupleLanguageTranslatorController.AddFeedback(update, feedback);

        return new MessageView
        {
            Text = "Thank you for your feedback",
            Markup = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton(TelegramBotButtons.Translate)
            })
            {
                ResizeKeyboard = true
            }
        };
    }


    private static TranslationFeedback StringToFeedback(string text)
    {
        return text switch
        {
            TelegramBotInlineButtons.Like => TranslationFeedback.Liked,
            TelegramBotInlineButtons.Dislike => TranslationFeedback.Disliked,
            _ => throw new Exception("Unknown feedback")
        };
    }
}
