using SmartTranslator.Enums;
using Xunit;
using Moq;
using OpenAI.ObjectModels.RequestModels;

namespace SmartTranslator.TranslationCore.Tests;

public class LanguageManagerTest
{
    private readonly IntegrationTestOptions _testOptions;
    private readonly GptHttpClientOptions _httpClientOptions;
    private readonly Mock<IGptHttpClient> _httpClientMock;

    public LanguageManagerTest()
    {
        _testOptions = IntegrationTestOptionsProvider.GetIntegrationTestOptions();

        _httpClientOptions = new GptHttpClientOptions
        {
            ApiKey = _testOptions.ApiKey
        };

        _httpClientMock = new Mock<IGptHttpClient>();
    }


    [Fact]
    public async Task DetermineLanguage_EnglishLanguage_DeterminesCorrectly()
    {
        // Arrange
        var languageOptions = GetLangugeOptions();
        var manager = new LanguageManager(languageOptions, _httpClientMock.Object);
        var text = "Hello world!";

        _httpClientMock.Setup(h => h.Send(It.IsAny<List<ChatMessage>>(), It.IsAny<GptModel>()))
                        .ReturnsAsync("English");

        // Act
        var result = await manager.DetermineLanguage(text);

        // Assert
        Assert.Equal(Language.English, result);
    }


    [Fact]
    public async Task DetermineLanguage_RussianLanguage_DeterminesCorrectly()
    {
        // Arrange
        var languageOptions = GetLangugeOptions();
        var manager = new LanguageManager(languageOptions, _httpClientMock.Object);
        var text = "Привет, мир!";

        _httpClientMock.Setup(h => h.Send(It.IsAny<List<ChatMessage>>(), It.IsAny<GptModel>()))
            .ReturnsAsync("Russian");

        // Act
        var result = await manager.DetermineLanguage(text);

        // Assert
        Assert.Equal(Language.Russian, result);
    }


    [Fact]
    public void GetLanguagePair_ValidInput_ReturnsCorrectPair()
    {
        // Arrange
        var languageOptions = GetLangugeOptions();

        var manager = new LanguageManager(languageOptions, _httpClientMock.Object);

        // Act
        var result = manager.GetLanguagePair();

        // Assert
        Assert.Equal((Language.English, Language.Russian), result);
    }

    private static LanguageOptions GetLangugeOptions() => new LanguageOptions()
    {
        From = Language.English,
        To = Language.Russian
    };
}
