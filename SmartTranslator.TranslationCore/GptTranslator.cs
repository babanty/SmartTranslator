﻿using Newtonsoft.Json;
using OpenAI.ObjectModels.RequestModels;
using SmartTranslator.Enums;
using SmartTranslator.TranslationCore.Exceptions;

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

        var translation = await _gptHttpClient.Send(messages, GptModel.GPT3d5Stable);

        return translation;
    }


    public async Task<string> EvaluateContext(string text, Language to)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "I can't translate empty text.";
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

        var evaluationJson = await _gptHttpClient.Send(messages, GptModel.GPT3d5Stable);
        var result = JsonConvert.DeserializeObject<EvaluationResponse>(evaluationJson);

        var percent = result.Percent;
        var question = result.Request.ClarifyingQuestion;

        var percentAnswer = $"Percent: {percent}.";
        return percent != 0 ? percentAnswer : percentAnswer + " " + question;
    }
}
