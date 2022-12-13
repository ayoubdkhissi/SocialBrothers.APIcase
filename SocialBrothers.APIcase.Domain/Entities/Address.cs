using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public int Id { get; set; }

    public string Street { get; set; }

    public int HouseNumber { get; set; }

    public int ZipCode { get; set; }

    public string City { get; set; }

    public string Country { get; set; }
}
