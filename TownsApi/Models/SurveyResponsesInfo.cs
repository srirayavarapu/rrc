using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TownsApi.Models
{
    [Table("SurveyResponsesInfo")]
    public class SurveyResponsesInfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public int UserId { get; set; }
        public int QuestionId { get; set; }
        public int? AnswerId { get; set; }
        public string? TextResponse { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
