namespace TownsApi.Models
{
    public class SurveyResponseDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int SurveyId { get; set; }
        public string SurveyTitle { get; set; }
        public string QuestionText { get; set; }
        public string AnswerText { get; set; }
        public string ResponseText { get; set; }
    }

}
