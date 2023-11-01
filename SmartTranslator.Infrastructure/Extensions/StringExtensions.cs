using SmartTranslator.Infrastructure.Exceptions;
using System.Text.RegularExpressions;

namespace SmartTranslator.Infrastructure.Extensions;

public static class StringExtensions
{
    /// <returns>JSON without extra text, <see cref="JsonNotFoundException"/> if no JSON found. </returns>
    public static string ExtractJson(this string input)
    {
        // Regular expression that looks for JSONs
        var jsonPattern = @"(\{(?:[^{}]|(?<Open>\{)|(?<-Open>\}))+(?(Open)(?!))\})|(\[(?:[^\[\]]|(?<Open>\[)|(?<-Open>\]))+(?(Open)(?!))\])";

        MatchCollection matches = Regex.Matches(input, jsonPattern);

        if (matches.Count == 0)
            throw new JsonNotFoundException($"No JSON found in the following text: {input}");

        return matches[0].Value;
    }
}