using System.Text.Json;

namespace SmartTranslator.TranslationCore.Tests;

public static class IntegrationTestOptionsProvider
{
    public static IntegrationTestOptions GetIntegrationTestOptions()
    {
        var optionsJson = File.ReadAllText("optionsConfig.json");
        IntegrationTestOptions options = JsonSerializer.Deserialize<IntegrationTestOptions>(optionsJson) ?? throw new ArgumentException("Options json wasn't found or failed to deseriaze");
        return options;
    }
}
