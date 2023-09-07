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

    public AddExtraContextInsteadOfStyleView(CoupleLanguageTranslatorController coupleLanguageTranslatorController,
                               ITemplateStringService templateStringService)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
        _templateStringService = templateStringService;
    }


    public async Task<MessageView> Render(Update update)
    {
        var dto = await _coupleLanguageTranslatorController.AddExtraContext(update);
        // TODO: fix template string service
        // var message = await _templateStringService.GetSingle("ReceivedAdditionalContextNowPleaseChooseStyle");
        var message = "We received the additional context you provided, now, please, choose one of the style options below";

        var buttons = (new TelegramBotStyleButtons()).Buttons.Select(button => new KeyboardButton(button)).ToArray();
        buttons.Append(new KeyboardButton(TelegramBotButtons.Translate));
        var markup = new ReplyKeyboardMarkup(buttons);

        return new MessageView
        {
            Text = message, // .Format(new List<KeyAndNewValue>()),
            Markup = markup
        };
    }
}
