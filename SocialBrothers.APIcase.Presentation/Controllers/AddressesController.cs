using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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


    public AddressesController(IAddressRepository addressRepository, IMapper mapper, IValidator<AddressDto> validator)
    {
        _addressRepository = addressRepository;
        _mapper = mapper;
        _validator = validator;
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
            return StatusCode(500, "Address can't be added! some error has occured");
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

}
