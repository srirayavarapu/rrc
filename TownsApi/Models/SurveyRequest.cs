namespace TownsApi.Models
{
    public class SurveyRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? CreatedBy { get; set; }
        public List<QuestionRequest>? Questions { get; set; }
    }
}
