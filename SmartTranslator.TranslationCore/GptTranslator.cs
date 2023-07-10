using OpenAI;
using OpenAI.Interfaces;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using SmartTranslator.Enums;
using SmartTranslator.TranslationCore;
using SmartTranslator.TranslationCore.Exceptions;

namespace ChatGptTranslator.Management.GptTranslation;

public class GptTranslator : IGptTranslator
{
    private const int _sendAttemptsCount = 5;

    private readonly GptTranslationOptions _options;
    private readonly IChatCompletionService _textChatGpt;

    public GptTranslator(GptTranslationOptions options)
    {
        _options = options;

        _textChatGpt = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = _options.ApiKey,
        });
    }


    /// <inheritdoc/>
    public async Task<string> Translate(string text, string context, Language from, Language to, TranslationStyle translationStyle)
    {
        var messages = new List<ChatMessage>()
        {
            ChatMessage.FromUser($"Translate this text into {to}: {text}; context:{context}; style: {translationStyle}")
        };

        var translation = await SendMessages(text, messages);

        return translation;
    }
 

    private async Task<string> SendMessages(string text, List<ChatMessage> messages)
    {
        for (int i = 0; i < _sendAttemptsCount; i++)
        {
            try
            {
                return await SendMessagesAttempt(text, messages);
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


    private async Task<string> SendMessagesAttempt(string text, List<ChatMessage> messages)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        if (text.Length > _options.MaxSymbols)
        {
            throw new TextIsTooLongException(_options.MaxSymbols, text.Length);
        }

        var completionResult = await _textChatGpt.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = messages,
            Model = Models.Gpt_4,
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

            throw new FailedToTranslateException(completionResult?.Error?.Message ?? "Ошибка не известна");

        }

        return completionResult.Choices.FirstOrDefault()?.Message?.Content ?? "Ответа нет";
    }
}
