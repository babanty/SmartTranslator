using MediatR;
using Telegram.Bot.Types;

namespace SmartTranslator.TelegramBot.View;

public record HandleMessageFailedEvent(Update Update, Exception Exception) : INotification;
