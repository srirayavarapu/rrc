using Microsoft.EntityFrameworkCore;
using TownsApi.Models;

namespace TownsApi.Data
{
    public class TownDBContext : DbContext
    {
        public TownDBContext(DbContextOptions<TownDBContext> options) : base(options)
        {
        }
        public DbSet<Models.Towns>? Towns { get; set; }
        public DbSet<Models.TaxPayer>? TaxPayer { get; set; }
        public DbSet<Models.pricingManual>? pricingManual { get; set; }
        public DbSet<Models.propertyType>? propertyType { get; set; }
        public DbSet<Models.Deprec>? Deprec { get; set; }
        public DbSet<Models.property>? property { get; set; }
        public DbSet<Models.Survey>? Survey { get; set; }
        public DbSet<Models.Question>? Question { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Response> Responses { get; set; }

        //public DbSet<UsersData> UsersData { get; set; }
        //public DbSet<SurveyHeadersData> SurveyHeadersData { get; set; }
        //public DbSet<QuestionsData> QuestionsData { get; set; }
        //public DbSet<ResponsesData> ResponsesData { get; set; }
        //public DbSet<ChoicesData> ChoicesData { get; set; }
        public DbSet<OP_Security_Points> OP_Security_Points { get; set; }
    }
}
