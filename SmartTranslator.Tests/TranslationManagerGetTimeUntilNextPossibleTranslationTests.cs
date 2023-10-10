using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using SmartTranslator.DataAccess;
using SmartTranslator.DataAccess.Entities;
using SmartTranslator.Enums;
using SmartTranslator.TelegramBot.Management.TranslationManagement;
using SmartTranslator.TranslationCore.Abstractions;
using Xunit;

namespace SmartTranslator.Tests;

public class TranslationManagerGetTimeUntilNextPossibleTranslationTests
{
    private TranslationManager _translationManager;
    private TelegramTranslationDbContext _dbContext;

    public TranslationManagerGetTimeUntilNextPossibleTranslationTests()
    {
        var options = new DbContextOptionsBuilder<TelegramTranslationDbContext>()
                      .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique name to ensure isolation
                      .Options;

        _dbContext = new TelegramTranslationDbContext(options);

        var mapper = new Mock<IMapper>();
        var translator = new Mock<IGptTranslator>();
        var languageManager = new Mock<ILanguageManager>();
        var textMistakeManager = new Mock<ITextMistakeManager>();
        var domainEventDistributor = new Mock<IPublisher>();

        _translationManager = new TranslationManager(_dbContext, mapper.Object, translator.Object, languageManager.Object, textMistakeManager.Object, domainEventDistributor.Object);
    }

    [Fact]
    public void GivenNoTranslations_WhenGetTimeUntilNextPossibleTranslation_ShouldReturnZero()
    {
        var result = _translationManager.GetTimeUntilNextPossibleTranslation("test");
        Assert.Equal(TimeSpan.Zero, result);
    }

    [Fact]
    public void GivenRecentTranslation_WhenGetTimeUntilNextPossibleTranslation_ShouldReturnExpectedTime()
    {
        for (int i = 0; i < 3; i++)
        {
            _dbContext.TelegramTranslations.Add(CreateTranslationEntity("test", DateTime.UtcNow.AddSeconds(-5)));
        }
        _dbContext.SaveChanges();

        var result = _translationManager.GetTimeUntilNextPossibleTranslation("test");

        Assert.InRange(result, TimeSpan.FromSeconds(50), TimeSpan.FromSeconds(60));
    }

    [Fact]
    public void GivenMultipleTranslations_WhenGetTimeUntilNextPossibleTranslation_ShouldReturnMaxTime()
    {
        for (int i = 0; i < 30; i++)
        {
            _dbContext.TelegramTranslations.Add(CreateTranslationEntity("test", DateTime.UtcNow.AddMinutes(-5)));
        }
        _dbContext.SaveChanges();

        var result = _translationManager.GetTimeUntilNextPossibleTranslation("test");

        Assert.InRange(result, TimeSpan.FromSeconds(86050), TimeSpan.FromSeconds(86150));
    }

    private TelegramTranslationEntity CreateTranslationEntity(string userName, DateTime createdAt)
    {
        return new TelegramTranslationEntity
        {
            UserName = userName,
            CreatedAt = createdAt,
            UpdatedAt = createdAt, // Or you can use DateTime.UtcNow for this
            ChatId = 12345, // Example chat ID, use a meaningful value if necessary
            BaseText = "Test Base Text",
            State = TelegramTranslationState.Finished, // Use a default or meaningful state if necessary
            Feedback = TranslationFeedback.Liked // Use a default or meaningful feedback if necessary
                                                               // Fill other fields as necessary for your tests.
        };
    }

}
