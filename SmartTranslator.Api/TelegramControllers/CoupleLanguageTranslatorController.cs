using AutoMapper;
using SmartTranslator.Api.Exceptions;
using SmartTranslator.Contracts.Dto;
using SmartTranslator.Contracts.Requests;
using SmartTranslator.DataAccess.Entities;
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


    public Task<Language> DetermineLanguage(Message message)
    {
        return Task.FromResult(Language.Unknown);
    }


    public async Task SetLanguage(Language language)
    {
        return;
    }


    public async Task<EvaluationResponse> EvaluateContext(Message message)
    {
        var response = new EvaluationResponse
        {
            Percent = 0,
            Request = new ClarificationRequest
            {
                ClarifyingQuestion = "What is the meaning of life?"
            }
        };

        return await Task.FromResult(response);
    }


    public Task<TranslationStyle> DetermineStyle(Message message)
    {
        return Task.FromResult(TranslationStyle.Unknown);
    }


    public async Task SetStyle(TranslationStyle style)
    {
        return;
    }


    public async Task<string> GiveFinalAnswer(Message message)
    {
        return $"Your message was: {message.Text}";
    }


    public async Task<TelegramTranslationDto?> GetLatest(Update update)
    {
        if (update?.Message?.From == null)
            throw new ChannelsNotSupportedException();

        var userName = update.Message.From.ToString();
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


    public async Task AddExtraContext(Update update)
    {
        var context = update.Message.Text;
        // Sends context to manager
        return;
    }


    public async Task<TelegramTranslationEntity> AddAnswerToContextQuestion(Update update)
    {
        var context = update.Message.Text;
        // Sends context to manager 
        var entity = new TelegramTranslationEntity(); // Change to updated entity returned from manager
        return entity;
    }

    public async Task FinishLatestTranslation(Update update)
    {
        var entity = await GetLatest(update);
        if (entity != null)
            await _translationManager.FinishTranslation(entity.Id);
    }
}
