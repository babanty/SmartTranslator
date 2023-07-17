using OpenAI;
using OpenAI.Interfaces;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using SmartTranslator.Enums;
using SmartTranslator.TranslationCore.Exceptions;

namespace SmartTranslator.TranslationCore;

public class GptTranslator : IGptTranslator
{
    private const int _sendAttemptsCount = 5;
    private readonly GptTranslationOptions _options;
    private readonly IGptHttpClient _gptHttpClient;

    public GptTranslator(GptTranslationOptions options, IGptHttpClient gptHttpClient)
    {
        _options = options;
        _gptHttpClient = gptHttpClient;
    }


    /// <inheritdoc/>
    public async Task<string> Translate(string text, string context, Language from, Language to, TranslationStyle translationStyle)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        if (text.Length > _options.MaxSymbols)
        {
            throw new TextIsTooLongException(_options.MaxSymbols, text.Length);
        }
        var messages = new List<ChatMessage>()
        {
            ChatMessage.FromUser($"Translate this text into {to}: {text}; context:{context}; style: {translationStyle}")
        };

        var translation = await _gptHttpClient.Send(messages, GptModel.GPT3d5Stable, _sendAttemptsCount);

        return translation;
    }
}
