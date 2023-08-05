namespace SmartTranslator.Infrastructure.TemplateStrings;

public interface ITemplateStringService
{
    Task<IReadOnlyCollection<TemplateString>> GetAll();


    Task<IReadOnlyCollection<TemplateString>> GetTemplates(string name,
                                                           TemplateLanguage? language = null,
                                                           EnvironmentType? environment = null,
                                                           string? tag = null);


    Task<TemplateString> GetSingle(string name,
                                   TemplateLanguage language = TemplateLanguage.Universal,
                                   EnvironmentType environment = EnvironmentType.Any,
                                   string? tag = null);


    Task<TemplateString> GetSingleForLanguageCouple(string name,
                                                    TemplateLanguage language1,
                                                    TemplateLanguage language2,
                                                    EnvironmentType environment = EnvironmentType.Any,
                                                    string? tag = null);
}
