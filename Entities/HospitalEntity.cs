using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class HospitalEntity
    {
        [Key]
        public int HospitalId { get; set; }
        public int? AddedBy { get; set; }
        public string HospitalName { get; set; }
        public string? Code { get; set; }
        public string? Branch { get; set; }
        public string? Address { get; set; }
        public string? Contact { get; set; }
        public string? Logo { get; set; }
        public decimal? RegFee { get; set; }
        public int? Visits { get; set; }
        public int? Days { get; set; }
        public int? ReservedTokens { get; set; }
        public string? Status { get; set; } = "Active";
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public string? PrintContactNo { get; set; }
        public string? WebSiteLink { get; set; }



    }
    public class HospitalProfile
    {
        public int Id { get; set; }
        public IFormFile? Logo { get; set; }
    }
}
