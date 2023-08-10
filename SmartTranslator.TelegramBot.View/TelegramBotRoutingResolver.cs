﻿using SmartTranslator.TelegramBot.View.Controls;
using SmartTranslator.TelegramBot.View.Exceptions;
using SmartTranslator.TelegramBot.View.Views;
using System.Reflection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SmartTranslator.TelegramBot.View;

public class TelegramBotRoutingResolver
{
    public async Task<ITelegramBotView?> RouteMessageOrThrow(Update update, List<ITelegramBotView> telegramBotViews)
    {
        T GetView<T>() where T : ITelegramBotView
        {
            var view = telegramBotViews.OfType<T>().FirstOrDefault();

            return view is null ? throw new InvalidOperationException($"No handler found for a message of type {typeof(T).Name}") : view;
        }

        if (update is null)
            return null;

        // translation
        if (update.Type == UpdateType.Message && update.Message?.Type == MessageType.Text)
        {
            var messageText = update?.Message?.Text;

            return messageText switch
            {
                var text when text == TelegramBotButtons.Start => await Task.FromResult(GetView<StartButtonView>()),
                var text when text == TelegramBotButtons.Translate => await Task.FromResult(GetView<TranslateButtonView>()),
                var text when text == "Determine" => await Task.FromResult(GetView<DetermineLanguageView>()),
                var text when IsMatchWithLanguageButtons(text) => await Task.FromResult(GetView<LanguageButtonView>()),
                _ => GetView<DefaultTranslateButtonView>()
            };
        }

        // /start /block etc
        if (update.Type == UpdateType.MyChatMember
            && update?.MyChatMember?.NewChatMember?.Status != update?.MyChatMember?.OldChatMember?.Status)
        {
            if (update?.MyChatMember?.NewChatMember?.Status == ChatMemberStatus.Kicked)
            {
                // return await GetView<BlockButtonView>().Render(update);
            }

            if (update?.MyChatMember?.NewChatMember?.Status == ChatMemberStatus.Member)
            {
                return GetView<ChangedStatusToMemberView>();
            }
        }

        // audio msg
        if (update?.Type == UpdateType.Message && update.Message?.Type == MessageType.Voice)
        {
            throw new VoiceMessageTypeNotImplementedException();
        }

        throw new UnknownMessageTypeException();
    }

    /// <summary>
    /// Checks whether provided text is one of TelegramBotLanguageButtons
    /// </summary>
    private bool IsMatchWithLanguageButtons(string text)
    {
        return typeof(TelegramBotLanguageButtons)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Any(fi => fi.IsLiteral && !fi.IsInitOnly && fi.GetValue(null).ToString() == text);
    }
}
