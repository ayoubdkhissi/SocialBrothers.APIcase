using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocialBrothers.APIcase.Domain.Common;
using SocialBrothers.APIcase.Domain.Entities;
using SocialBrothers.APIcase.Domain.Interfaces;
using SocialBrothers.APIcase.Presentation.Controllers;
using SocialBrothers.APIcase.Presentation.DTOs;

namespace SocialBrothers.APIcase.Tests;
public class AddressesControllerTests
{
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient;
    private readonly IValidator<AddressDto> _validator;
    private readonly ILogger<AddressesController> _logger;
    private readonly IAddressRepository _addressRepository;

    public AddressesControllerTests()
    {
        // Mocking dependencies
        _mapper = A.Fake<IMapper>();
        _httpClient = A.Fake<HttpClient>();
        _validator = A.Fake<IValidator<AddressDto>>();
        _logger = A.Fake<ILogger<AddressesController>>();
        _addressRepository = A.Fake<IAddressRepository>();
    }


    /// <summary>
    /// Get Addresses method should return OK when everything went okay
    /// </summary>
    [Fact]
    public async Task AddressesController_GetAddresses_ReturnOK()
    {
        // Arrange
        var addresses = A.Fake<IEnumerable<Address>>();
        var paginationParameters = A.Fake<PaginationParameters>();
        var queryParameters = A.Fake<QueryParameters>();
        var sortCriteria = A.Fake<SortCriteria>();

            // Faking call to repository
        A.CallTo(() => _addressRepository.GetAddressesAsync(paginationParameters, queryParameters, sortCriteria)).Returns(Task.FromResult(addresses));

        var addressesController = new AddressesController(_addressRepository, _mapper, _validator, _httpClient, _logger);

        // Act
        var result = await addressesController.GetAddresses(paginationParameters, queryParameters, sortCriteria);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(OkObjectResult));
    }

    /// <summary>
    /// Get addresses method should return Bad Request when passing an invalid sort criteria
    /// </summary>
    [Fact]
    public async Task AddressesController_GetAddresses_ReturnBadRequest()
    {
        // Arrange
        var addresses = A.Fake<IEnumerable<Address>>();
        var paginationParameters = A.Fake<PaginationParameters>();
        var queryParameters = A.Fake<QueryParameters>();
        
            // passing an orderBy criteria that does not exist (not an address property name)
        var sortCriteria = new SortCriteria() { OrderBy = "fake_prop"};
        A.CallTo(() => _addressRepository.GetAddressesAsync(paginationParameters, queryParameters, sortCriteria)).Returns(Task.FromResult(addresses));

        var addressesController = new AddressesController(_addressRepository, _mapper, _validator, _httpClient, _logger);

        // Act
        var result = await addressesController.GetAddresses(paginationParameters, queryParameters, sortCriteria);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(BadRequestObjectResult));
    }
    
    [Fact]
    public async Task AddressesController_GetAddress_ReturnOK()
    {
        // Arrange
        var address = A.Fake<Address>();
        A.CallTo(() => _addressRepository.GetAddressAsync(1)).Returns(Task.FromResult(address));

        var addressesController = new AddressesController(_addressRepository, _mapper, _validator, _httpClient, _logger);

        // Act
        var result = await addressesController.GetAddress(1);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(OkObjectResult));
    }

    /// <summary>
    /// Get address method should return Not Found when passing the id of a non existing Address.
    /// we fake that the repository returns null
    /// </summary>
    [Fact]
    public async Task AddressesController_GetAddress_ReturnNotFound()
    {
        // Arrange
        Address address = null;
        A.CallTo(() => _addressRepository.GetAddressAsync(1)).Returns(Task.FromResult(address));

        var addressesController = new AddressesController(_addressRepository, _mapper, _validator, _httpClient, _logger);

        // Act
        var result = await addressesController.GetAddress(1);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(NotFoundObjectResult));
    }


    /// <summary>
    /// Post Address method should return created when the Passed DTO is valid, the Mapping was successful
    /// and the repository successfully inserted the address
    /// </summary>
    [Fact]
    public async Task AddressesController_PostAddress_ReturnCreated()
    {
        // Arrange
        var addressDto = A.Fake<AddressDto>();
        var address = A.Fake<Address>();

        // Mocking calls to validator, mapper and repository

        A.CallTo(() => _validator.ValidateAsync(addressDto, default)).Returns(Task.FromResult(new ValidationResult()));
        A.CallTo(() => _mapper.Map<Address>(addressDto)).Returns(address);
        A.CallTo(() => _addressRepository.InsertAddressAysnc(address)).Returns(true);

        var addressesController = new AddressesController(_addressRepository, _mapper, _validator, _httpClient, _logger);

        // Act
        var result = await addressesController.PostAddress(addressDto);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(CreatedResult));
    }

    /// <summary>
    /// Post address method should return Bad Request when the passed DTO is invalid
    /// </summary>
    [Fact]
    public async Task AddressesController_PostAddress_ReturnBadRequest()
    {
        // Arrange
        var addressDto = A.Fake<AddressDto>();
        var address = A.Fake<Address>();

            // Mocking calls to, mapper and repository
            // a mock address dto will be passed to the validator, should validation should fail
        A.CallTo(() => _mapper.Map<Address>(addressDto)).Returns(address);
        A.CallTo(() => _addressRepository.InsertAddressAysnc(address)).Returns(true);

        var addressesController = new AddressesController(_addressRepository, _mapper, _validator, _httpClient, _logger);

        // Act
        var result = await addressesController.PostAddress(addressDto);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(BadRequestObjectResult));
    }


    /// <summary>
    /// Post Address method should return 500 Status code when the inserting failed
    /// </summary>
    [Fact]
    public async Task AddressesController_PostAddress_Return500()
    {
        // Arrange
        var addressDto = A.Fake<AddressDto>();
        var address = A.Fake<Address>();

        // Mocking calls to, mapper and repository
        A.CallTo(() => _validator.ValidateAsync(addressDto, default)).Returns(Task.FromResult(new ValidationResult()));
        A.CallTo(() => _mapper.Map<Address>(addressDto)).Returns(address);
        A.CallTo(() => _addressRepository.InsertAddressAysnc(address)).Returns(false);

        var addressesController = new AddressesController(_addressRepository, _mapper, _validator, _httpClient, _logger);

        // Act
        var result = await addressesController.PostAddress(addressDto);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(ObjectResult)); // if the method returns an ObjectResult, it means it returned 500
    }


    /// <summary>
    /// Delete Address method should return Not Found when there is no address with the given id
    /// </summary>
    [Fact]
    public async Task AddressesController_DeleteAddress_ReturnNotFound()
    {
        // Arrange
        Address address = null;

        // Mocking calls to repository
        A.CallTo(() => _addressRepository.GetAddressAsync(1)).Returns(address);

        var addressesController = new AddressesController(_addressRepository, _mapper, _validator, _httpClient, _logger);

        // Act
        var result = await addressesController.DeleteAddress(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType(typeof(NotFoundObjectResult));
    }


    /// <summary>
    /// DeleteAddress method should return OK if the passed id belongs to an address in the database
    /// </summary>
    [Fact]
    public async Task AddressesController_DeleteAddress_ReturnOk()
    {
        // Arrange
        Address address = A.Fake<Address>();

        // Mocking calls to repository
        A.CallTo(() => _addressRepository.GetAddressAsync(1)).Returns(address);
        A.CallTo(() => _addressRepository.DeteleAddressAsync(address)).Returns(true);

        var addressesController = new AddressesController(_addressRepository, _mapper, _validator, _httpClient, _logger);

        // Act
        var result = await addressesController.DeleteAddress(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType(typeof(OkObjectResult));
    }

    
}
