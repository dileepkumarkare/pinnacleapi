using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Pinnacle.Entities
{
    public class OrganizationEntity
    {
        [Key]
        public int OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public string? OrganizationCode { get; set; }
        public string? OrganizationType { get; set; }
        public int? HospitalId { get; set; }
        public string? ContactNo { get; set; }
        public DateTime? ContactDate { get; set; }
        public string? ContactPerson { get; set; }
        public string? Color { get; set; }
        public DateTime? EffectFrom { get; set; }
        public DateTime? EffectTo { get; set; }
        public string? AuthorizedPerson { get; set; }
        public string? PanNumber { get; set; }
        public string? TanNumber { get; set; }
        public string? GSTCode { get; set; }
        public decimal? OpOrgPercentage { get; set; }
        public decimal? OpEmpPercentage { get; set; }
        public decimal? IpOrgPercentage { get; set; }
        public decimal? IpEmpPercentage { get; set; }
        public int? OpNos { get; set; }
        public int? OpDays { get; set; }
        public decimal? OpDisc { get; set; }
        public int? OpDaysForVisit { get; set; }
        public int? IpNos { get; set; }
        public int? IpDays { get; set; }
        public decimal? IpDisc { get; set; }
        public int? IpDaysForVisit { get; set; }
        public string? Pharmacy { get; set; }
        public decimal? PharmacyDisc { get; set; }
        public string? TariffPriorityFor { get; set; }
        public string? CorpConsultation { get; set; }
        public string? IsActive { get; set; } = "Yes";
        public int? AddedBy { get; set; }
        public string? Category { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [NotMapped]
        public int? IpPriorityOne { get; set; }
        [NotMapped]
        public decimal? IpPriorityOneDisc { get; set; }
        [NotMapped]
        public int? IpPriorityTwo { get; set; }
        [NotMapped]
        public decimal? IpPriorityTwoDisc { get; set; }
        [NotMapped]
        public int? IpPriorityThree { get; set; }
        [NotMapped]
        public decimal? IpPriorityThreeDisc { get; set; }
        [NotMapped]
        public int? IpPriorityDefault { get; set; }
        [NotMapped]
        public decimal? IpPriorityDefaultDisc { get; set; }
        [NotMapped]
        public int? OpPriorityOne { get; set; }
        [NotMapped]
        public decimal? OpPriorityOneDisc { get; set; }
        [NotMapped]
        public int? OpPriorityTwo { get; set; }
        [NotMapped]
        public decimal? OpPriorityTwoDisc { get; set; }
        [NotMapped]
        public int? OpPriorityThree { get; set; }
        [NotMapped]
        public decimal? OpPriorityThreeDisc { get; set; }
        [NotMapped]
        public int? OpPriorityDefault { get; set; }
        [NotMapped]
        public decimal? OpPriorityDefaultDisc { get; set; }
        [NotMapped]
        public int? CorporateBillDoneIn { get; set; }
        [NotMapped]
        public int? SubmitToMktgDeptIn { get; set; }
        [NotMapped]
        public int? SubmitToOrganizationIn { get; set; }
        [NotMapped]
        public int? SummaryApprovalDays { get; set; }
        [NotMapped]
        public int? BillClearanceTime { get; set; }
        [NotMapped]
        public string? Area { get; set; }
        [NotMapped]
        public string? Address { get; set; }
        [NotMapped]
        public int? AreaCode { get; set; }
        [NotMapped]
        public string? Country { get; set; }
    }
    public class OrganizationTariff
    {
        [Key]
        public int Id { get; set; }
        public int? OrganizationId { get; set; }
        public int? IpPriorityOne { get; set; }
        public decimal? IpPriorityOneDisc { get; set; }
        public int? IpPriorityTwo { get; set; }
        public decimal? IpPriorityTwoDisc { get; set; }
        public int? IpPriorityThree { get; set; }
        public decimal? IpPriorityThreeDisc { get; set; }
        public int? IpPriorityDefault { get; set; }
        public decimal? IpPriorityDefaultDisc { get; set; }
        public int? OpPriorityOne { get; set; }
        public decimal? OpPriorityOneDisc { get; set; }
        public int? OpPriorityTwo { get; set; }
        public decimal? OpPriorityTwoDisc { get; set; }
        public int? OpPriorityThree { get; set; }
        public decimal? OpPriorityThreeDisc { get; set; }
        public int? OpPriorityDefault { get; set; }
        public decimal? OpPriorityDefaultDisc { get; set; }
        public int? CorporateBillDoneIn { get; set; }
        public int? SubmitToMktgDeptIn { get; set; }
        public int? SubmitToOrganizationIn { get; set; }
        public int? SummaryApprovalDays { get; set; }
        public int? BillClearanceTime { get; set; }
    }

    public class OrganizationChargeEntity
    {
        [Key]
        public int Id { get; set; }
        public string? VisitType { get; set; } = "Normal";
        public int? DoctorId { get; set; }
        public int? OrganizationId { get; set; }
        public decimal? OpCharge { get; set; }
        public decimal? IpCharge { get; set; }
        public string? IsActive { get; set; } = "Yes";
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [NotMapped]
        public List<DoctorIds>? DoctorIds { get; set; }
    }


    public class DoctorIds
    {
        public int? Value { get; set; }
        public string? Label { get; set; }
    }
    public class OrganizationAddress
    {
        public int Id { get; set; }
        public int? OrganizationId { get; set; }
        public int? AreaCode { get; set; }
        public string? Area { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
    }


}