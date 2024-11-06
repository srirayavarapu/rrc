using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TownsApi.Models
{
    public class OP_Employee
    {
        public string nuser { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string initials { get; set; }
        public decimal empLevel { get; set; }
        public string utype { get; set; }
        public string name { get; set; }
        public string empIn { get; set; }
        public bool? fax { get; set; }
        public string ext { get; set; }
        public string position { get; set; }
        public string printyes { get; set; }
        public string password { get; set; }
        public string Locations { get; set; }
        public DateTime? PwdChangedOn { get; set; }
    }
}
