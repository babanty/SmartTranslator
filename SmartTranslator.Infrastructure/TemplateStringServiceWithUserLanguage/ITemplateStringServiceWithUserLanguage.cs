using SmartTranslator.Infrastructure.TemplateStrings;

namespace SmartTranslator.Infrastructure.TemplateStringServiceWithUserLanguage;

public interface ITemplateStringServiceWithUserLanguage
{
    Task<TemplateString> GetSingle(string name,
                               EnvironmentType environment = EnvironmentType.Any,
                               string? tag = null);
}
