using Newtonsoft.Json;
using OpenAI.ObjectModels.RequestModels;
using SmartTranslator.Enums;
using SmartTranslator.TranslationCore.Exceptions;

namespace SmartTranslator.TranslationCore;

public class SpellingManager : ISpellingManager
{
    private readonly GptTranslationOptions _options;
    private readonly IGptHttpClient _gptHttpClient;

    public SpellingManager(GptTranslationOptions options, IGptHttpClient httpClient)
    {
        _options = options;
        _gptHttpClient = httpClient;
    }


    public async Task<string> CorrectSpelling(string text)
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
        var result = JsonConvert.DeserializeObject<SpellingCorrectorResponse>(correctSentence);

        return result.Text;
    }
}
