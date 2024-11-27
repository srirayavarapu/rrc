namespace TownsApi.Models
{
    public class Response
    {
        public int Id { get; set; }

        public int? AnswerId { get; set; }       // Foreign key to Answer (nullable for text responses)
        public Answer Answer { get; set; }       // Navigation property

        public string TextResponse { get; set; } // For open-ended text responses

        public int UserId { get; set; }          // Foreign key to User
        public User User { get; set; }           // Navigation property

        public int SurveyId { get; set; } // Identify the survey this response belongs to
        public int QuestionId { get; set; } // Identify the question this response belongs to
        public int QuestionType { get; set; } // Identify the type of question (MultipleChoice, Text, Dropdown)
        //public int? AnswerId { get; set; } // For MultipleChoice and Dropdown questions
        //public string? TextResponse { get; set; } // For Text responses
        //public int UserId { get; set; } // User submitting the response
    }


}
