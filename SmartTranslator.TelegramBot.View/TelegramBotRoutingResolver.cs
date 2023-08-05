using SmartTranslator.TelegramBot.View.Exceptions;
using SmartTranslator.TelegramBot.View.Views;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SmartTranslator.TelegramBot.View;

public class TelegramBotRoutingResolver
{
    public async Task<ITelegramBotView?> RouteMessageOrThrow(Update update, List<ITelegramBotView> telegramBotViews)
    {
        T GetView<T>() where T : ITelegramBotView
        {
            var view = telegramBotViews.OfType<T>().FirstOrDefault();

            return view is null ? throw new InvalidOperationException($"Не найден обработчик для сообщения типа {typeof(T).Name}") : view;
        }

        if (update is null)
            return null;

        // translation
        if (update.Type == UpdateType.Message && update.Message?.Type == MessageType.Text)
        {
            var messageText = update?.Message?.Text;

            return messageText switch
            {

            };
        }

        // /start /block etc
        if (update.Type == UpdateType.MyChatMember
            && update?.MyChatMember?.NewChatMember?.Status != update?.MyChatMember?.OldChatMember?.Status)
        {
            if (update?.MyChatMember?.NewChatMember?.Status == ChatMemberStatus.Kicked)
            {
                // return await GetView<BlockButtonView>().Render(update);
            }

            if (update?.MyChatMember?.NewChatMember?.Status == ChatMemberStatus.Member)
            {
                return GetView<ChangedStatusToMemberView>();
            }
        }

        // audio msg
        if (update?.Type == UpdateType.Message && update.Message?.Type == MessageType.Voice)
        {
            throw new VoiceMessageTypeNotImplementedException();
        }

        throw new UnknownMessageTypeException();
    }
}
