using AutoMapper;
using SmartTranslator.Infrastructure.Extensions;
using SmartTranslator.Infrastructure.TemplateStrings;
using SmartTranslator.TranslationCore;

namespace SmartTranslator.TelegramBot.View.Filters.Infrastructure;

public class FilterTools
{
    private readonly IMapper _mapper;
    private readonly GptTranslationOptions _gptTranslationOptions;
    private readonly ITemplateStringService _templateStringService;


    public FilterTools(GptTranslationOptions gptTranslationOptions,
                       IMapper mapper,
                       ITemplateStringService templateStringService)
    {
        _gptTranslationOptions = gptTranslationOptions ?? new();
        _mapper = mapper;
        _templateStringService = templateStringService;
    }


    /// <returns> A message in two languages, for example: "My error {New line} Моя ошибка" </returns>
    public async Task<string> GetMessageUsingTemplate(string templateName, IEnumerable<KeyAndNewValue>? templateArgs = null)
    {
        var templates = await GetTemplates(templateName);

        templateArgs ??= new List<KeyAndNewValue>();

        var message = templates.First().Format(templateArgs) + Environment.NewLine + templates.Second()?.Format(templateArgs);

        return message;
    }


    public async Task<TemplateString[]> GetTemplates(string templateName)
    {
        var result = new TemplateString[2];

        var coupleLang = _gptTranslationOptions.CoupleLanguage;

        result[0] = await _templateStringService.GetSingle(templateName, _mapper.Map<TemplateLanguage>(coupleLang.Item1));
        result[1] = await _templateStringService.GetSingle(templateName, _mapper.Map<TemplateLanguage>(coupleLang.Item2));

        return result;
    }
}
