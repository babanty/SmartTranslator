using Telegram.Bot.Types;

namespace SmartTranslator.TelegramBot.View.Views;

public class ChangedStatusToMemberView : ITelegramBotView
{
    public Task<MessageView> Render(Update update)
    {
        // NOTE: из-за особенностей телеграм ответ в секции "translation", реакция на TelegramBotButtons.Start -> StartButtonView
        return Task.FromResult(new MessageView 
        {
            Text = string.Empty,
            Markup = null
        });
    }
}
