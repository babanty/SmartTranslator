using Xunit;
using SmartTranslator.Enums;

namespace SmartTranslator.TranslationCore.Tests
{
    public class GptTranslationTest
    {
        private readonly IntegrationTestOptions _testOptions;
        private readonly GptTranslationOptions _options;
        private readonly GptHttpClient _httpClient;

        public GptTranslationTest()
        {
            _testOptions = IntegrationTestOptionsProvider.GetIntegrationTestOptions();
            _options = new GptTranslationOptions
            {
                ApiKey = _testOptions.ApiKey,
                MaxTokens = _testOptions.MaxTokens
            };
            _httpClient = new GptHttpClient(_options);
        }

        [Fact]
        public async Task Translate_ValidInputWithoutContext_TranslatesCorrectly()
        {
            // Arrange
            var translator = new GptTranslator(_options, _httpClient);
            var text = "Hello world!";
            var context = "";
            var from = Language.English;
            var to = Language.Russian;
            var translationStyle = TranslationStyle.СonversationalStyle;

            // Act
            var result = await translator.Translate(text, context, from, to, translationStyle);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Привет, мир!", result);
        }
    }
}
