using System.ComponentModel.DataAnnotations;

namespace TownsApi.Models
{

    public class Survey
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        // Navigation property
        public ICollection<Question>? Questions { get; set; }
    }

}
