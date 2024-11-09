namespace TownsApi.Models
{
    public class SurveryDetails
    {
        public int? SurveyId { get; set; }
        public string? SurveyTitle { get; set; }
        public string? SurveyDescription { get; set; }

        public int? QuestionId { get; set; }
        public string? QuestionText { get; set; }
        public QuestionType? QuestionType { get; set; }

        public int? AnswerId { get; set; }
        public string? AnswerText { get; set; }

        public int? ResponseId { get; set; }
        public string? ResponseText { get; set; }
        public int? UserId { get; set; }
        public string? Username { get; set; }
        public string? UserEmail { get; set; }
    }
}
