using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pinnacle.Entities
{
    public class ServiceGroupEntity
    {
        [Key]
        public int ServiceGroupId { get; set; }
        public string? ServiceGroupCode { get; set; }
        public string? ServiceGroupName { get; set; }
        public int? DepartmentId { get; set; }
        public int? HospitalId { get; set; }
        public string? IncludeInvestigation { get; set; }
        public string? SampleReq { get; set; }
        public string? IsAccessoinNoReq { get; set; }
        public string? AutomationRadiologyPost { get; set; }
        public string? UpdBtnInVerification { get; set; }
        public string? UpdBtnInApproval { get; set; }
        public string? BarcodePrintReq { get; set; }
        public string? GrossingandSubmitting { get; set; }
        public string? ReqAutoReportDispatch { get; set; }
        public string? ReqVerification { get; set; }
        public string? ReqDispatching { get; set; }
        public string? ReqApproval { get; set; }
        public string? ReqDigitalSign { get; set; }
        public string? ResultEntry { get; set; }
        public string? Verification { get; set; }
        public string? Approval { get; set; }
        public string? IsActive { get; set; } = "Yes";
        public string? S1 { get; set; }
        public string? S2 { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [NotMapped]
        public string? ReportTitle { get; set; }
        [NotMapped]
        public string? DoctorSignCaption { get; set; }
        [NotMapped]
        public string? Suggessions { get; set; }
        [NotMapped]
        public string? Note1 { get; set; }
        [NotMapped]
        public string? Note2 { get; set; }
        [NotMapped]
        public string? ParameterCap { get; set; }
        [NotMapped]
        public string? ResultValueCap { get; set; }
        [NotMapped]
        public string? NormalValueCap { get; set; }
        [NotMapped]
        public string? MethodCap { get; set; }
        [NotMapped]
        public string? UOMCap { get; set; }
        [NotMapped]
        public string? BarcodePrefix { get; set; }
    }
    public class LabReportingSettings
    {
        [Key]
        public int Id { get; set; }
        public int? ServiceGroupId { get; set; }
        public string? ReportTitle { get; set; }
        public string? DoctorSignCaption { get; set; }
        public string? Suggessions { get; set; }
        public string? Note1 { get; set; }
        public string? Note2 { get; set; }
        public string? ParameterCap { get; set; }
        public string? ResultValueCap { get; set; }
        public string? NormalValueCap { get; set; }
        public string? MethodCap { get; set; }
        public string? UOMCap { get; set; }
        public string? BarcodePrefix { get; set; }
    }
}
