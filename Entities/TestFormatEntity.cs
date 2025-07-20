using DevExpress.XtraRichEdit.Model;
using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class TestFormatEntity
    {
        [Key]
        public int Id { get; set; }
        public int? GroupId { get; set; }
        public int? TestId { get; set; }
        public string? FormatCode { get; set; }
        public string? LabEquName { get; set; }
        public string? ReportTitle { get; set; }
        public int? HospitalId { get; set; }
        public string? Gender { get; set; }
        public string? IsGenderSpecific { get; set; }
        public string? IsSampleNeed { get; set; }
        public string? IsDefaultFormat { get; set; }
        public string? IsGrowth { get; set; }
        public string? Specimen { get; set; }
        public string? ColCap1 { get; set; }
        public string? ColCap2 { get; set; }
        public string? ColCap3 { get; set; }
        public string? ColCap4 { get; set; }
        public string? MinTime { get; set; }
        public string? MaxTime { get; set; }
        public string? DoctorId { get; set; }
        public string? IsAccNeeded { get; set; }
        public string? IsCriticalHistory { get; set; }
        public string? IsNoNormalRnages { get; set; }
        public string? IsTemplateNeed { get; set; }
        public string? IsMulOrgNeed { get; set; }
        public string? IsBig { get; set; }
        public string? ResultAlignment { get; set; }
        public string? ResultEntry { get; set; }
        public string? ResultVerification { get; set; }
        public string? Remarks { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? ApproveBy { get; set; }
        public DateTime? ApproveDate { get; set; }
        public List<TestParameterMapping>? TestParameterMappings { get; set; }
    }
    public class TestParameterMapping
    {
        [Key]
        public int Id { get; set; }
        public string? SubTitle { get; set; }
        public int? TestId { get; set; }
        public int? ParamId { get; set; }
    }
}
