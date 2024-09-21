using Microsoft.EntityFrameworkCore;

namespace TownsApi.Data
{
    public class TownDBContext : DbContext
    {
        public TownDBContext(DbContextOptions<TownDBContext> options) : base(options)
        {
        }
        public DbSet<Models.Towns>? Towns { get; set; }

    }
}
