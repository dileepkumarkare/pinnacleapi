using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pinnacle.Entities
{
    public class ReferralEntity
    {
        [Key]
        public int Id { get; set; }
        public string ReferralType { get; set; }
        public string? ReferenceCode { get; set; }
        public string ReferenceName { get; set; }
        public string? AliasName { get; set; }
        public int HospitalId { get; set; }
        public int? Specialization { get; set; }
        public int? Designation { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime? MarriageDate { get; set; }
        public int? PROCode { get; set; }
        public int? AreaCode { get; set; }
        public DateTime? ActiveDate { get; set; }
        public DateTime? DeactiveDate { get; set; }
        public string? IsPaymentRequired { get; set; }
        public string? IsActive { get; set; } = "Yes";
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? ActivateBy { get; set; }
        public DateTime? ActivateDate { get; set; }
        public int? DeactivateBy { get; set; }
        public DateTime? DeactivatedDate { get; set; }
    }

    public class ReferralPercentage
    {
        public int Id { get; set; }
        public int? ReferralId { get; set; }
        public decimal? InPatient { get; set; }
        public decimal? Investigations { get; set; }
        public decimal? OpConsultations { get; set; }
        public string? PAN { get; set; }
        public string? ACNo { get; set; }

    }

}