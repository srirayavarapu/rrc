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
        public string? LoginTimeFrom { get; set; }
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


    public class TaxPayer
    {
        [Key]
        public int accountno { get; set; }
        public string nbrhd { get; set; }
        public string owner { get; set; }
        public DateTime inputdate { get; set; }
        public int locnum { get; set; }
        public string locsuffix { get; set; }
        public string locstreet { get; set; }
        public string dba { get; set; }
        public string mailaddr1 { get; set; }
        public string mailaddr2 { get; set; }
        public string mailcity { get; set; }
        public string mailstate { get; set; }
        public string mailzip { get; set; }
        public string areacode { get; set; }
        public string phone { get; set; }
        public string source { get; set; }
        public string taxcode { get; set; }
        public string datalister { get; set; }
        public string entryclerk { get; set; }
        public decimal totalvalue { get; set; }
        public decimal oldtotal1 { get; set; }
        public decimal oldtotal2 { get; set; }
        public decimal oldtotal3 { get; set; }
        public DateTime listdate { get; set; }
        public string busntype { get; set; }
        public string user1 { get; set; }
        public string user2 { get; set; }
        public string user3 { get; set; }
        public string user4 { get; set; }
        public DateTime lastinput { get; set; }
        public string status { get; set; }
        public decimal growth { get; set; }
        public string notes { get; set; }
        public string penalty { get; set; }
        public decimal exemption { get; set; }
        public string emailid { get; set; }
        public decimal penaltyval { get; set; }
        public decimal netvalue { get; set; }
        public string commno { get; set; }
        public decimal iTotal { get; set; }
        public decimal fTotal { get; set; }
        public decimal pTotal { get; set; }
        public decimal oTotal { get; set; }
        public decimal mTotal { get; set; }
        public string Action { get; set; }
        public string FOL { get; set; }
        public string WebAddress { get; set; }
        public string FId { get; set; }
        //public byte[] RowVer { get; set; }
        public string EditUser { get; set; }
        public DateTime EditDate { get; set; }
        public string EntryUser { get; set; }
        public DateTime EntryDate { get; set; }
        public string Password { get; set; }
        public string FOLEmail { get; set; }

    }

    public class pricingManual
    {
        [Key]
        public short PMYear { get; set; }
        public string pricecode { get; set; }
        public string descript { get; set; }
        public decimal unitcost { get; set; }
        public string category { get; set; }

    }

    public class propertyType
    {
        [Key]
        public string proptype { get; set; }
        public string descript { get; set; }
        public string exemption { get; set; }
    }
    public class Deprec
    {
        [Key]
        public int age { get; set; }
        public string cond { get; set; }
        public decimal Dpercent { get; set; }
    }
    public class LookUPData
    {
        public List<pricingManual>? pricingManual { get; set; }
        public List<propertyType>? propertyType { get; set; }
         public List<Deprec> Deprec { get; set; }
    }
}
