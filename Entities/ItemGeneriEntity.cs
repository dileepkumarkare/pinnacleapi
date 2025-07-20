using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.XtraRichEdit.Model;

namespace Pinnacle.Entities
{
    public class ItemGenericEntity
    {
        public int Id { get; set; }
        public string? GenericCode { get; set; }
        public string? GenericDesc { get; set; }
        public int? GroupCodeId { get; set; }
        public string? ItemType { get; set; }
        public string? IsFormulary { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? HospitalId { get; set; }
    }
}
