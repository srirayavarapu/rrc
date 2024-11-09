using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TownsApi.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public QuestionType Type { get; set; }  // Enum type for question type

        public int SurveyId { get; set; }       // Foreign key to Survey
        public Survey Survey { get; set; }       // Navigation property

        // Navigation property
        public ICollection<Answer> Answers { get; set; }
        public ICollection<Response> Responses { get; set; }
    }



}
