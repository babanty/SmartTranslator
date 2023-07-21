using SmartTranslator.Enums;
using Xunit;

namespace SmartTranslator.TranslationCore.Tests;

public class LanguageManagerIntegrationTest
{
    private readonly IntegrationTestOptions _testOptions;

    public LanguageManagerIntegrationTest()
    {
        _testOptions = IntegrationTestOptionsProvider.GetIntegrationTestOptions();
    }


    [Fact]
    public async Task DetermineLanguage_EnglishLanguage_DeterminesCorrectly()
    {
        // Arrange
        var clientOptions = new GptHttpClientOptions
        {
            ApiKey = _testOptions.ApiKey,
        };
        var httpClient = new GptHttpClient(clientOptions);
        var languageOptions = GetLangugeOptions();
        var manager = new LanguageManager(languageOptions, httpClient);
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
        var clientOptions = new GptHttpClientOptions
        {
            ApiKey = _testOptions.ApiKey,
        };
        var httpClient = new GptHttpClient(clientOptions);
        var languageOptions = GetLangugeOptions();
        var manager = new LanguageManager(languageOptions, httpClient);
        var text = "Привет, мир!";

        // Act
        var result = await manager.DetermineLanguage(text);

        // Assert
        Assert.Equal(Language.Russian, result);
    }


    private static LanguageOptions GetLangugeOptions() => new LanguageOptions()
    {
        From = Language.English,
        To = Language.Russian
    };
}
