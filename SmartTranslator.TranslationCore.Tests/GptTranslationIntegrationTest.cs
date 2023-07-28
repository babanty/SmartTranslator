using SmartTranslator.TranslationCore.Abstractions.Models;
using SmartTranslator.TranslationCore.Enums;
using Xunit;

namespace SmartTranslator.TranslationCore.Tests;

public class GptTranslationIntegrationTest
{
    private readonly IntegrationTestOptions _testOptions;

    public GptTranslationIntegrationTest()
    {
        _testOptions = IntegrationTestOptionsProvider.GetIntegrationTestOptions();
    }


    [Fact]
    public async Task Translate_ValidInputWithoutContext_TranslatesCorrectly()
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
        var translator = new GptTranslator(translationOptions, httpClient);
        var text = "Hello world!";
        var context = "";
        var from = Language.English;
        var to = Language.Russian;
        var translationStyle = TranslationStyle.ConversationalStyle;

        // Act
        var result = await translator.Translate(text, context, from, to, translationStyle);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Привет, мир!", result);
    }


    [Fact]
    public async Task EvaluateContext_UnequivocalInput_AnswerOK()
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
        var translator = new GptTranslator(translationOptions, httpClient);
        var text = "Hello world!";
        var to = Language.Russian;

        // Act
        var result = await translator.EvaluateContext(text, to);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1f, result.Percent);
    }


    [Fact]
    public async Task EvaluateContext_EquivocalInput_AnswerOK()
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
        var translator = new GptTranslator(translationOptions, httpClient);
        var text = "She was struck by the book.";
        var to = Language.Russian;

        // Act
        var result = await translator.EvaluateContext(text, to);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0f, result.Percent);
        Assert.NotNull(result.Request);
    }


    [Fact]
    public async Task DefineStyle_ValidInput_AnswerOK()
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
        var translator = new GptTranslator(translationOptions, httpClient);
        var text = "She was struck by the book.";
        var context = "";
        var from = Language.English;
        var to = Language.Russian;              


        //Act
        var result = await translator.DefineStyle(text, context, from, to);


        // Assert
        float GetProbability(TranslationStyle style) =>
            result.ProbabilityOfSuccess.Where(prob => prob.Style == style).First().Probability;

        var officialProb = GetProbability(TranslationStyle.OfficialStyle);
        var conversationalProb = GetProbability(TranslationStyle.ConversationalStyle);
        var teenageProb = GetProbability(TranslationStyle.TeenageStyle);
        
        Assert.NotNull(result);
        
        Assert.InRange(officialProb, 0.7f, 0.9f); // TODO: change to more appropriate values once GPT-4 is accessible
        Assert.InRange(conversationalProb, 0.1f, 0.3f); // TODO: change to more appropriate values once GPT-4 is accessible
        Assert.InRange(teenageProb, 0f, 0.2f); // TODO: change to more appropriate values once GPT-4 is accessible
    }
}
