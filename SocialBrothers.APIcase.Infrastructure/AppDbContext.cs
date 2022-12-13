using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SocialBrothers.APIcase.Domain;
using SocialBrothers.APIcase.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SocialBrothers.APIcase.Infrastructure;
public class AppDbContext : DbContext
{

    public DbSet<Address> Addresses { get; set; }


    public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
    {
    }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // Configuring the Addresses Table
        modelBuilder.Entity<Address>(options =>
        {
            // Adding index on the FullAddress field to make search faster
            options.HasIndex(address => address.FullAddress);
        });

        base.OnModelCreating(modelBuilder);
    }

}
