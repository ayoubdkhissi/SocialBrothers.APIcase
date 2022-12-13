using Microsoft.EntityFrameworkCore;
using SocialBrothers.APIcase.Domain.Common;
using SocialBrothers.APIcase.Domain.Entities;
using SocialBrothers.APIcase.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SocialBrothers.APIcase.Infrastructure.Services;
public class AddressRepository : IAddressRepository
{

    private readonly AppDbContext _appDbContext;

    public AddressRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    
    public async Task<IEnumerable<Address>> GetAddressesAsync(
        PaginationParameters paginationParameters,
        QueryParameters queryParameters,
        SortCriteria sortCriteria)
    {


        /*
            The best way to make the search dynamic is to use the package OData.
            another approach would be to build lambda expressions dynamically, but it is very complex 
            and error prone.

            the approach I'm going to take is to build a Raw SQL expression, it is a bad idea, 
            but it is simpler than building Expression Trees dynamically.

            PS: in a production scenario I would definitely use OData.
         */
        
        // default SQL query
        string sql = "select * from Addresses ";

        // this parameter indicates whether a where clause was introduced in the SQL expression or not
        bool where = false;

        // we loop through the properties of the query parameters object using reflections
        foreach(PropertyInfo prop in queryParameters.GetType().GetProperties())
        {
            // get the value of the property
            var val = prop.GetValue(queryParameters, null);

            // if the property has a value, then we add it to the filter
            if(val is not null)
            {
                if(!where)
                {
                    sql += $"where {prop.Name} = '{val}' ";
                    where = true;
                }
                else
                {
                    sql += $"and {prop.Name} = '{val}' ";
                }
            }
        }


        // sorting
        if(sortCriteria.OrderBy is not null)
        {
            sql += $"order by {sortCriteria.OrderBy} ";

            if (sortCriteria.Desc)
                sql += "DESC";
        }

        // get addresses using the raw SQL that we built
        var addresses = _appDbContext.Addresses.FromSqlRaw(sql);

        // Sorting

        // pagination
        addresses = addresses.Skip((paginationParameters.PageIndex - 1) * paginationParameters.PageSize)
            .Take(paginationParameters.PageSize);



        return await addresses.ToListAsync();

    }

    
    public async Task<Address> GetAddressAsync(int id)
    {
        return await _appDbContext.Addresses.FindAsync(id);
    }


    public async Task<bool> InsertAddressAysnc(Address address)
    {
        try 
        {

            await _appDbContext.Addresses.AddAsync(address);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAddressAysnc(Address address)
    {
        try
        {

            _appDbContext.Addresses.Update(address);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    public async Task<bool> DeteleAddressAsync(Address address)
    {
        try
        {
            _appDbContext.Remove(address);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

}
