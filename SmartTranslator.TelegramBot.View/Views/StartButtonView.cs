using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class StartButtonView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;

    public StartButtonView(CoupleLanguageTranslatorController coupleLanguageTranslatorController)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
    }


    public async Task<MessageView> Render(Update update)
    {
        await _coupleLanguageTranslatorController.NewUser(update.MyChatMember!);

        return await GreetingLetterView(update);
    }


    private Task<MessageView> GreetingLetterView(Update update)
    {
        var greetingText = "Привет, новый пользователь / Hello, new user";
        var replyKeyboard = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton(TelegramBotButtons.Translate)
        });


        return Task.FromResult(new MessageView
        {
            Text = greetingText,
            Markup = replyKeyboard
        });
    }
}
