using AutoMapper;
using SocialBrothers.APIcase.Domain.Entities;
using SocialBrothers.APIcase.Presentation.DTOs;

namespace SocialBrothers.APIcase.Presentation.Profiles;

public class AddressProfile : Profile
{
    public AddressProfile()
    {
        CreateMap<PostAddressDto, Address>()
            .ForMember(source => source.Id, opt => opt.Ignore());

    }
}
