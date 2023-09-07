using AutoMapper;
using SmartTranslator.Api.Exceptions;
using SmartTranslator.Contracts.Dto;
using SmartTranslator.Contracts.Requests;
using SmartTranslator.DataAccess.Entities;
using SmartTranslator.TelegramBot.Management.Exceptions;
using SmartTranslator.TelegramBot.Management.TranslationManagement;
using SmartTranslator.TranslationCore.Abstractions.Models;
using SmartTranslator.TranslationCore.Enums;
using Telegram.Bot.Types;


namespace SmartTranslator.Api.TelegramControllers;

public class CoupleLanguageTranslatorController
{
    private readonly ITranslationManager _translationManager;
    private readonly IMapper _mapper;

    public CoupleLanguageTranslatorController(ITranslationManager translationManager,
                                              IMapper mapper)
    {
        _translationManager = translationManager;
        _mapper = mapper;
    }


    public async Task NewUser(ChatMemberUpdated chatMemberUpdated)
    {
        return;
    }


    public Task<string> Translate(Message message)
    {
        return Task.FromResult("I don't know how to translate yet");
    }


    public async Task<TelegramTranslationDto> SetLanguages(Update update,Language baseLanguage)
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
}
