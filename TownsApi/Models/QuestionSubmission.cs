namespace TownsApi.Models
{
    public class QuestionSubmission
    {
        public int QuestionId { get; set; }
        public int QuestionType { get; set; } // 1 = MultipleChoice, 2 = Text, 3 = Dropdown, 4 = RadioButton
        public string QuestionAnswer { get; set; } // Text or Answer IDs as comma-separated string
    }
}
