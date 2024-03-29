﻿using SmartTranslator.Api.TelegramControllers;
using SmartTranslator.TelegramBot.View.Controls;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartTranslator.TelegramBot.View.Views;

public class FinalAnswerView : ITelegramBotView
{
    private readonly CoupleLanguageTranslatorController _coupleLanguageTranslatorController;

    public FinalAnswerView(CoupleLanguageTranslatorController coupleLanguageTranslatorController)
    {
        _coupleLanguageTranslatorController = coupleLanguageTranslatorController;
    }

    public async Task<MessageView> Render(Update update)
    {
        if (update.Message == null)
            throw new ArgumentException("FinalAnswerView got incorrect update (Message == null)");

        var result = await _coupleLanguageTranslatorController.GiveFinalAnswer(update);

        var markup = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton(TelegramBotButtons.Translate)
            })
        {
            ResizeKeyboard = true
        };
        var inlineButtons = (new TelegramBotInlineButtons()).Buttons.Select(button => new InlineKeyboardButton(button)
        {
            CallbackData = button
        }).ToArray();
        var inlineKeyboard = new InlineKeyboardMarkup(inlineButtons);

        return new MessageView
        {
            Text = result,
            Markup = markup,
            InlineMarkup = inlineKeyboard
        };
    }
}
