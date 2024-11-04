using Microsoft.EntityFrameworkCore;
using TownsApi.Data;

namespace TownsApi
{
    public interface IConnectionStringProvider
    {
        string GetConnectionString(string databaseName);
        DbContextOptions<TownDBContext> GetDbContextOptions(string databaseName);

    }

}
