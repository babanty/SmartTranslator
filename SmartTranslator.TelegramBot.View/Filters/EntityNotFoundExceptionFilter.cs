using Microsoft.Extensions.Logging;
using SmartTranslator.TelegramBot.Management.Exceptions;
using SmartTranslator.TelegramBot.View.Filters.Infrastructure;

namespace SmartTranslator.TelegramBot.View.Filters;

public class EntityNotFoundExceptionFilter : DefaultFilter<EntityNotFoundException>
{
    private readonly ILogger<ExceptionFilter> _logger;
    private readonly FilterTools _tools;

    public EntityNotFoundExceptionFilter(FilterTools filterTools, ILogger<ExceptionFilter> logger)
    {
        _tools = filterTools;
        _logger = logger;
    }

    public override bool CanHandle(Exception e) => true;

    public override async Task<string> Handle(EntityNotFoundException e)
    {
        _logger.LogError(new EventId(1000002), e, "Couldn't find entity in db.");

        return await _tools.GetMessageUsingTemplate("EntityNotFoundInDb");
    }
}
