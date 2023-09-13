using SmartTranslator.Infrastructure.TemplateStrings;

namespace SmartTranslator.Infrastructure.UserLanguages;

public interface IUserLanguageProvider
{
    TemplateLanguage UserLanguage { get; set; }
}