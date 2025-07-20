using DevExpress.XtraRichEdit.Model;
using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class SpecimenEntity
    {
        [Key]
        public int Id { get; set; }
        public string? SpecimenCode { get; set; }
        public string? SpecimenName { get; set; }
        public int? HospitalId { get; set; }
        public string? IsActive { get; set; } = "Yes";
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
