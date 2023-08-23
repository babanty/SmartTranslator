using AutoMapper;
using SmartTranslator.Contracts.Dto;
using SmartTranslator.DataAccess.Entities;

namespace SmartTranslator.TelegramBot.Management;

public class TelegramTranslationMappingProfile : Profile
{
    public TelegramTranslationMappingProfile()
    {
        CreateMap<TelegramTranslationEntity, TelegramTranslationDto>();
    }
}
