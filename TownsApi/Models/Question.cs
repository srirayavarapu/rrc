using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TownsApi.Models
{
    [Table("Question")]
    public class Question
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        public int Type { get; set; } // Store enum as int
        public int SurveyId { get; set; } // Foreign key
    }

}
