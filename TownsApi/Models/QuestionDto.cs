namespace TownsApi.Models
{
    public class QuestionDto
    {
        public int Id { get; set; } // Question ID
        public string Text { get; set; } // Question Text
        public int Type { get; set; } // Question Type (e.g., 1 = Multiple Choice, 2 = Text)
        public List<AnswerDto> Answers { get; set; } = new(); // List of Answers for the Question
    }
}
