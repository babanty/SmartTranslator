using SmartTranslator.Enums;
using Xunit;

namespace SmartTranslator.TranslationCore.Tests;

public class LanguageManagerTest
{
    private readonly IntegrationTestOptions _testOptions;
    private readonly GptHttpClientOptions _httpClientOptions;
    private readonly IGptHttpClient _httpClient;

    public LanguageManagerTest()
    {
        _testOptions = IntegrationTestOptionsProvider.GetIntegrationTestOptions();

        _httpClientOptions = new GptHttpClientOptions
        {
            ApiKey = _testOptions.ApiKey
        };

        _httpClient = new GptHttpClient(_httpClientOptions);
    }


    [Fact]
    public async Task DetermineLanguage_EnglishLanguage_DeterminesCorrectly()
    {
        // Arrange
        var languageOptions = GetLangugeOptions();
        var manager = new LanguageManager(languageOptions, _httpClient);
        var text = "Hello world!";

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
        var manager = new LanguageManager(languageOptions, _httpClient);
        var text = "Привет, мир!";

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

        var manager = new LanguageManager(languageOptions, _httpClient);

        // Act
        var result = manager.GetLanguagePair();

        // Assert
        Assert.Equal((Language.English, Language.Russian), result);
    }

    private LanguageOptions GetLangugeOptions() => new LanguageOptions()
    {
        From = Language.English,
        To = Language.Russian
    };
}
