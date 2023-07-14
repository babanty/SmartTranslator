using Xunit;
using SmartTranslator.Enums;

namespace SmartTranslator.TranslationCore.Tests
{
    public class GptTranslationTest
    {
        private readonly IntegrationTestOptions _testOptions;
        private readonly GptTranslationOptions _options;

        public GptTranslationTest()
        {
            _testOptions = IntegrationTestOptionsProvider.GetIntegrationTestOptions();
            _options = new GptTranslationOptions
            {
                ApiKey = _testOptions.ApiKey,
                MaxTokens = _testOptions.MaxTokens
            };
        }

        [Fact]
        public async Task Translate_ValidInputWithoutContext_TranslatesCorrectly()
        {
            // Arrange
            GptTranslator translator = new (_options);
            string text = "Hello world!";
            string context = "";
            Language from = Language.English;
            Language to = Language.Russian;
            TranslationStyle translationStyle = TranslationStyle.СonversationalStyle;

            // Act
            var result = await translator.Translate(text, context, from, to, translationStyle);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Привет, мир!", result);
        }
    }
}
