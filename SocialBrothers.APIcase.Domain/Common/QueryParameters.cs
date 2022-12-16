/**
 * @author: AYOUB DKHISSI 
 */
namespace SocialBrothers.APIcase.Domain.Common;

public class QueryParameters
{
    // the following fields are used for searching
    public string Street { get; set; }
    public int? HouseNumber { get; set; }
    public int? ZipCode { get; set; }
    public string City { get; set; }
    public string Country { get; set; }

}
