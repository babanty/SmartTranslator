using SmartTranslator.Api.TelegramControllers;
using Telegram.Bot.Types;

namespace SmartTranslator.TelegramBot.View.Views;

public class BlockButtonView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;

    public BlockButtonView(CoupleLanguageTranslatorController coupleLanguageTranslatorController)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
    }

    public async Task<MessageView> Render(Update update)
    {
        await _coupleLanguageTranslatorController.Block(update);

        return new MessageView
        {
            Text = string.Empty
        };
    }
}
