using AutoMapper;
using Bogus.Bson;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SocialBrothers.APIcase.Domain.Common;
using SocialBrothers.APIcase.Domain.Entities;
using SocialBrothers.APIcase.Domain.Interfaces;
using SocialBrothers.APIcase.Presentation.DTOs;

namespace SocialBrothers.APIcase.Presentation.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AddressesController : ControllerBase
{

    private readonly IAddressRepository _addressRepository;
    private readonly IValidator<AddressDto> _validator;
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient;


    public AddressesController(IAddressRepository addressRepository, 
        IMapper mapper, 
        IValidator<AddressDto> validator, 
        HttpClient httpClient)
    {
        _addressRepository = addressRepository;
        _mapper = mapper;
        _validator = validator;
        _httpClient = httpClient;
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

    [HttpGet("{id}")]
    public async Task<ActionResult<Address>> GetAddress(int id)
    {
        // check if an address with the given id exists
        var address = await _addressRepository.GetAddressAsync(id);
        
        if(address is null)
            return NotFound("No address with the given id was found!");

        return Ok(address);
    }

    [HttpPost]
    public async Task<ActionResult<Address>> PostAddress(AddressDto addressDto)
    {
        //Validating the DTO
        ValidationResult validationResult = await _validator.ValidateAsync(addressDto);
        if(!validationResult.IsValid)
        {
            // return validation error messages
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        // Mapping DTO into Address Entity
        var address = _mapper.Map<Address>(addressDto);

        // Creating the item in DB
        bool created = await _addressRepository.InsertAddressAysnc(address);

        if(!created)
        {
            return StatusCode(500, "Address can't be added! some error has occurred");
        }

        // returning the created item (201 Created)
        return Created($"~api/addresses/{address.Id}", address);

    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAddress(int id, AddressDto addressDto)
    {
        // Check if the address exist
        var address = await _addressRepository.GetAddressAsync(id);

        if(address is null)
        {
            return NotFound("No address with the given id was found!");
        }

        // Validate the passed DTO
        ValidationResult validationResult = await _validator.ValidateAsync(addressDto);
        if (!validationResult.IsValid)
        {
            // return validation error messages
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        // Mapping the passed DTO to the address retrieved from database
        _mapper.Map(addressDto, address);

        // Update the address
        await _addressRepository.UpdateAddressAysnc(address);


        return Ok("The address was successfuly updated");
        // we can also return NoContent() 204.
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAddress(int id)
    {
        // Check if the address exist
        var address = await _addressRepository.GetAddressAsync(id);

        if (address is null)
        {
            return NotFound("No address with the given id was found!");
        }

        bool deleted = await _addressRepository.DeteleAddressAsync(address);

        if(!deleted)
        {
            return StatusCode(500, "Address can't be deleted! some error has occurred");
        }

        return Ok("Address was successfully deleted");
        // we can also return a NoContent() 204.
    }

    [HttpGet("distance")]
    public async Task<ActionResult<double>> GetDistance(int originId, int destinationId)
    {
        // id1 and id2 should not be null
        if(originId == 0 || destinationId == 0)
        {
            return BadRequest("Please provide the values for originId and destinationId");
        }
        // check if the two addresses exist in DB
        var originAddress = await _addressRepository.GetAddressAsync(originId);
        if(originAddress is null)
        {
            return NotFound("The specified origin address is not found!");
        }

        var destinationAddress = await _addressRepository.GetAddressAsync(destinationId);
        if(destinationAddress is null)
        {
            return NotFound("The specified destination address is not found");
        }


        // Construct request to make call to the public API
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{Constants.DISTANCES_API_BASE_URL}?origins={originAddress.ToString()}&destinations={destinationAddress.ToString()}&key={Constants.DISTANCES_API_KEY}"),
        };

        try
        {
            using (var response = await _httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                
                JObject joResponse = JObject.Parse(await response.Content.ReadAsStringAsync());

                /*
                 * We know exactly the JSON tree structure of each response possible,
                 * according to the documentation here : https://distancematrix.ai/distance-matrix-api#responses
                 * so we can use the following line to traverse and get the status of the request
                 */
                JValue status = (JValue)joResponse["rows"][0]["elements"][0]["status"];


                // constructing our Distance Statistics object. it does not contain the distance yet
                var distanceStats = new DistanceStats()
                {
                    OriginAddress = joResponse["origin_addresses"][0].ToString(),
                    DestinationAddress = joResponse["destination_addresses"][0].ToString()
                };

                // if the status is equal to OK it means that the API has found a distance between the two address
                if (status.ToString().Equals("OK"))
                {
                    // get the distance and return the stats 
                    distanceStats.Distance = joResponse["rows"][0]["elements"][0]["distance"].ToString();

                    return Ok(distanceStats);
                }

                // Api could not calculate the distance
                distanceStats.Distance = "NOT FOUND!";
                return NotFound(distanceStats);

            }
        }
        catch (Exception ex)
        {
            // log that the call to the API has failed

            return NotFound("Oops! We can't calculate the distance between the two specified addresses");
        }
        

    }

}
