using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class FillContextView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;
    private readonly TranslationViewProvider _translationViewProvider;

    public FillContextView(CoupleLanguageTranslatorController coupleLanguageTranslatorController,
                           TranslationViewProvider translationViewProvider)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
        _translationViewProvider = translationViewProvider;
    }


    public async Task<MessageView> Render(Update update)
    {
        var dto = await _coupleLanguageTranslatorController.AddAnswerToContextQuestion(update);

        return await _translationViewProvider.GetTranslationView(dto).Render(update);
    }
}
