using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SmartTranslator.DataAccess;
using SmartTranslator.DataAccess.Entities;
using SmartTranslator.Enums;
using SmartTranslator.TelegramBot.Management.Exceptions;
using SmartTranslator.TelegramBot.Management.GptTelegramBots.Events;
using SmartTranslator.TranslationCore;
using SmartTranslator.TranslationCore.Abstractions.Exceptions;

namespace SmartTranslator.TelegramBot.Management;

public class StatisticsManagement : INotificationHandler<TextWasTranslatedEvent>,
                                    INotificationHandler<HandleMessageFailedEvent>,
                                    INotificationHandler<UserHasBlockedBotEvent>,
                                    INotificationHandler<UserHasActivatedBotEvent>
{
    private readonly StatisticsDbContext _context;
    private readonly GptTranslationOptions _gptTranslationOptions;

    public StatisticsManagement(StatisticsDbContext context,
                                GptTranslationOptions gptTranslationOptions)
    {
        _context = context;
        _gptTranslationOptions = gptTranslationOptions;
    }


    public async Task Handle(TextWasTranslatedEvent @event, CancellationToken ct)
    {
        var languagePair = GetLanguagePair();

        var userStatistics = await _context.TextTranslationStatistics.FirstOrDefaultAsync(u => u.UserName == @event.Translation.UserName && u.LanguagePair == languagePair);
        var isToFirstLanguage = _gptTranslationOptions.CoupleLanguage.Item1 == @event.Translation.LanguageTo;

        if (userStatistics == null)
        {
            userStatistics = new TextTranslationStatisticsByUserEntity
            {
                Id = Guid.NewGuid(),
                FirstAt = @event.Translation.UpdatedAt,
                LastAt = @event.Translation.UpdatedAt,
                UserName = @event.Translation.UserName,
                LanguagePair = languagePair,
                ToFirstLanguageCount = isToFirstLanguage ? 1 : 0,
                TranslationStyle = @event.Translation.TranslationStyle.ToString(),
                SuccessfullyUsedTranslationCount = 1,
                AverageTextSize = @event.Translation.BaseText.Length
            };

            _context.TextTranslationStatistics.Add(userStatistics);
        }
        else
        {
            // If user already exists, update his statistics
            userStatistics.LastAt = @event.Translation.UpdatedAt;
            userStatistics.LanguagePair = languagePair;
            userStatistics.ToFirstLanguageCount = isToFirstLanguage ? userStatistics.ToFirstLanguageCount + 1 : userStatistics.ToFirstLanguageCount;
            userStatistics.TranslationStyle = @event.Translation.TranslationStyle.ToString();
            userStatistics.AverageTextSize = (userStatistics.AverageTextSize * userStatistics.SuccessfullyUsedTranslationCount + @event.Translation.BaseText.Length) / (userStatistics.SuccessfullyUsedTranslationCount + 1);
            userStatistics.SuccessfullyUsedTranslationCount += 1;
        }

        await _context.SaveChangesAsync(ct);
    }


    public async Task Handle(HandleMessageFailedEvent @event, CancellationToken ct)
    {
        var exceptionStr = @event?.Exception is null ? "No exception"
            : $"Message: {@event.Exception.Message} {Environment.NewLine} InnerException: {@event.Exception.InnerException?.Message} {Environment.NewLine} {@event.Exception.StackTrace}";

        var entity = new HandleMessageFailedStatisticsEntity()
        {
            Id = Guid.NewGuid(),
            HappennedAt = DateTime.UtcNow,
            Priority = GetExceptionPriority(@event?.Exception),
            ExceptionName = @event?.Exception?.GetType()?.Name,
            UserName = @event?.Update?.Message?.From?.Username ?? "Unknown",
            MessageJson = JsonConvert.SerializeObject(@event?.Update),
            Exception = exceptionStr,
        };

        _context.HandleMessageFailedStatistics.Add(entity);

        await _context.SaveChangesAsync(ct);
    }


    public async Task Handle(UserHasBlockedBotEvent notification, CancellationToken ct)
    {
        var entity = new BotSubscriptionStatistics()
        {
            Id = Guid.NewGuid(),
            HappennedAt = DateTime.UtcNow,
            UserName = notification.UserName,
            ActionType = BotSubscriptionActionType.Blocked,
            LanguagePair = GetLanguagePair()
        };

        _context.BotSubscriptionStatistics.Add(entity);

        await _context.SaveChangesAsync(ct);
    }

    public async Task Handle(UserHasActivatedBotEvent notification, CancellationToken cancellationToken)
    {
        var entity = new BotSubscriptionStatistics()
        {
            Id = Guid.NewGuid(),
            HappennedAt = DateTime.UtcNow,
            UserName = notification.UserName,
            ActionType = BotSubscriptionActionType.Activated,
            LanguagePair = GetLanguagePair()
        };

        _context.BotSubscriptionStatistics.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }


    private ExceptionPriority GetExceptionPriority(Exception? e)
    {
        if (e is null)
            return ExceptionPriority.Unknown;

        var exceptionName = e.GetType().Name;

        return exceptionName switch
        {
            nameof(TextIsTooLongException) => ExceptionPriority.Low,
            "UnknownMessageTypeException" => ExceptionPriority.Low,
            "VoiceMessageTypeNotImplementedException" => ExceptionPriority.Low,
            nameof(EntityNotFoundException) => ExceptionPriority.Medium,
            nameof(RateLimitException) => ExceptionPriority.Medium,
            nameof(GptOverloadedException) => ExceptionPriority.Medium,
            nameof(ContextEvaluationErrorException) => ExceptionPriority.High,
            nameof(CorrectionErrorException) => ExceptionPriority.High,
            nameof(FailedToTranslateException) => ExceptionPriority.High,
            nameof(StyleDefinitionErrorException) => ExceptionPriority.High,
            nameof(UnknownLanguageException) => ExceptionPriority.High,
            nameof(UnknownModelException) => ExceptionPriority.High,
            _ => ExceptionPriority.High,
        };
    }


    private string GetLanguagePair()
    {
        return $"{_gptTranslationOptions.CoupleLanguage.Item1}-{_gptTranslationOptions.CoupleLanguage.Item2}";
    }
}
