using SmartTranslator.Enums;
using Xunit;

namespace SmartTranslator.TranslationCore.Tests
{
    public class GptTranslationTest
    {
        private readonly IntegrationTestOptions _testOptions;
        private readonly GptTranslationOptions _translationOptions;
        private readonly GptHttpClientOptions _httpClientOptions;
        private readonly GptHttpClient _httpClient;

        public GptTranslationTest()
        {
            _testOptions = IntegrationTestOptionsProvider.GetIntegrationTestOptions();

            _translationOptions = new GptTranslationOptions
            {
                MaxTokens = _testOptions.MaxTokens
            };

            _httpClientOptions = new GptHttpClientOptions
            {
                ApiKey = _testOptions.ApiKey,
            };

            _httpClient = new GptHttpClient(_httpClientOptions);
        }

        [Fact]
        public async Task Translate_ValidInputWithoutContext_TranslatesCorrectly()
        {
            // Arrange
            var translator = new GptTranslator(_translationOptions, _httpClient);
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
