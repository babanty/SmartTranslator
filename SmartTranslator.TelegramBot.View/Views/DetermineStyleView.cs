using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.TelegramBot.View.Controls;
using SmartTranslator.TranslationCore.Enums;
using System.Reflection;
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
        var language = await _coupleLanguageTranslatorController.DetermineLanguage(update.Message);

        if (language == Language.Unknown)
        {
            return await UnknownStyleView();
        }

        return await Task.FromResult(new MessageView
        {
            Text = "Style determined successfully"
        });
    }


    private Task<MessageView> UnknownStyleView()
    {
        var text = "Failed to determine style of request, please choose one of the options provided";

        var buttons = typeof(TelegramBotStyleButtons)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(field => new KeyboardButton(field.GetValue(null).ToString()))
            .ToArray();

        var replyKeyboard = new ReplyKeyboardMarkup(buttons);

        return Task.FromResult(new MessageView
        {
            Text = text,
            Markup = replyKeyboard
        });
    }
}
