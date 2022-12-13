using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialBrothers.APIcase.Domain.Common;
using SocialBrothers.APIcase.Domain.Entities;
using SocialBrothers.APIcase.Domain.Interfaces;

namespace SocialBrothers.APIcase.Presentation.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AddressesController : ControllerBase
{

    private readonly IAddressRepository _addressRepository;
    

    public AddressesController(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Address>>> GetAddresses(
        [FromQuery] PaginationParameters paginationParameters,
        [FromQuery] QueryParameters queryParameters,
        [FromQuery] SortCriteria sortCriteria)
    {

        // orderBy value specified must be one of the address fields
        if (sortCriteria.OrderBy != null && !queryParameters.GetType().GetProperties().Any(x => x.Name.ToLower() == sortCriteria.OrderBy.ToLower()))
            return BadRequest("The specified orderBy value is not an address field");

        var addresses = await _addressRepository.
            GetAddressesAsync(paginationParameters, queryParameters, sortCriteria);

        // we return OK 200 in all cases, even if the list is empty
        return Ok(addresses);
    }
}
