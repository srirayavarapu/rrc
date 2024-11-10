namespace TownsApi.Models
{
    public class AnswerSubmission
    {
        public int SurveyId { get; set; }
        public int QuestionId { get; set; }
        public QuestionType QuestionType { get; set; }  // Include QuestionType
        public int? AnswerId { get; set; }  // For MultipleChoice and Dropdown questions
        public string TextResponse { get; set; }  // For Text questions
        public string UserEmail { get; set; }  // Identifying user by email
    }
}
