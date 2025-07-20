using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class DesignationEntity
    {
        [Key]
        public int DesignationId { get; set; }
        public string? DesignationCode { get; set; }
        public string DesignationName { get; set; }
        public string? Status { get; set; } = "Active";
        public int? AddedBy { get; set; }
        public int? HospitalId { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
    public class TitleEntity
    {
        [Key]
        public int TitleId { get; set; }
        public string Title { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
    }
}
