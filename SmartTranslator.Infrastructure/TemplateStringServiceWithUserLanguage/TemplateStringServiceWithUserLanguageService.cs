using SmartTranslator.Infrastructure.TemplateStrings;
using SmartTranslator.Infrastructure.UserLanguages;

namespace SmartTranslator.Infrastructure.TemplateStringServiceWithUserLanguage;

public class TemplateStringServiceWithUserLanguageService : ITemplateStringServiceWithUserLanguage
{
    private readonly ITemplateStringService _templateStringService;
    private readonly IUserLanguageProvider _userLanguageProvider;

    public TemplateStringServiceWithUserLanguageService(ITemplateStringService templateStringService, IUserLanguageProvider userLanguageProvider)
    {
        _templateStringService = templateStringService;
        _userLanguageProvider = userLanguageProvider;
    }


    public Task<TemplateString> GetSingle(string name, EnvironmentType environment = EnvironmentType.Any, string? tag = null)
    {
        return _templateStringService.GetSingle(name, _userLanguageProvider.UserLanguage, environment, tag);
    }
}