using DevExpress.XtraRichEdit.Model;

namespace Pinnacle.Entities
{
    public class VacutainerEntity
    {
        public int Id { get; set; }
        public string? VacutainerCode { get; set; }
        public string? VacutainerName { get; set; }
        public int? HospitalId { get; set; }
        public string? IsActive { get; set; } = "Yes";
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
