using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.Enums;
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
        var translation = await _coupleLanguageTranslatorController.CreateTranslation(update);

        if (translation.State == TelegramTranslationState.Finished)
        {
            return await Task.FromResult(new MessageView
            {
                Text = translation.Translation,
                Markup = new ReplyKeyboardMarkup(new[]
                {
                new KeyboardButton(TelegramBotButtons.Translate)
                })
                {
                    ResizeKeyboard = true
                }
            });
        }

        return await _viewProvider.GetTranslationView(translation).Render(update);
    }
}