using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.Infrastructure.TemplateStrings;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class AddExtraContextInsteadOfLanguageView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;
    private readonly ITemplateStringService _templateStringService;
    private readonly IButtonsHolder _telegramBotLanguageButtons;


    public AddExtraContextInsteadOfLanguageView(CoupleLanguageTranslatorController coupleLanguageTranslatorController,
                               ITemplateStringService templateStringService,
                               TelegramBotLanguageButtons telegramBotLanguageButtons)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
        _templateStringService = templateStringService;
        _telegramBotLanguageButtons = telegramBotLanguageButtons;
    }


    public async Task<MessageView> Render(Update update)
    {
        await _coupleLanguageTranslatorController.AddExtraContext(update);

        var buttons = _telegramBotLanguageButtons.Buttons.Select(button => new KeyboardButton(button)).ToArray();
        // TODO: add such a template to service
        var message = await _templateStringService.GetSingle("ReceivedAdditionalContextNowPleaseChooseLanguage");
        var markup = new ReplyKeyboardMarkup(buttons);

        return new MessageView
        {
            Text = message.Format(new List<KeyAndNewValue>()),
            Markup = markup
        };
    }
}
