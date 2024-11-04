using Microsoft.EntityFrameworkCore;
using TownsApi.Data;

namespace TownsApi
{
    public static class DbContextFactory
    {
        public static TownDBContext Create(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TownDBContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new TownDBContext(optionsBuilder.Options);
        }
    }
}
