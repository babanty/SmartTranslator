using MediatR;
using SmartTranslator.DataAccess.Entities;

namespace SmartTranslator.TelegramBot.Management.GptTelegramBots.Events;

public record TextWasTranslatedEvent(TelegramTranslationEntity Translation) : INotification;
