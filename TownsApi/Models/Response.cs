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
    }


}
