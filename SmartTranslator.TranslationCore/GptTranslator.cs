using Newtonsoft.Json;
using OpenAI.ObjectModels.RequestModels;
using SmartTranslator.TranslationCore.Abstractions;
using SmartTranslator.TranslationCore.Abstractions.Exceptions;
using SmartTranslator.TranslationCore.Abstractions.Models;
using SmartTranslator.TranslationCore.Enums;

namespace SmartTranslator.TranslationCore;

public class GptTranslator : IGptTranslator
{
    private readonly GptTranslationOptions _options;
    private readonly IGptHttpClient _gptHttpClient;

    public GptTranslator(GptTranslationOptions options, IGptHttpClient gptHttpClient)
    {
        _options = options;
        _gptHttpClient = gptHttpClient;
    }


    /// <inheritdoc/>
    public async Task<string> Translate(string text, string? context, Language from, Language to, TranslationStyle translationStyle)
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

        var translation = await _gptHttpClient.Send(messages, GptModel.Gpt4Stable);

        return translation;
    }


    /// <inheritdoc/>
    public async Task<EvaluationResponse> EvaluateContext(string text, Language to)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new EmptyTextException();
        }

        if (text.Length > _options.MaxSymbols)
        {
            throw new TextIsTooLongException(_options.MaxSymbols, text.Length);
        }

        var prompt = $@"Do you have enough context to unequivocally translate the following text into {to}: ""{text}"". Answer in JSON format: {{""percent"": 0}}, where 0 means not enough context and clarification is needed, and 1 means enough context for unequivocal translation. If percent=0, request context in JSON format: {{
""request"": {{
""clarifyingQuestion"": """"
}}
}}
where clarifyingQuestion is the field where you need to enter a clarifying question.";

        var messages = new List<ChatMessage>()
        {
            ChatMessage.FromUser(prompt)
        };

        var evaluationJson = await _gptHttpClient.Send(messages, GptModel.Gpt4Stable);
        try
        {
            var result = JsonConvert.DeserializeObject<EvaluationResponse>(evaluationJson);
            return result!;
        }
        catch (Exception ex)
        {
            throw new ContextEvaluationErrorException("Failed to evaluate context.", ex);
        }
    }


    /// <inheritdoc/>
    public async Task<StyleDefinitionResult> DefineStyle(string text, string? context, Language? from, Language? to)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new EmptyTextException();
        }

        if (text.Length > _options.MaxSymbols)
        {
            throw new TextIsTooLongException(_options.MaxSymbols, text.Length);
        }

        var styles = GetTranslationStyles();

        var prompt = $@"There are {styles.Count()} writing styles: {string.Join(", ", styles)}. 
Determine which one the sentence '{text}' corresponds to in the format JSON: 
{{
  ""ProbabilityOfSuccess"": [
    {{
      ""Probability"": 
      ""Style"": 
    }}
    }}
  ]
}} , where percent can be from 0 to 1, where 0 is absolutely not matching, and 1 is a complete match.
Give probabilities of all said styles.";

        var messages = new List<ChatMessage>()
        {
            ChatMessage.FromUser(prompt)
        };

        var evaluationJson = await _gptHttpClient.Send(messages, GptModel.Gpt4Stable);
        try
        {
            var result = JsonConvert.DeserializeObject<StyleDefinitionResult>(evaluationJson);
            return result!;
        }
        catch (Exception ex)
        {
            throw new StyleDefinitionErrorException("Failed to define text style.", ex);
        }
    }


    private static List<string> GetTranslationStyles()
    {
        var styles = Enum.GetNames(typeof(TranslationStyle)).Where(name => name != "Unknown").ToList();
        return styles;
    }
}
