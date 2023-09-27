using MediatR;

namespace SmartTranslator.TelegramBot.Management.GptTelegramBots.Events;

public record UserHasActivatedBotEvent(string UserName) : INotification;
