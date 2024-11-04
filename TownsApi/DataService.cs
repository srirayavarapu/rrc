namespace TownsApi
{
    public class DataService
    {
        public readonly string _connectionString;

        public DataService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BaseConnection");
        }
    }
}
