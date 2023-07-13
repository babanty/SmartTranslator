using Xunit;
using SmartTranslator.Enums;

namespace SmartTranslator.TranslationCore.Tests
{
    public class GptTranslationTest
    {
        private readonly IntegrationTestOptions _testOptions = new IntegrationTestOptions();
        private readonly GptTranslationOptions _options = new GptTranslationOptions();

        public GptTranslationTest()
        {
            _testOptions = IntegrationTestOptionsProvider.GetIntegrationTestOptions();
            _options.ApiKey = _testOptions.ApiKey;
            _options.MaxTokens = _testOptions.MaxTokens;
        }

        [Fact]
        public void Translate_ValidInputWithoutContext_TranslatesCorrectly()
        {
            // Arrange
            GptTranslator translator = new (_options);
            string text = "Hello world!";

            // Act
            var asyncResult = translator.Translate(text, "", Language.English, Language.Russian, TranslationStyle.СonversationalStyle);
            string result = asyncResult.Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Привет, мир!", result);
        }
    }
}
