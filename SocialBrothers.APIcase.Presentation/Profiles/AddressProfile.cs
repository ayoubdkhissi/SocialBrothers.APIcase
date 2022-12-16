/**
 * @author: AYOUB DKHISSI 
 */

using AutoMapper;
using SocialBrothers.APIcase.Domain.Entities;
using SocialBrothers.APIcase.Presentation.DTOs;

namespace SocialBrothers.APIcase.Presentation.Profiles;

/// <summary>
/// Class containing Mapping rules for an Address
/// </summary>
public class AddressProfile : Profile
{

    /// <summary>
    /// Constructor
    /// </summary>
    public AddressProfile()
    {
        CreateMap<AddressDto, Address>()
            .ForMember(source => source.Id, opt => opt.Ignore());

    }
}
