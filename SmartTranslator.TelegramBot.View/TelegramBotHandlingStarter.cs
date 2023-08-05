using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SmartTranslator.TelegramBot.View;

public class TelegramBotHandlingStarter : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;


    public TelegramBotHandlingStarter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }


    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var gptTelegramBotBuilder = scope.ServiceProvider.GetRequiredService<IGptTelegramBotBuilder>();
        await gptTelegramBotBuilder.Build();

        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(1000, ct);
        }
    }
}
