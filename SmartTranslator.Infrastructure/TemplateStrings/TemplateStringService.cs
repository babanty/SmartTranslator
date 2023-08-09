namespace SmartTranslator.Infrastructure.TemplateStrings;

/// <inheritdoc/>
internal class TemplateStringService : ITemplateStringService
{
    private readonly TemplateStringDbContext _dbContext;


    /// <inheritdoc/>
    public TemplateStringService(TemplateStringDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<TemplateString>> GetAll() => await _dbContext.ExecuteQuery(_dbContext.GetQuery());


    /// <inheritdoc/>
    public async Task<TemplateString> GetSingle(string name,
                                                TemplateLanguage language = TemplateLanguage.Universal,
                                                EnvironmentType environment = EnvironmentType.Any,
                                                string? tag = null)
    {
        var entities = await GetTemplates(name, language, environment, tag);

        if (entities is null || entities.Count != 1)
        {
            throw new NotImplementedException($"Ожидается, что шаблон в БД только один с такими параметрами: name {name}, language {language} , environment {environment}, {tag}. Найдено: {entities?.Count} шт.");
        }

        return entities.Single();
    }

    /// <inheritdoc/>
    public async Task<TemplateString> GetSingleForLanguageCouple(string name,
                                                                 TemplateLanguage language1,
                                                                 TemplateLanguage language2,
                                                                 EnvironmentType environment = EnvironmentType.Any,
                                                                 string? tag = null)
    {
        var language = GetTemplateLanguageForCouple(language1, language2);

        return await GetSingle(name, language, environment, tag);
    }


    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<TemplateString>> GetTemplates(string name,
                                                                        TemplateLanguage? language = null,
                                                                        EnvironmentType? environment = null,
                                                                        string? tag = null)
    {
        var query = _dbContext.GetQuery();

        query = query.Where(x => x.Name == name);

        if (language is not null)
        {
            query = query.Where(q => q.Language == language);
        }

        if (environment is not null)
        {
            query = query.Where(q => q.Environment == environment);
        }

        if (tag is not null)
        {
            query = query.Where(q => q.Tag == tag);
        }

        return await _dbContext.ExecuteQuery(query);
    }


    private static TemplateLanguage GetTemplateLanguageForCouple(TemplateLanguage lang1, TemplateLanguage lang2)
    {
        if (IsCouple(TemplateLanguage.Rus, TemplateLanguage.Eng, lang1, lang2))
            return TemplateLanguage.Rus_Eng;

        if (IsCouple(TemplateLanguage.Rus, TemplateLanguage.Tur, lang1, lang2))
            return TemplateLanguage.Rus_Tur;

        if (IsCouple(TemplateLanguage.Rus, TemplateLanguage.Esp, lang1, lang2))
            return TemplateLanguage.Rus_Esp;

        if (IsCouple(TemplateLanguage.Rus, TemplateLanguage.Deu, lang1, lang2))
            return TemplateLanguage.Rus_Deu;

        if (IsCouple(TemplateLanguage.Rus, TemplateLanguage.Fra, lang1, lang2))
            return TemplateLanguage.Rus_Fra;

        if (IsCouple(TemplateLanguage.Rus, TemplateLanguage.Gre, lang1, lang2))
            return TemplateLanguage.Rus_Gre;

        if (IsCouple(TemplateLanguage.Rus, TemplateLanguage.Uzb, lang1, lang2))
            return TemplateLanguage.Rus_Uzb;

        throw new NotImplementedException($"Invalid language combination: {lang1} ; {lang2}");
    }


    private static bool IsCouple(TemplateLanguage expectedLang1, TemplateLanguage expectedLang2,
                                 TemplateLanguage factLang1, TemplateLanguage factLang2)
    {
        if (expectedLang1 == factLang1 && expectedLang2 == factLang2)
            return true;

        if (expectedLang1 == factLang2 && expectedLang2 == factLang1)
            return true;

        return false;
    }
}
