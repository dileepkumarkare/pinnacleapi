using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class OccupationEntity
    {
        [Key]
        public int OccupationId { get; set; }
        public string? OccupationCode { get; set; }
        public string? OccupationName { get; set; }
        public int? HospitalId { get; set; }
        public string? Status { get; set; } = "Active";
        public int? AddedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
    }
}
