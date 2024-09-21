using System.ComponentModel.DataAnnotations;

namespace TownsApi.Models
{
    public class Towns
    {
        [Key]
        public string? TownId { get; set; }
        public string? TownName { get; set; }
        public int Taxpayer { get; set; }
        public int? Properties { get; set; }
        public decimal? TotalValue { get; set; }
        public decimal? GrowthValue { get; set; }
        public decimal? TaxRate { get; set; }
        public decimal? GrowthAmt { get; set; }
        public decimal? GrowthYr { get; set; }
        public string? Contact { get; set; }        
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }

        public string? Zip { get; set; }
        public string? ContactNumber { get; set; }
        public string? Notes { get; set; }
        public string? DBName { get; set; }
        public string? MainPage { get; set; }
        public bool? CurrentActive { get; set; }
        public bool? RRCPP { get; set; }
        public string? SnapShots { get; set; }
        public string?LoginTimeFrom { get; set; }
        public string? LoginTimeTo { get; set; }
        public string? AllowedIPs { get; set; }
        public string? FTPInfo { get; set; }
        public decimal? lat { get; set; }
        public decimal? lng { get; set; }
        public string? Website { get; set; }

    }
    public class ResultObject
    {
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string? token { get; set; }

        public object? data { get; set; }
    }

}
