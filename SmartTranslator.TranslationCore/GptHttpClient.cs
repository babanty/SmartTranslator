using OpenAI;
using OpenAI.Interfaces;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using SmartTranslator.Enums;
using SmartTranslator.TranslationCore.Exceptions;

namespace SmartTranslator.TranslationCore;

public class GptHttpClient : IGptHttpClient
{
    private readonly GptHttpClientOptions _options;
    private readonly IChatCompletionService _textChatGpt;

    public GptHttpClient(GptHttpClientOptions options)
    {
        _options = options;

        _textChatGpt = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = _options.ApiKey,
        });
    }


    public async Task<string> Send(List<ChatMessage> messages, GptModel model, int attemptCount)
    {
        for (int i = 0; i < attemptCount; i++)
        {
            try
            {
                return await SendMessagesAttempt(messages, model);
            }
            catch (GptOverloadedException)
            {
            }
            catch (TaskCanceledException)
            {
            }
        }

        throw new GptOverloadedException();
    }

    private async Task<string> SendMessagesAttempt(List<ChatMessage> messages, GptModel model)
    {
        var completionResult = await _textChatGpt.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = messages,
            Model = GptModelToString(model),
            Temperature = 0.0f
        });

        if (!completionResult.Successful)
        {
            if (completionResult?.Error?.Message?.ToLower()?.Contains("rate limit") ?? false)
            {
                throw new RateLimitException();
            }

            if (completionResult?.Error?.Message?.ToLower()?.Contains("overloaded") ?? false) // "That model is currently overloaded with other requ..."
            {
                throw new GptOverloadedException();
            }

            throw new FailedToTranslateException(completionResult?.Error?.Message ?? "Unknown error");

        }

        return completionResult.Choices.FirstOrDefault()?.Message?.Content ?? "No answer";
    }

    private static string GptModelToString(GptModel gptModel)
    {
        return gptModel switch
        {
            GptModel.Gpt4StableLong => "gpt-4-32k-0613",
            GptModel.Gpt4Stable => "gpt-4-0613",
            GptModel.GPT3d5Stable => "gpt-3.5-turbo-0613",
            GptModel.GPT3d5StableLong => "gpt-3.5-turbo-16k-0613",
            _ => throw new UnknownModelException($"Unknown model: {gptModel}"),
        };
    }
}
