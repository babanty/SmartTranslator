using Newtonsoft.Json;
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
    private readonly TranslationResultProvider _translationResultProvider;

    public GptTranslator(GptTranslationOptions options,
                         TranslationResultProvider translationResultHolder)
    {
        _options = options;

        _textChatGpt = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = _options.ApiKey,
        });
        _translationResultProvider = translationResultHolder;
    }


    /// <inheritdoc/>
    public async Task<TranslationResult> Translate(string text, Language from, Language to, TranslationStyle translationStyle)
    {
        var messages = new List<ChatMessage>()
        {
            ChatMessage.FromSystem($"You are a translator from {from} to {to}."),
            ChatMessage.FromUser($"Translate into {to} using {translationStyle}: {text}")
        };

        var translation = await SendMessages(text, messages);

        var result = new TranslationResult()
        {
            LanguageTo = to,
            Translation = translation
        };

        _translationResultProvider.Result = result;
        return result;
    }


    public async Task<TranslationResult> Translate(string text, (Language, Language) couple, TranslationStyle translationStyle)
    {
        // первая попытка
        try
        {
            var messages = new List<ChatMessage>()
            {
                ChatMessage.FromSystem($@"You are a {couple.Item1}-{couple.Item2} translator."),
                ChatMessage.FromUser($"JSON format: {{ \"Translation\": \"\", \"LanguageTo\": \"\" }}  Where the variable LanguageTo can only be {couple.Item1} or {couple.Item2}. Translate into JSON using {translationStyle} this text: '{text}'")
            };

            return await Translate(text, messages);
        }
        // вторая попытка
        catch
        {
            var messages = new List<ChatMessage>()
            {
                ChatMessage.FromSystem($@"You are a {couple.Item1}-{couple.Item2} translator. Answer in JSON format: {{ ""Translation"": """", ""LanguageTo"": 0 }}  Where the variable LanguageTo can only be {(int)couple.Item1} - {couple.Item1}, {(int)couple.Item2} - {couple.Item2}."),
                ChatMessage.FromUser($"Translate using {translationStyle}: {text}")
            };

            return await Translate(text, messages);
        }
    }

    private async Task<TranslationResult> Translate(string text, List<ChatMessage> messages)
    {
        var responseJson = await SendMessages(text, messages);

        var result = JsonConvert.DeserializeObject<TranslationResult>(responseJson);
        _translationResultProvider.Result = result;

        return result;
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
