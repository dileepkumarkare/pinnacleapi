using DevExpress.XtraRichEdit.Model;
using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class ItemMasterEntity
    {
        [Key]
        public int ItemId { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemDesc { get; set; }
        public string? ItemType { get; set; }
        public string? IsActive { get; set; }
        public int? HospitalId { get; set; }
        public string? IsFavorite { get; set; }
        public int? FormCodeId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? GenericCodeId { get; set; }
    }
    public class LiveInventory
    {
        public string? ItemCd { get; set; }
        public double? OnHandQty { get; set; }
    }
}
