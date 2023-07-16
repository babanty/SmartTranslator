using OpenAI.Interfaces;
using OpenAI.Managers;
using OpenAI;
using OpenAI.ObjectModels.RequestModels;
using SmartTranslator.Enums;
using SmartTranslator.TranslationCore.Exceptions;

namespace SmartTranslator.TranslationCore;

public class GptHttpClient : IGptHttpClient
{
    private readonly GptTranslationOptions _options;
    private readonly IChatCompletionService _textChatGpt;

    public GptHttpClient(GptTranslationOptions options)
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
        var messageContent = messages.FirstOrDefault()?.Content;

        if (string.IsNullOrWhiteSpace(messageContent))
        {
            return string.Empty;
        }

        if (messageContent.Length > _options.MaxSymbols)
        {
            throw new TextIsTooLongException(_options.MaxSymbols, messageContent.Length);
        }

        var completionResult = await _textChatGpt.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = messages,
            Model = gptModelToString(model),
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

    private string gptModelToString(GptModel gptModel)
    {
        switch (gptModel)
        {
            case GptModel.Gpt4StableLong:
                return "gpt-4-32k-0613";
            case GptModel.Gpt4Stable:
                return "gpt-4-0613";
            case GptModel.GPT3d5Stable:
                return "gpt-3.5-turbo-0613";
            case GptModel.GPT3d5StableLong:
                return "gpt-3.5-turbo-16k-0613";
            default:
                throw new UnknownModelException($"Unknown model: {gptModel}");
        }
    }
        
}
