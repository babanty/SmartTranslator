using AutoMapper;
using SmartTranslator.Api.Exceptions;
using SmartTranslator.Contracts.Dto;
using SmartTranslator.Contracts.Requests;
using SmartTranslator.DataAccess.Entities;
using SmartTranslator.Enums;
using SmartTranslator.TelegramBot.Management.Exceptions;
using SmartTranslator.TelegramBot.Management.TranslationManagement;
using SmartTranslator.TranslationCore.Enums;
using Telegram.Bot.Types;


namespace SmartTranslator.Api.TelegramControllers;

public class CoupleLanguageTranslatorController
{
    private readonly ITranslationManager _translationManager;

    public CoupleLanguageTranslatorController(ITranslationManager translationManager)
    {
        _translationManager = translationManager;
    }


    public async Task NewUser(Update update)
    {
        var username = update.MyChatMember?.From?.Username;
        if (username is null)
            return;

        await _translationManager.Activate(username);
    }


    public async Task<TelegramTranslationDto> SetLanguages(Update update, Language baseLanguage)
    {
        var entity = await GetLatest(update);

        if (entity == null)
            throw new EntityNotFoundException();

        var dto = await _translationManager.SetLanguages(entity.Id, baseLanguage);

        return dto;
    }


    public async Task<TelegramTranslationDto> EvaluateContext(Update update)
    {
        var entity = await GetLatest(update);

        if (entity == null)
            throw new EntityNotFoundException();

        var response = await _translationManager.DetermineContext(entity.Id);

        return response;
    }


    public async Task<Context> GetLatestContext(Update update)
    {
        var entity = await GetLatest(update);

        if (entity == null)
            throw new EntityNotFoundException();

        var response = await _translationManager.GetLatestContext(entity.Id);

        return response;
    }


    public async Task<TelegramTranslationDto> SetStyle(Update update, TranslationStyle style)
    {
        var entity = await GetLatest(update);

        if (entity == null)
            throw new EntityNotFoundException();

        var dto = await _translationManager.SetStyle(entity.Id, style);

        return dto;
    }


    public async Task<string> GiveFinalAnswer(Update update)
    {
        var entity = await GetLatest(update);

        if (entity == null)
            throw new EntityNotFoundException();

        var response = await _translationManager.GetLatestTranslatedText(entity.Id);

        return response;
    }


    public async Task<TelegramTranslationDto?> GetLatest(Update update)
    {
        if (update?.Message?.From == null)
            throw new ChannelsNotSupportedException();

        var userName = update.Message.From.Username;
        var chatId = update.Message.Chat.Id;

        return await _translationManager.GetLatest(userName, chatId);
    }


    public async Task<TelegramTranslationDto?> GetLatestByQuery(Update update)
    {
        var userName = update.CallbackQuery.From.Username;
        var chatId = update.CallbackQuery.Message.Chat.Id;

        return await _translationManager.GetLatest(userName, chatId);
    }


    public async Task<TelegramTranslationDto> CreateTranslation(Update update)
    {
        var request = new CreateTelegramTranslationEntityRequest
        {
            BaseText = update.Message.Text,
            ChatId = update.Message.Chat.Id,
            UserName = update.Message.From.Username
        };
        var entity = await _translationManager.Create(request);

        return entity;
    }


    public async Task<TelegramTranslationDto> AddExtraContext(Update update)
    {
        var context = update.Message.Text;
        var entity = await GetLatest(update);

        var dto = await _translationManager.AddExtraContext(entity.Id, context);

        return dto;
    }


    public async Task<TelegramTranslationDto> AddAnswerToContextQuestion(Update update)
    {
        var answer = update.Message.Text;
        var entity = await GetLatest(update);

        var dto = await _translationManager.AddAnswerToContextQuestion(entity.Id, answer);

        return dto;
    }

    public async Task FinishLatestTranslation(Update update)
    {
        var entity = await GetLatest(update);
        if (entity != null)
            await _translationManager.FinishTranslation(entity.Id);
    }

    public async Task Block(Update update)
    {
        var username = update.MyChatMember?.From?.Username;
        if (username is null)
            return;

        await _translationManager.Block(username);
    }

    public async Task AddFeedback(Update update, TranslationFeedback feedback)
    {
        var entity = await GetLatestByQuery(update);

        if (entity != null)
            await _translationManager.AddFeedback(entity.Id, feedback);
    }
}
