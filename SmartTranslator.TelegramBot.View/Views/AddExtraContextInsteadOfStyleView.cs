using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.Infrastructure.TemplateStrings;
using SmartTranslator.Infrastructure.TemplateStringServiceWithUserLanguage;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class AddExtraContextInsteadOfStyleView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;
    private readonly ITemplateStringServiceWithUserLanguage _templateStringService;

    public AddExtraContextInsteadOfStyleView(CoupleLanguageTranslatorController coupleLanguageTranslatorController,
                               ITemplateStringServiceWithUserLanguage templateStringService)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
        _templateStringService = templateStringService;
    }


    public async Task<MessageView> Render(Update update)
    {
        var dto = await _coupleLanguageTranslatorController.AddExtraContext(update);

        var message = await _templateStringService.GetSingle("ReceivedAdditionalContextNowPleaseChooseStyle");

        var buttons = (new TelegramBotStyleButtons()).Buttons.Select(button => new KeyboardButton(button)).ToArray();
        buttons.Append(new KeyboardButton(TelegramBotButtons.Translate));
        var markup = new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };

        return new MessageView
        {
            Text = message.Format(new List<KeyAndNewValue>()),
            Markup = markup
        };
    }
}
