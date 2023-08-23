using SmartTranslator.TelegramBot.View.Filters.Infrastructure;
using SmartTranslator.TranslationCore.Abstractions.Exceptions;

namespace SmartTranslator.TelegramBot.View.Filters;

public class ArgumentExceptionFilter : DefaultFilter<ArgumentException>
{
    private readonly FilterTools _tools;

    public ArgumentExceptionFilter(FilterTools tools)
    {
        _tools = tools;
    }

    public override async Task<string> Handle(ArgumentException e)
    {
        return await Task.FromResult($"Чё-то не так: {e}");
    }
}
