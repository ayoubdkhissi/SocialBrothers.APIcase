/**
 * @author: AYOUB DKHISSI 
 */
using SocialBrothers.APIcase.Domain.Common;
using SocialBrothers.APIcase.Domain.Entities;

namespace SocialBrothers.APIcase.Domain.Interfaces;
public interface IAddressRepository
{

    /// <summary>
    /// Get a list of Addresses with pagination
    /// </summary>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size: number of items returned</param>
    /// <returns>list of Addresses</returns>
    Task<IEnumerable<Address>> GetAddressesAsync(
        PaginationParameters paginationParameters,
        QueryParameters queryParameters,
        SortCriteria sortCriteria);

    /// <summary>
    /// Get an address by id
    /// </summary>
    /// <param name="id">id of the address</param>
    /// <returns>The Address with the given id</returns>
    Task<Address> GetAddressAsync(int id);

    /// <summary>
    /// Insert an address
    /// </summary>
    /// <param name="address"></param>
    /// <returns>True if the item was successfully inserted, false otherwise </returns>
    Task<bool> InsertAddressAysnc(Address address);

    /// <summary>
    /// Update an Address
    /// </summary>
    /// <param name="address"></param>
    /// <returns>true if the address was successfully updated, false otherwise</returns>
    Task<bool> UpdateAddressAysnc(Address address);

    /// <summary>
    /// Delete an Address
    /// </summary>
    /// <param name="id"></param>
    /// <returns>true if the address was successfully deleted, false otherwise </returns>
    Task<bool> DeteleAddressAsync(Address address);

}
