using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class ItemFormCodesEntity
    {
        [Key]
        public int Id { get; set; }
        public string? FormCode { get; set; }
        public string? FormCodeDesc { get; set; }
        public string? ItemType { get; set; }
        public int? ROConsDays { get; set; }
        public int? ROMaxDays { get; set; }
        public int? RORoDays { get; set; }
        public int? ROMinDays { get; set; }
        public string? SepPoFromPI { get; set; }
        public int? HospitalId { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
