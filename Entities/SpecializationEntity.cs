using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class SpecializationEntity
    {
        [Key]
        public int SpecializationId { get; set; }
        public string? SpecializationCode { get; set; }
        public string SpecializationName { get; set; }
        public string? Status { get; set; } = "Active";
        public int? HospitalId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifyDate { get; set; }
    }
}
