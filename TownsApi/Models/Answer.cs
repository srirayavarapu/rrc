namespace TownsApi.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public int QuestionId { get; set; }     // Foreign key to Question
        public Question Question { get; set; }  // Navigation property
    }


}
