using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.Infrastructure.TemplateStrings;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class AddExtraContextInsteadOfStyleView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;
    private readonly ITemplateStringService _templateStringService;
    private readonly IButtonsHolder _telegramBotStyleButtons;


    public AddExtraContextInsteadOfStyleView(CoupleLanguageTranslatorController coupleLanguageTranslatorController,
                               ITemplateStringService templateStringService,
                               TelegramBotStyleButtons telegramBotStyleButtons)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
        _templateStringService = templateStringService;
        _telegramBotStyleButtons = telegramBotStyleButtons;
    }


    public async Task<MessageView> Render(Update update)
    {
        await _coupleLanguageTranslatorController.AddExtraContext(update);

        var buttons = _telegramBotStyleButtons.Buttons.Select(button => new KeyboardButton(button)).ToArray();
        // TODO: add such a template to service
        var message = await _templateStringService.GetSingle("ReceivedAdditionalContextNowPleaseChooseStyle");
        var markup = new ReplyKeyboardMarkup(buttons);

        return new MessageView
        {
            Text = message.Format(new List<KeyAndNewValue>()),
            Markup = markup
        };
    }
}
