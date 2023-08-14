using Microsoft.EntityFrameworkCore;
using SmartTranslator.DataAccess;
using SmartTranslator.DataAccess.Entities;
using SmartTranslator.TelegramBot.Management.TranslationManagement;
using Xunit;

namespace SmartTranslator.Tests;

public class TranslationManagerTests : IDisposable
{
    private readonly TelegramTranslationDbContext _dbContext;
    private readonly TranslationManager _translationManager;

    public TranslationManagerTests()
    {
        var options = new DbContextOptionsBuilder<TelegramTranslationDbContext>()
            .UseSqlite("Filename=:memory:")
            .Options;

        _dbContext = new TelegramTranslationDbContext(options);
        _dbContext.Database.OpenConnection();
        _dbContext.Database.EnsureCreated();

        _translationManager = new TranslationManager(_dbContext);
    }

    [Fact]
    public async void GetLatest_ReturnsNullIfNoTranslationExists()
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
    public async void GetLatest_ReturnsTranslation()
    {
        // Arrange
        var username = "testUser";
        var chatId = 12345L;
        var baseText = "Translation base text";
        var translationText = "Translation";

        var translation = new TelegramTranslationEntity
        {
            Id = TelegramTranslationEntity.GetId(chatId, username + "new"),
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
        Assert.Equal(translation.Id, result.Id);
    }


    [Fact]
    public async void GetLatest_ReturnsLatestTranslation()
    {
        // Arrange
        var username = "testUser";
        var chatId = 12345L;
        var oldBaseText = "Old translation base text";
        var newBaseText = "New translation base text";
        var translationText = "Translation";

        var oldTranslation = new TelegramTranslationEntity
        {
            Id = TelegramTranslationEntity.GetId(chatId, username),
            UserName = username,
            ChatId = chatId,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            BaseText = oldBaseText,
            Translation = translationText
        };

        var latestTranslation = new TelegramTranslationEntity
        {
            Id = TelegramTranslationEntity.GetId(chatId, username + "new"),
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
        Assert.Equal(latestTranslation.Id, result.Id);
    }


    [Fact]
    public async void GetLatest_ReturnsOneOfTranslationsWithSameCreationTime()
    {
        // Arrange
        var username = "testUser";
        var chatId = 12345L;
        var baseText = "Translation base text";
        var translationText = "Translation";

        var createdAtTime = DateTime.UtcNow;

        var translation1 = new TelegramTranslationEntity
        {
            Id = TelegramTranslationEntity.GetId(chatId, username + "1"),
            UserName = username,
            ChatId = chatId,
            CreatedAt = createdAtTime,
            UpdatedAt = createdAtTime,
            BaseText = baseText,
            Translation = translationText
        };

        var translation2 = new TelegramTranslationEntity
        {
            Id = TelegramTranslationEntity.GetId(chatId, username + "2"),
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
        Assert.True(result.Id == translation1.Id || result.Id == translation2.Id);
    }


    [Fact]
    public async void GetLatest_ReturnsTranslationOnlyForSpecificUserAndChat()
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
            Id = TelegramTranslationEntity.GetId(chatId1, username1),
            UserName = username1,
            ChatId = chatId1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BaseText = baseText,
            Translation = translationText
        };

        var translationForUser2 = new TelegramTranslationEntity
        {
            Id = TelegramTranslationEntity.GetId(chatId2, username2),
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

        // Assert
        Assert.Equal(translationForUser1.Id, resultForUser1.Id);

        // Act
        var resultForUser2 = await _translationManager.GetLatest(username2, chatId2);

        // Assert
        Assert.Equal(translationForUser2.Id, resultForUser2.Id);
    }


    public void Dispose()
    {
        _dbContext.Database.CloseConnection();
        _dbContext.Dispose();
    }
}

