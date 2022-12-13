using Microsoft.EntityFrameworkCore;
using SocialBrothers.APIcase.Domain.Entities;

namespace SocialBrothers.APIcase.Infrastructure;
public class AppDbContext : DbContext
{

    public DbSet<Address> Addresses { get; set; }


    public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
    {
        Database.EnsureCreated();
    }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuring the Addresses Table
        modelBuilder.Entity<Address>(options =>
        {
            // Adding indexes on all fields to make search faster
            options.HasIndex(address => address.Street);
            options.HasIndex(address => address.ZipCode);
            options.HasIndex(address => address.City);
            options.HasIndex(address => address.Country);


            // Seeding the database
            options.HasData
            (
                new Address
                {
                    Id = 1,
                    Street = "Laazifat",
                    City = "Tangier",
                    Country = "Morocco",
                    HouseNumber = 12,
                    ZipCode = 90000
                },
                new Address
                {
                    Id = 2,
                    Street = "Los Santos",
                    City = "California",
                    Country = "USA",
                    HouseNumber = 77,
                    ZipCode = 91100
                },
                new Address
                {
                    Id = 3,
                    Street = "Chiko",
                    City = "Las Vegas",
                    Country = "USA",
                    HouseNumber = 172,
                    ZipCode = 45855
                },
                new Address
                {
                    Id = 4,
                    Street = "Kacman",
                    City = "Tehran",
                    Country = "Iran",
                    HouseNumber = 172,
                    ZipCode = 85255
                }
            );
        });

        

        
    }

}
