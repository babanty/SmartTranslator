using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class ClarifyContextView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;
    private readonly TranslationViewProvider _viewProvider;

    public ClarifyContextView(CoupleLanguageTranslatorController coupleLanguageTranslatorController,
                              TranslationViewProvider viewProvider)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
        _viewProvider = viewProvider;
    }


    public async Task<MessageView> Render(Update update)
    {
        if (update.Message == null)
            throw new ArgumentException("ClarifyContextView got incorrect update (Message == null)");

        var response = await _coupleLanguageTranslatorController.EvaluateContext(update);

        var dto = response.Item1;
        var question = response.Item2;

        if (question == null)
            return await _viewProvider.GetTranslationView(dto).Render(update);

        return new MessageView
        {
            Text = $"Not enough context provided, please, answer the following quesion: {question}", // TODO: make multilingual
            Markup = new ReplyKeyboardMarkup(new KeyboardButton(TelegramBotButtons.Translate))
        };
    }
}
