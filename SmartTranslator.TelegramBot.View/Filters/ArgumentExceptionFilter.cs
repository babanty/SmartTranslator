using SmartTranslator.Infrastructure.TemplateStrings;
using SmartTranslator.Infrastructure.TemplateStringServiceWithUserLanguage;
using SmartTranslator.TelegramBot.View.Filters.Infrastructure;

namespace SmartTranslator.TelegramBot.View.Filters;

public class ArgumentExceptionFilter : DefaultFilter<ArgumentException>
{
    private readonly FilterTools _tools;
    private readonly ITemplateStringServiceWithUserLanguage _templateStringService;

    public ArgumentExceptionFilter(FilterTools tools,
                                   ITemplateStringServiceWithUserLanguage templateStringService)
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
