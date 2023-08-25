using AutoMapper;
using SmartTranslator.Contracts.Dto;
using SmartTranslator.DataAccess.Entities;

namespace SmartTranslator.TelegramBot.Management.TranslationManagement;

public class TranslationManagerMappingProfile : Profile
{
    public TranslationManagerMappingProfile()
    {
        CreateMap<TelegramTranslationEntity, TelegramTranslationDto>();
    }
}