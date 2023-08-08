using Microsoft.Extensions.Logging;
using SmartTranslator.TelegramBot.View.Filters.Infrastructure;

namespace SmartTranslator.TelegramBot.View.Filters;

/// <summary> Неизвестная ошибка, мы постараемся исправить ее как можно скорее. </summary>
public class ExceptionFilter : Filter
{
    private readonly ILogger<ExceptionFilter> _logger;
    private readonly FilterTools _tools;

    public ExceptionFilter(FilterTools filterTools, ILogger<ExceptionFilter> logger)
    {
        Priority = -1000;

        _tools = filterTools;
        _logger = logger;
    }

    public override bool CanHandle(Exception e) => true;

    public override async Task<string> Handle(Exception e)
    {
        _logger.LogError(new EventId(1000002), e, "Перевод прошел неудачно");

        return await _tools.GetMessageUsingTempalate("UnknownErrorWeWillTryToFixItAsSoonAsPossible");
    }
}
