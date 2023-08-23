using SmartTranslator.Infrastructure.TemplateStrings;
using SmartTranslator.TelegramBot.View.Filters.Infrastructure;
using SmartTranslator.TranslationCore.Abstractions.Exceptions;

namespace SmartTranslator.TelegramBot.View.Filters;

public class ArgumentExceptionFilter : DefaultFilter<ArgumentException>
{
    private readonly FilterTools _tools;
    private readonly ITemplateStringService _templateStringService;

    public ArgumentExceptionFilter(FilterTools tools,
                                   ITemplateStringService templateStringService)
    {
        _tools = tools;
        _templateStringService = templateStringService;
    }

    public override async Task<string> Handle(ArgumentException e)
    {
        var message = await _templateStringService.GetSingle("SomethingWentWrongWorkingOnIt");
        return message.Format(new List<KeyAndNewValue>());
    }
}
