namespace TownsApi.Models
{
    public class QuestionRequest
    {
        public string? Text { get; set; }
        public QuestionType? Type { get; set; }
        public List<AnswerRequest> Answers { get; set; }

    }
    public class AnswerRequest
    {
        public string Text { get; set; }
    }
}
