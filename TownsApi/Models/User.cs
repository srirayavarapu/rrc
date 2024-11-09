namespace TownsApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        // Navigation property
        public ICollection<Response> Responses { get; set; }
    }

}
