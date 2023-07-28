using Moq;
using OpenAI.ObjectModels.RequestModels;
using SmartTranslator.TranslationCore.Enums;
using Xunit;

namespace SmartTranslator.TranslationCore.Tests;

public class GptTranslationTest
{
    private readonly IntegrationTestOptions _testOptions;
    private readonly GptTranslationOptions _translationOptions;
    private readonly Mock<IGptHttpClient> _httpClientMock;

    public GptTranslationTest()
    {
        _testOptions = IntegrationTestOptionsProvider.GetIntegrationTestOptions();

        _translationOptions = new GptTranslationOptions
        {
            MaxTokens = _testOptions.MaxTokens
        };

        _httpClientMock = new Mock<IGptHttpClient>();
    }

    [Fact]
    public async Task Translate_ValidInputWithoutContext_TranslatesCorrectly()
    {
        // Arrange
        var translator = new GptTranslator(_translationOptions, _httpClientMock.Object);
        var text = "Hello world!";
        var context = "";
        var from = Language.English;
        var to = Language.Russian;
        var translationStyle = TranslationStyle.ConversationalStyle;

        _httpClientMock.Setup(h => h.Send(It.IsAny<List<ChatMessage>>(), It.IsAny<GptModel>()))
    .ReturnsAsync("Привет, мир!");

        // Act
        var result = await translator.Translate(text, context, from, to, translationStyle);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Привет, мир!", result);
    }
}
