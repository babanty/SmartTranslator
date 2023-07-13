using Xunit;
using System.Text.Json;
using SmartTranslator.Enums;

namespace SmartTranslator.TranslationCore.Tests
{
    public class GptTranslationTest
    {
        [Fact]
        public void TranslatesCorrectly()
        {
            // Arrange
            var optionsJson = File.ReadAllText("optionsConfig.json");
            GptTranslationOptions options = JsonSerializer.Deserialize<GptTranslationOptions>(optionsJson) ?? throw new ArgumentException("Options json wasn't found or failed to deseriaze");
            GptTranslator translator = new (options);
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
