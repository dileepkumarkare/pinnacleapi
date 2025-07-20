using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class DepartmentEntity
    {
        [Key]
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? DepartmentCode { get; set; }
        public string? InCHargeCode {  get; set; }
        public string? ContactOne { get; set; }
        public string? ContactTwo { get; set; }
        public string? CostCenterCd { get; set; }
        public string? PrimarySpecialzation { get; set; }
        public string? SpecialzationOne { get; set; }
        public string? SpecialzationTwo { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifyDate { get; set; }
        public string? IsMedical { get; set; }
        public string? MedicalType { get; set; }
        public int? HospitalId { get; set; }
    }
}
