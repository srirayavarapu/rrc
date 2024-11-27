namespace TownsApi.Models
{
    public class SurveyResponse
    {

        public int SurveyId { get; set; } // Identify the survey this response belongs to
        public int QuestionId { get; set; } // Identify the question this response belongs to
        public int QuestionType { get; set; } // Identify the type of question (MultipleChoice, Text, Dropdown)
        public int? AnswerId { get; set; } // For MultipleChoice and Dropdown questions
        public string? TextResponse { get; set; } // For Text responses
        public int UserId { get; set; } // User submitting the response
    }
}
