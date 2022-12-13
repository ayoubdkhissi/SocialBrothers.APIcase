using Microsoft.EntityFrameworkCore;
using SocialBrothers.APIcase.Domain.Entities;
using SocialBrothers.APIcase.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
    
    public async Task<IEnumerable<Address>> GetAddressesAsync(int pageIndex = 1, int pageSize = 20)
    {
        return await _appDbContext.Addresses
            .Skip((pageIndex-1)*pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    
    public async Task<Address> GetAddressAsync(int id)
    {
        return await _appDbContext.Addresses.FindAsync(id);
    }


    public async Task<bool> InsertAddressAysnc(Address address)
    {
        try 
        {
            address.BuildFullAddress();

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
            address.BuildFullAddress();

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
