using SmartTranslator.Enums;

namespace SmartTranslator.TranslationCore.Tests
{
    public record IntegrationTestOptions
    {
        public string ApiKey { get; set; } = default!;

        public int MaxTokens { get; set; } = default!;


        public string From { get; set; } = default!;
        public string To { get; set; } = default!;
    }
}