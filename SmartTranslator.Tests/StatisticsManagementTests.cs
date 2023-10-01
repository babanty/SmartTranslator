using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using SmartTranslator.TelegramBot.Management.GptTelegramBots.Events;
using SmartTranslator.TranslationCore.Abstractions.Exceptions;
using SmartTranslator.DataAccess;
using SmartTranslator.TelegramBot.Management.Exceptions;
using SmartTranslator.TelegramBot.Management;
using SmartTranslator.TranslationCore.Enums;
using SmartTranslator.TranslationCore;
using SmartTranslator.DataAccess.Entities;
using static SmartTranslator.TranslationCore.GptTranslationOptions;
using SmartTranslator.Enums;
using Telegram.Bot.Types;

namespace SmartTranslator.Tests;

public class StatisticsManagementTests : IDisposable
{
    private readonly StatisticsDbContext _testDbContext;
    private readonly Mock<INotification> _mockNotification;
    private readonly StatisticsManagement _statisticsManagement;
    private readonly GptTranslationOptions _testGptTranslationOptions = new GptTranslationOptions
    {
        CoupleLanguage = new CoupleLanguageHolder 
        { 
            Item1 = Language.Russian,
            Item2 = Language.English
        }
    };

    public StatisticsManagementTests()
    {
        var options = new DbContextOptionsBuilder<StatisticsDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _testDbContext = new StatisticsDbContext(options);
        _mockNotification = new Mock<INotification>();
        _statisticsManagement = new StatisticsManagement(_testDbContext, _testGptTranslationOptions);
    }

    [Fact]
    public async Task TextWasTranslatedEvent_NewUser_EntryAdded()
    {
        var translation = new TelegramTranslationEntity
        {
            ChatId = 1,
            State = TelegramTranslationState.Finished,
            BaseText = "Test",
            Translation = "Тест",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserName = "TestUser",
            Id = "TestId",
            LanguageFrom = Language.English,
            LanguageTo = Language.Russian,
            TranslationStyle = TranslationStyle.ConversationalStyle,
            Contexts = Array.Empty<Context>()
        };

        var @event = new TextWasTranslatedEvent(translation);

        await _statisticsManagement.Handle(@event, CancellationToken.None);

        var storedEntity = await _testDbContext.TextTranslationStatistics.FirstOrDefaultAsync(e => e.UserName == "TestUser");

        Assert.NotNull(storedEntity);
        Assert.Equal(1, storedEntity.SuccessfullyUsedTranslationCount);
    }

    [Fact]
    public async Task HandleMessageFailedEvent_Error_StoredWithExceptionDetails()
    {
        var username = "TestUsername";
        var exception = new EntityNotFoundException();
        var update = new Update
        {
            
            Message = new Message
            {
                From = new User
                {
                    Username = username,
                    FirstName = "Test"
                },
                Date = DateTime.UtcNow,
                Chat = new Chat
                {
                    Id = 1,
                }
            }
        };

        var @event = new HandleMessageFailedEvent(update, exception);

        await _statisticsManagement.Handle(@event, CancellationToken.None);

        var storedEntity = await _testDbContext.HandleMessageFailedStatistics.FirstOrDefaultAsync(e => e.ExceptionName == nameof(EntityNotFoundException));

        Assert.NotNull(storedEntity);
        Assert.Equal(username ,storedEntity.UserName);
    }


    public void Dispose()
    {
        _testDbContext.Database.EnsureDeleted();
        _testDbContext.Dispose();
    }
}
