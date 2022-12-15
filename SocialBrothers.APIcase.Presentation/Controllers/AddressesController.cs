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

/// <summary>
/// Controller with CRUD actions for Addresses
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AddressesController : ControllerBase
{

    // Private Services to be injected
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient;
    private readonly IValidator<AddressDto> _validator;
    private readonly ILogger<AddressesController> _logger;
    private readonly IAddressRepository _addressRepository;


    /// <summary>
    /// Constructor used to inject services
    /// </summary>
    public AddressesController(IAddressRepository addressRepository,
        IMapper mapper,
        IValidator<AddressDto> validator,
        HttpClient httpClient,
        ILogger<AddressesController> logger)
    {
        _addressRepository = addressRepository;
        _mapper = mapper;
        _validator = validator;
        _httpClient = httpClient;
        _logger = logger;
    }


    /// <summary>
    /// Get a list of addresses
    /// </summary>
    /// <param name="paginationParameters">Object defining the page number and the page size</param>
    /// <param name="queryParameters">object defining search parameters</param>
    /// <param name="sortCriteria">object defining sort criteria</param>
    /// <returns>List of addresses, empty list if there were none</returns>
    /// <response code="200">Returns the list of addresses</response>
    /// <response code="400">the orderBy criteria was invalid</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Address>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<Address>>> GetAddresses(
        [FromQuery] PaginationParameters paginationParameters,
        [FromQuery] QueryParameters queryParameters,
        [FromQuery] SortCriteria sortCriteria)
    {

        _logger.LogInformation("GetAddresses Method is invoked with parameters {@params}", new {paginationParameters, queryParameters, sortCriteria });

        // orderBy value specified must be one of the address fields
        if (sortCriteria.OrderBy != null && !queryParameters.GetType().GetProperties().Any(x => x.Name.ToLower() == sortCriteria.OrderBy.ToLower()))
            return BadRequest("The specified orderBy value is not an address field");

        var addresses = await _addressRepository.
            GetAddressesAsync(paginationParameters, queryParameters, sortCriteria);

        // we return OK 200 in all cases, even if the list is empty
        return Ok(addresses);
    }



    /// <summary>
    /// Get an address by id
    /// </summary>
    /// <param name="id">The id of the address to get</param>
    /// <returns>An ActionResult</returns>
    /// <response code="200">Returns the requested address</response>
    /// <response code="400">If no city with the given id was found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Address))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Address>> GetAddress(int id)
    {

        _logger.LogInformation($"GetAddress method is invoked with the id = {id}");

        // check if an address with the given id exists
        var address = await _addressRepository.GetAddressAsync(id);
        
        if(address is null)
        {
            return NotFound("No address with the given id was found!");
        }

        return Ok(address);
    }


    /// <summary>
    /// Add a new address
    /// </summary>
    /// <param name="addressDto">object representing address properties</param>
    /// <returns>An Action result</returns>
    /// <response code="201">Returns the newly created address</response>
    /// <response code="400">some address properties are invalid</response>
    /// <response code="500">the address could not be added due to some server error</response>
    /// 
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Address))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Address>> PostAddress(AddressDto addressDto)
    {

        _logger.LogWarning("Post address method is invoked with parameter {@params}", addressDto);

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

    /// <summary>
    /// Update an existing address
    /// </summary>
    /// <param name="id">the id of the address you want to update</param>
    /// <param name="addressDto">object representing address properties</param>
    /// <returns>An ActionResult</returns>
    /// <response code="200">Returns the updated address</response>
    /// <response code="400">Some address properties were invalid</response>
    /// <response code="404">No address with the given id was found</response>
    /// 
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Address))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateAddress(int id, AddressDto addressDto)
    {

        _logger.LogInformation("UpdateAddress method is invoked with parameters {@params}", new { id, addressDto });

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


    /// <summary>
    /// Delete an address by id
    /// </summary>
    /// <param name="id">The id of the address you want to delete</param>
    /// <returns>An Action result</returns>
    /// <response code="200">The address was successfully deleted</response>
    /// <response code="404">No address with the given id was found</response>
    /// <response code="500">The address could not be deleted due to some server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAddress(int id)
    {

        _logger.LogInformation("DeleteAddress method is invoked with params {@params}", id);

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



    /// <summary>
    /// Get the distance between two addresses origin and destination
    /// </summary>
    /// <param name="originId">id of the origin address</param>
    /// <param name="destinationId">if of the destination address</param>
    /// <returns>An ActionResult</returns>
    /// <response code="200">
    /// Returns an object containing the distance between the origin and the destination address,
    /// as well as the two addresses
    /// </response>
    /// <response code="400">Ids of the origin and destination addresses were not provided</response>
    /// <response code="404">
    /// One or both addresses were not found.
    /// or the distance could not be calculated because the addresses are not valid
    /// </response>
    /// <response code="500">The distance could not be calculated due to some server error.</response>
    /// 
    [HttpGet("distance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DistanceStats))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<double>> GetDistance(int originId, int destinationId)
    {
        _logger.LogInformation("GetDistance method is invoked with params {@params}", new { originId, destinationId });

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
                 * so we can traverse the response safely and get the status of the request
                 */
                JValue status = (JValue)joResponse["rows"]![0]!["elements"]![0]!["status"]!;


                // constructing our Distance Statistics object. it does not contain the distance yet
                var distanceStats = new DistanceStats()
                {
                    OriginAddress = joResponse["origin_addresses"]![0]!.ToString(),
                    DestinationAddress = joResponse["destination_addresses"]![0]!.ToString()
                };

                // if the status is equal to OK it means that the API has found a distance between the two address
                if (status.ToString().Equals("OK"))
                {
                    // get the distance and return the stats 
                    distanceStats.Distance = joResponse["rows"]![0]!["elements"]![0]!["distance"]!["text"]!.ToString();

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

            return StatusCode(500, "Oops! We can't calculate the distance between the two specified addresses");
        }
        

    }

}
