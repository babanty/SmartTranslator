using OpenAI.ObjectModels.RequestModels;
using SmartTranslator.TranslationCore.Enums;

namespace SmartTranslator.TranslationCore;

public interface IGptHttpClient
{
    Task<string> Send(List<ChatMessage> messages, GptModel model);
}
