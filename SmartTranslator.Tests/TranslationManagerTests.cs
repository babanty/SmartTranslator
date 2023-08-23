using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartTranslator.DataAccess;
using SmartTranslator.DataAccess.Entities;
using SmartTranslator.Enums;
using SmartTranslator.TelegramBot.Management;
using SmartTranslator.TelegramBot.Management.TranslationManagement;
using SmartTranslator.TranslationCore.Enums;
using Xunit;

namespace SmartTranslator.Tests;

public class TranslationManagerTests : IDisposable
{
    private readonly TelegramTranslationDbContext _dbContext;
    private readonly TranslationManager _translationManager;
    private readonly MapperConfiguration _mapperConfiguration;
    private readonly Mapper _mapper;

    public TranslationManagerTests()
    {
        var options = new DbContextOptionsBuilder<TelegramTranslationDbContext>()
            .UseSqlite("Filename=:memory:")
            .Options;

        _dbContext = new TelegramTranslationDbContext(options);
        _dbContext.Database.OpenConnection();
        _dbContext.Database.EnsureCreated();

        _mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TelegramTranslationMappingProfile>();
        });
        _mapper = new Mapper(_mapperConfiguration);

        _translationManager = new TranslationManager(_dbContext, _mapper);
    }

    [Fact]
    public async void GetLatest_NoTranslation_ReturnsNull()
    {
        // Arrange
        var username = "nonExistingUser";
        var chatId = 99999L;

        // Act
        var result = await _translationManager.GetLatest(username, chatId);

        // Assert
        Assert.Null(result);
    }


    [Fact]
    public async void GetLatest_OneTranslation_ReturnsCorrectly()
    {
        // Arrange
        var username = "testUser";
        var chatId = 12345L;
        var baseText = "Translation base text";
        var translationText = "Translation";

        var translation = new TelegramTranslationEntity
        {
            Id = Guid.NewGuid().ToString(),
            UserName = username,
            ChatId = chatId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BaseText = baseText,
            Translation = translationText
        };
        _dbContext.TelegramTranslations.Add(translation);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _translationManager.GetLatest(username, chatId);

        // Assert
        Assert.Equal(translation.Id, result?.Id);
    }


    [Fact]
    public async void GetLatest_MultipleTranslations_ReturnsLatest()
    {
        // Arrange
        var username = "testUser";
        var chatId = 12345L;
        var oldBaseText = "Old translation base text";
        var newBaseText = "New translation base text";
        var translationText = "Translation";

        var oldTranslation = new TelegramTranslationEntity
        {
            Id = Guid.NewGuid().ToString(),
            UserName = username,
            ChatId = chatId,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            BaseText = oldBaseText,
            Translation = translationText
        };

        var latestTranslation = new TelegramTranslationEntity
        {
            Id = Guid.NewGuid().ToString(),
            UserName = username,
            ChatId = chatId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BaseText = newBaseText,
            Translation = translationText
        };

        _dbContext.TelegramTranslations.Add(oldTranslation);
        _dbContext.TelegramTranslations.Add(latestTranslation);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _translationManager.GetLatest(username, chatId);

        // Assert
        Assert.Equal(latestTranslation.Id, result?.Id);
    }


    [Fact]
    public async void GetLatest_MultipleTranslationsWithSameCreationTime_ReturnsAny()
    {
        // Arrange
        var username = "testUser";
        var chatId = 12345L;
        var baseText = "Translation base text";
        var translationText = "Translation";

        var createdAtTime = DateTime.UtcNow;

        var translation1 = new TelegramTranslationEntity
        {
            Id = Guid.NewGuid().ToString(),
            UserName = username,
            ChatId = chatId,
            CreatedAt = createdAtTime,
            UpdatedAt = createdAtTime,
            BaseText = baseText,
            Translation = translationText
        };

        var translation2 = new TelegramTranslationEntity
        {
            Id = Guid.NewGuid().ToString(),
            UserName = username,
            ChatId = chatId,
            CreatedAt = createdAtTime,
            UpdatedAt = createdAtTime,
            BaseText = baseText,
            Translation = translationText
        };

        _dbContext.TelegramTranslations.Add(translation1);
        _dbContext.TelegramTranslations.Add(translation2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _translationManager.GetLatest(username, chatId);

        // Assert
        Assert.True(result?.Id == translation1.Id || result?.Id == translation2.Id);
    }


    [Fact]
    public async void GetLatest_MultipleUsersAndChats_ReturnsCorrectOne()
    {
        // Arrange
        var username1 = "testUser1";
        var chatId1 = 12345L;
        var username2 = "testUser2";
        var chatId2 = 67890L;
        var baseText = "Translation base text";
        var translationText = "Translation";

        var translationForUser1 = new TelegramTranslationEntity
        {
            Id = Guid.NewGuid().ToString(),
            UserName = username1,
            ChatId = chatId1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BaseText = baseText,
            Translation = translationText
        };

        var translationForUser2 = new TelegramTranslationEntity
        {
            Id = Guid.NewGuid().ToString(),
            UserName = username2,
            ChatId = chatId2,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BaseText = baseText,
            Translation = translationText
        };

        _dbContext.TelegramTranslations.Add(translationForUser1);
        _dbContext.TelegramTranslations.Add(translationForUser2);
        await _dbContext.SaveChangesAsync();

        // Act
        var resultForUser1 = await _translationManager.GetLatest(username1, chatId1);
        var resultForUser2 = await _translationManager.GetLatest(username2, chatId2);

        // Assert
        Assert.Equal(translationForUser1.Id, resultForUser1?.Id);
        Assert.Equal(translationForUser2.Id, resultForUser2?.Id);
    }


    [Fact]
    public async void DetermineState_WaitingForStyle_ReturnsCorrectly()
    {
        // Arrange
        var translation = new TelegramTranslationEntity
        {
            UpdatedAt = DateTime.UtcNow.AddSeconds(5),
            Id = Guid.NewGuid().ToString(),
            UserName = "test",
            LanguageFrom = Language.English,
            LanguageTo = Language.Russian
        };
        var expected = TelegramTranslationState.WaitingForStyle;

        // Act
        var result = _translationManager.DetermineState(translation);

        // Assert
        Assert.Equal(expected, result);
    }


    [Fact]
    public async void DetermineState_UnknownContext_WaitingForContext()
    {
        // Arrange
        var context = new Context
        {
            Question = "What is the meaning of life?"
        };
        var translation = new TelegramTranslationEntity
        {
            UpdatedAt = DateTime.UtcNow.AddSeconds(5),
            Id = Guid.NewGuid().ToString(),
            UserName = "test",
            LanguageFrom = Language.English,
            LanguageTo = Language.Russian,
            Contexts = new[] { context }
        };
        var expected = TelegramTranslationState.WaitingForContext;

        // Act
        var result = _translationManager.DetermineState(translation);

        // Assert
        Assert.Equal(expected, result);
    }


    [Fact]
    public async void DetermineState_UnknownStyle_WaitingForStyle()
    {
        // Arrange
        var context = new Context
        {
            Question = "What is the meaning of life?",
            Response = "42"
        };
        var translation = new TelegramTranslationEntity
        {
            UpdatedAt = DateTime.UtcNow.AddSeconds(5),
            Id = Guid.NewGuid().ToString(),
            UserName = "test",
            LanguageFrom = Language.English,
            LanguageTo = Language.Russian,
            Contexts = new[] { context }
        };
        var expected = TelegramTranslationState.WaitingForStyle;

        // Act
        var result = _translationManager.DetermineState(translation);

        // Assert
        Assert.Equal(expected, result);
    }

    public void Dispose()
    {
        _dbContext.Database.CloseConnection();
        _dbContext.Dispose();
    }
}

