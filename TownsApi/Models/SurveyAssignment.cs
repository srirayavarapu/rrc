namespace TownsApi.Models
{
    public class SurveyAssignment
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime AssignedDate { get; set; }
    }
}
