using Microsoft.EntityFrameworkCore;

namespace TownsApi.Data
{
    public class TownDBContext : DbContext
    {
        public TownDBContext(DbContextOptions<TownDBContext> options) : base(options)
        {
        }
        public DbSet<Models.Towns>? Towns { get; set; }
        public DbSet<Models.TaxPayer>? TaxPayer { get; set; }
        public DbSet<Models.pricingManual>? pricingManual { get; set; }
        public DbSet<Models.propertyType>? propertyType { get; set; }
        public DbSet<Models.Deprec>? Deprec { get; set; }
        public DbSet<Models.property>? property { get; set; }
    }
}
