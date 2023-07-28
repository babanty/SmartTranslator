using Xunit;

namespace SmartTranslator.TranslationCore.Tests;

public class TextMistakeManagerTest
{
    private readonly IntegrationTestOptions _testOptions;

    public TextMistakeManagerTest()
    {
        _testOptions = IntegrationTestOptionsProvider.GetIntegrationTestOptions();
    }


    [Fact]
    public async Task Correct_ValidIncorrectInput_AnswerOK()
    {
        // Arrange
        var translationOptions = new GptTranslationOptions
        {
            MaxTokens = _testOptions.MaxTokens
        };
        var httpClientOptions = new GptHttpClientOptions
        {
            ApiKey = _testOptions.ApiKey
        };
        var httpClient = new GptHttpClient(httpClientOptions);
        var corrector = new TextMistakeManager(translationOptions, httpClient);
        var text = "I have gone there yesterday.";
        var expected = "I went there yesterday.";
        var result = await corrector.Correct(text);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }
}
