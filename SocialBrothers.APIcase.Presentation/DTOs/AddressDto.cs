namespace SocialBrothers.APIcase.Presentation.DTOs;

/// <summary>
/// Data Transfer object for an Address entity.
/// it is used for Post and Put, It has the same properties as an Address entity except for the id.
/// but usually DTOs and Entities can have different properties. 
/// </summary>
public class AddressDto
{
    /// <summary>
    /// String representing the street name
    /// </summary>
    public string? Street { get; set; }

    /// <summary>
    /// Integer representing the house number
    /// </summary>
    public int? HouseNumber { get; set; }

    /// <summary>
    /// Integer representing the Zip Code  
    /// </summary>
    public int? ZipCode { get; set; }

    /// <summary>
    /// String repressing city name
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// String representing country name
    /// </summary>
    public string? Country { get; set; }
}
