using SmartTranslator.TelegramBot.Management.GptTelegramBots;

namespace SmartTranslator.TelegramBot.View;

public class LoadingAnimation : ILoadingAnimation
{
    public async Task<CancellationTokenSource> ActivateLoadingAnimation(ITelegramBotMessageSender messageSender, long? chatId)
    {
        // NOTE: the loading animation consists of just dots, for example "....", and each second the number of dots increases by one

        CancellationTokenSource cts = new();

        if (chatId is null)
        {
            return cts;
        }

        string loadingMessage = "\\.\\.";
        var loadingMessageId = (await messageSender.SendOrUpdateMessage(loadingMessage, chatId.Value)).MessageId;

        _ = Task.Run(async () =>
        {
            try
            {
                var dotsCounter = 2;
                var maxDots = 15;
                while (!cts.Token.IsCancellationRequested && dotsCounter < maxDots)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cts.Token);

                    loadingMessage += "\\.";
                    dotsCounter++;

                    await messageSender.SendOrUpdateMessage(loadingMessage, chatId.Value, loadingMessageId);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception)
            {
            }

            try
            {
                await messageSender.DeleteMessage(chatId.Value, loadingMessageId);
            }
            catch
            {
            }

        }, cts.Token);

        return cts;
    }

    public void DeactivateLoadingAnimation(CancellationTokenSource cancellationTokenSource)
    {
        cancellationTokenSource.Cancel();
    }
}
