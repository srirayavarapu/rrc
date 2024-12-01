namespace TownsApi.Models
{
    public class GetSurveysResponse
    {
        public int Id { get; set; } // Survey ID
        public string Title { get; set; } // Survey Title
        public string Description { get; set; } // Survey Description
        public int CreatedBy { get; set; } // Admin/Employee ID who created the survey
        public List<QuestionDto> Questions { get; set; } = new(); // List of Questions
    }
}
