using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class NewTranslationView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;
    private readonly TranslationViewProvider _viewProvider;

    public NewTranslationView(CoupleLanguageTranslatorController coupleLanguageTranslatorController,
                              TranslationViewProvider viewProvider)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
        _viewProvider = viewProvider;
    }


    public async Task<MessageView> Render(Update update)
    {
        var timeout = _coupleLanguageTranslatorController.GetTimeUntilNextPossibleTranslation(update);

        if (update.Message.Text.Length < 5)
        {
            return new MessageView
            {
                // TODO: make multilingual
                Text = $"Your message was too short, it must be 5 characters or more",
                Markup = new ReplyKeyboardMarkup(new[]
                {
                new KeyboardButton(TelegramBotButtons.Translate)
                })
                {
                    ResizeKeyboard = true
                }
            };
        }

        if (timeout > TimeSpan.Zero)
        {
            var timeoutString = FormatTimeSpan(timeout);
            return new MessageView
            {
                // TODO: make multilingual
                Text = $"You have exceeded the translation limit. Please wait {timeoutString}",
                Markup = new ReplyKeyboardMarkup(new[]
                {
                new KeyboardButton(TelegramBotButtons.Translate)
                })
                {
                    ResizeKeyboard = true
                }
            };
        }

        var translation = await _coupleLanguageTranslatorController.CreateTranslation(update);

        return await _viewProvider.GetTranslationView(translation).Render(update);
    }


    private static string FormatTimeSpan(TimeSpan span)
    {
        if (span.TotalHours >= 1)
        {
            return $"{span.Hours} hour(s) {span.Minutes} minute(s) {span.Seconds} second(s)";
        }
        else if (span.TotalMinutes >= 1)
        {
            return $"{span.Minutes} minute(s) {span.Seconds} second(s)";
        }
        else
        {
            return $"{span.Seconds} second(s)";
        }
    }
}