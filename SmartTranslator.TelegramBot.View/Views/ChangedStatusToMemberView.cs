using Telegram.Bot.Types;

namespace SmartTranslator.TelegramBot.View.Views;

public class ChangedStatusToMemberView : ITelegramBotView
{
    public Task<MessageView> Render(Update update)
    {
        // NOTE: because of telegram's quirks answer resides in the "Text" section, and all buttons are in Markup section
        return Task.FromResult(new MessageView
        {
            Text = string.Empty,
            Markup = null
        });
    }
}
