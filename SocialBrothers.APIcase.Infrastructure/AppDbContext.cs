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
                    Street = "Brockton Avenue",
                    City = "Abington MA",
                    Country = "United States",
                    HouseNumber = 777,
                    ZipCode = 96522
                },
                new Address
                {
                    Id = 2,
                    Street = "Memorial Drive",
                    City = "Avon MA",
                    Country = "United States",
                    HouseNumber = 30,
                    ZipCode = 2322
                },
                new Address
                {
                    Id = 3,
                    Street = "Hartford Avenue",
                    City = " Bellingham MA",
                    Country = "United States",
                    HouseNumber = 250,
                    ZipCode = 2019
                },
                new Address
                {
                    Id = 4,
                    Street = "Oak Street",
                    City = "Brockton MA",
                    Country = "United States",
                    HouseNumber = 700,
                    ZipCode = 2301
                }
            );
        });

        

        
    }

}
