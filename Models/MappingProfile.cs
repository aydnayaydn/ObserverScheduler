using AutoMapper;
using ObserverScheduler.Models;
using ObserverScheduler.Entities;
using ObserverScheduler.Helper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserViewModel>().ForMember(dest => dest.ApiKey, opt => opt.MapFrom(src => 
            src.ApiKey != null && !string.IsNullOrEmpty(src.ApiKey) ? EncryptionHelper.Decrypt(src.ApiKey, src.ApiKeyTag) : null));
        
        CreateMap<UserCreateModel, User>();
    }
}