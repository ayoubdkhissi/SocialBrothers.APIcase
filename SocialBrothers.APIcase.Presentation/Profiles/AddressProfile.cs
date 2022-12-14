using AutoMapper;
using Bogus.DataSets;
using SocialBrothers.APIcase.Presentation.DTOs;

namespace SocialBrothers.APIcase.Presentation.Profiles;

public class AddressProfile : Profile
{
    protected AddressProfile()
    {
        CreateMap<Address, PostAddressDto>();
    }
}
