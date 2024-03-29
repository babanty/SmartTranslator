﻿using AutoMapper;
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

public class TranslationManagerCountTimeoutTests
{
    private TranslationManager _translationManager;
    private TelegramTranslationDbContext _dbContext;

    public TranslationManagerCountTimeoutTests()
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
        var rateLimitOptions = new RateLimitOptions
        {
            RateLimits = new RateLimit[]
            {
                new RateLimit
                {
                    AllowedTranslations = 3,
                    TimeSpanInSeconds = 60
                },
                new RateLimit
                {
                    AllowedTranslations = 30,
                    TimeSpanInSeconds = 86400
                }
            }
        };

        _translationManager = new TranslationManager(_dbContext, mapper.Object, translator.Object, languageManager.Object, textMistakeManager.Object, domainEventDistributor.Object, rateLimitOptions);
    }

    [Fact]
    public void GivenNoTranslations_WhenGetTimeUntilNextPossibleTranslation_ShouldReturnZero()
    {
        var result = _translationManager.CountTimeout("test");
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

        var result = _translationManager.CountTimeout("test");

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

        var result = _translationManager.CountTimeout("test");

        Assert.InRange(result, TimeSpan.FromSeconds(86050), TimeSpan.FromSeconds(86150));
    }

    private TelegramTranslationEntity CreateTranslationEntity(string userName, DateTime createdAt)
    {
        return new TelegramTranslationEntity
        {
            UserName = userName,
            CreatedAt = createdAt,
            UpdatedAt = createdAt,
            ChatId = 12345,
            BaseText = "Test Base Text",
            State = TelegramTranslationState.Finished,
            Feedback = TranslationFeedback.Liked
        };
    }

}
