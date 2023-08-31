using CitiesManager.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.Infrastructure.DatabaseContext;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        
    }
    public ApplicationDbContext()
    {
        
    }

    public virtual DbSet<City> Cities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<City>().HasData(new City() { CityId = Guid.Parse( "9DF994BA-D082-4808-84D2-5E28CD478C5A" ) , CityName="New York"});
        modelBuilder.Entity<City>().HasData(new City() { CityId = Guid.Parse("F7D45F62-DDE5-4A6C-B0A9-766A50D489C2"), CityName= "London" });
    }
}
