using SmartTranslator.Infrastructure.TemplateStrings;

namespace SmartTranslator.Infrastructure.UserLanguages;

public class UserLanguageProviderStub : IUserLanguageProvider
{
    public TemplateLanguage UserLanguage { get; set; } = TemplateLanguage.Rus_Eng;
}