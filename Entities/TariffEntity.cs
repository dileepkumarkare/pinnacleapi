using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class TariffEntity
    {

        [Key]
        public int TariffId { get; set; }
        public string? TariffCode { get; set; }
        public string? ContactPerson { get; set; }
        public string EffectFromDate { get; set; }
        public string EffectToDate { get; set; }
        public int? HospitalId { get; set; }
        public string? Status { get; set; } = "Active";
        public string? TariffName { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; } = DateTime.Now;

    }

    public class TariffServiceMapping
    {
        [Key]
        public int Id { get; set; }
        public int? TariffId { set; get; }
        //public int? ServiceGroupId { get; set; }
        public int? ServiceId { get; set; }
        public string? TariffServiceName { get; set; }
        public string? TariffServiceCode { get; set; }
        public decimal? Charge { get; set; }
        public int? CreatedBy { set; get; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { set; get; }
        public DateTime? UpdatedDate { get; set; }
        public int? HospitalId { get; set; }
    }
}
