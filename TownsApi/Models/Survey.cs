using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TownsApi.Models
{
  
    public class Survey
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
