using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.Infrastructure.TemplateStrings;
using SmartTranslator.Infrastructure.TemplateStringServiceWithUserLanguage;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class StartButtonView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;
    private readonly ITemplateStringServiceWithUserLanguage _templateStringService;

    public StartButtonView(CoupleLanguageTranslatorController coupleLanguageTranslatorController,
                           ITemplateStringServiceWithUserLanguage templateStringService)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
        _templateStringService = templateStringService;
    }


    public async Task<MessageView> Render(Update update)
    {
        await _coupleLanguageTranslatorController.NewUser(update.MyChatMember!);

        return await GreetingLetterView(update);
    }


    private async Task<MessageView> GreetingLetterView(Update update)
    {
        var greetingText = await _templateStringService.GetSingle("Hello, new user");
        var replyKeyboard = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton(TelegramBotButtons.Translate)
        })
        {
            ResizeKeyboard = true
        };


        return new MessageView
        {
            Text = greetingText.Format(new List<KeyAndNewValue>()),
            Markup = replyKeyboard
        };
    }
}
