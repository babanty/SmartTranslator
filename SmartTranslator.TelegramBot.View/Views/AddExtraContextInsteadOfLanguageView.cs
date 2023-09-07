using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.Infrastructure.TemplateStrings;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;

namespace SmartTranslator.TelegramBot.View.Views;

public class AddExtraContextInsteadOfLanguageView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;
    private readonly ITemplateStringService _templateStringService;


    public AddExtraContextInsteadOfLanguageView(CoupleLanguageTranslatorController coupleLanguageTranslatorController,
                               ITemplateStringService templateStringService)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
        _templateStringService = templateStringService;
    }


    public async Task<MessageView> Render(Update update)
    {
        var dto = await _coupleLanguageTranslatorController.AddExtraContext(update);

        // TODO: fix template string service
        // var message = await _templateStringService.GetSingle("ReceivedAdditionalContextNowPleaseChooseLanguage");
        var message = "We received the additional context you provided, now, please, choose one of the language options below";

        var languageButtons = (new TelegramBotLanguageButtons()).Buttons.Select(button => new KeyboardButton(button)).ToArray();
        var translateButton = new KeyboardButton(TelegramBotButtons.Translate);
        var keyboard = new ReplyKeyboardMarkup(new[] { languageButtons, new[] { translateButton } });

        return new MessageView
        {
            Text = message, // .Format(new List<KeyAndNewValue>()),
            Markup = keyboard
        };
    }
}
