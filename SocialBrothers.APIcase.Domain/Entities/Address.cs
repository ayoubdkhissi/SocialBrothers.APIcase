/**
 * @author: AYOUB DKHISSI 
 */

namespace SocialBrothers.APIcase.Domain.Entities;

/// <summary>
/// Entity Representing an address.
/// 
/// NOTES : 
/// • Id field mandatory because most DBMS platforms don't support a table without a primary key.
/// 
/// • The generated field "FullAddress" combines the values of all other fields,
///   this will help in searching, because the search is applied on all address fields.
///  
/// • An index is added on the FullAddress field to make search faster.
/// 
/// </summary>
public class Address
{
    /// <summary>
    /// Unique identifier of an address
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// String representing the street name of the address
    /// </summary>
    public string Street { get; set; }

    /// <summary>
    /// Integer representing the house number of the address
    /// </summary>
    public int HouseNumber { get; set; }

    /// <summary>
    /// Integer representing the Zip Code of the address
    /// </summary>
    public int ZipCode { get; set; }

    /// <summary>
    /// String repressing city name of the address
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// String representing country name of the address
    /// </summary>
    public string Country { get; set; }


    public override string ToString()
    {
        return $"{HouseNumber} {Street}, {City} {ZipCode} {Country}";
    }
}
