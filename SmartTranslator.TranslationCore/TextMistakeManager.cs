using Newtonsoft.Json;
using OpenAI.ObjectModels.RequestModels;
using SmartTranslator.TranslationCore.Abstractions;
using SmartTranslator.TranslationCore.Abstractions.Exceptions;
using SmartTranslator.TranslationCore.Enums;

namespace SmartTranslator.TranslationCore;

public class TextMistakeManager : ITextMistakeManager
{
    private readonly GptTranslationOptions _options;
    private readonly IGptHttpClient _gptHttpClient;

    public TextMistakeManager(GptTranslationOptions options, IGptHttpClient httpClient)
    {
        _options = options;
        _gptHttpClient = httpClient;
    }


    /// <inheritdoc/>
    public async Task<string> Correct(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new EmptyTextException();
        }

        if (text.Length > _options.MaxSymbols)
        {
            throw new TextIsTooLongException(_options.MaxSymbols, text.Length);
        }

        var messages = new List<ChatMessage>()
        {
            ChatMessage.FromUser($"Correct all mistakes in this text: {text}. Answer in json format {{\"Text\": \"\"}}.")
        };

        var correctSentence = await _gptHttpClient.Send(messages, GptModel.Gpt4Stable);

        try
        {
            var result = JsonConvert.DeserializeObject<SpellingCorrectorResponse>(correctSentence);
            return result!.Text;
        }
        catch (Exception ex)
        {
            throw new CorrectionErrorException("Failed to correct text.", ex);
        }
    }
}
