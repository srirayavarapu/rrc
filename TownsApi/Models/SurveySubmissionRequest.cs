namespace TownsApi.Models
{
    public class SurveySubmissionRequest
    {
        public int SurveyId { get; set; }
        public int UserId { get; set; }
        public List<QuestionSubmission> Questions { get; set; }
    }
}
