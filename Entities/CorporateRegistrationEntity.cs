using DevExpress.CodeParser;
using DevExpress.Office;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class CorporateRegistrationEntity
    {
        [Key]

        public int CoRegId { get; set; }
        public int? OrganizationId { get; set; }
        public int? PatientId { get; set; }
        public int? HospitalId { get; set; }
        public string? MedicalCardNo { get; set; }
        public DateTime? CardValidUpto { get; set; }
        public string? RelationToEmp { get; set; }
        public string? RelationType { get; set; }
        public string? EmpNo { get; set; }
        public string? EmpName { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Remarks { get; set; }
    }
    public class CoLetterDetailsEntity
    {
        [Key]
        public int LetterId { get; set; }
        public string? RefNo { get; set; }
        public int? CoRegId { get; set; }
        public string? RefLetterNo { get; set; }
        public DateTime? LetterDate { get; set; }
        public DateTime? LetterValidUpto { get; set; }
        public string? LetterFor { get; set; }
        public string? LetterFileName { get; set; }
        public string? PurposeofRef { get; set; }
        public string? LetterIssueBy { get; set; }
        public DateTime? RefNoValidUpto { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Remarks { get; set; }

    }
    public class LetterUpload
    {
        public int letterId { get; set; }
        public string? refNo { get; set; }
        public int? coRegId { get; set; }
        public string? refLetterNo { get; set; }
        public DateTime? letterDate { get; set; }
        public DateTime? letterValidUpto { get; set; }
        public string? letterFor { get; set; }
        public string? purposeofRef { get; set; }
        public string? letterIssueBy { get; set; }
        public DateTime? refNoValidUpto { get; set; }
        public int? maxCredLimit { get; set; } = 0;
        public string? remarks { get; set; }
        public IFormFile? File { get; set; }
    }
}