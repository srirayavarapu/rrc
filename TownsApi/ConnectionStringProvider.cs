using Microsoft.EntityFrameworkCore;
using TownsApi.Data;

namespace TownsApi
{
    public class ConnectionStringProvider : IConnectionStringProvider
    {
        private readonly IConfiguration _configuration;

        public ConnectionStringProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString(string databaseName)
        {
            var baseConnectionString = _configuration.GetConnectionString("BaseConnection");
            return $"{baseConnectionString};Database={databaseName}";
        }

        public DbContextOptions<TownDBContext> GetDbContextOptions(string databaseName)
        {
            var connectionString = GetConnectionString(databaseName);

            var optionsBuilder = new DbContextOptionsBuilder<TownDBContext>();
            optionsBuilder.UseSqlServer(connectionString); // or UseMySql, UseSqlite, etc.
            return optionsBuilder.Options;
        }
    }
}