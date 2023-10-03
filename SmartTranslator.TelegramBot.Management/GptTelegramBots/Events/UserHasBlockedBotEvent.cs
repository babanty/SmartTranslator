using MediatR;

namespace SmartTranslator.TelegramBot.Management.GptTelegramBots.Events;

public record UserHasBlockedBotEvent(string UserName) : INotification;
