using MediatR;
using Telegram.Bot.Types;

namespace SmartTranslator.TelegramBot.Management.GptTelegramBots.Events;

public record HandleMessageFailedEvent(Update Update, Exception Exception) : INotification;
