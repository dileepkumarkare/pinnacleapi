using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Pinnacle.Entities
{
    public class PatientEntity
    {
        [Key]
        public int PatientId { get; set; }
        public string? PatientAdmissionType { get; set; }
        public string? Vip { get; set; }
        public string? NewBorn { get; set; }
        public string? PatientType { get; set; }
        public string? Age { get; set; }
        public string? MaritalStatus { get; set; }
        public string? Relation { get; set; }
        public string? RelationName { get; set; }
        public string? Nationality { get; set; }
        public string? BloodGroup { get; set; }
        public int? OccupationId { get; set; }
        public string? Refferal { get; set; }
        public int? ReferredBy { get; set; }
        public string? VipSour { get; set; }
        public string? Remarks { get; set; }
        public int? DoctorId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public int UserId { get; set; }

        public string? PassportNo { get; set; }
        public int? PCountryId { get; set; }
        public DateTime? PIssueDate { get; set; }
        public DateTime? PExpiryDate { get; set; }
        public string? VisaNo { get; set; }
        public int? VCountryId { get; set; }
        public DateTime? VIssueDate { get; set; }
        public DateTime? VExpiryDate { get; set; }

        public byte[]? PatientProfile { get; set; }
        public string? PrimarycontactNo { get; set; }
        [NotMapped]
        public dynamic? ProfilePic { get; set; }

        [NotMapped]
        public string? UMRNumber { get; set; }
        [NotMapped]
        public string? Email { get; set; }
        [NotMapped]
        public string? ContactNo { get; set; }
        [NotMapped]
        public string? PatientName { get; set; }
        [NotMapped]
        public string? DOB { get; set; }
        [NotMapped]
        public string? Gender { get; set; }
        [NotMapped]
        public int? TitleId { get; set; }
        [NotMapped]
        public int? HospitalId { get; set; }
        [NotMapped]
        public int? AreaCode { get; set; }
        [NotMapped]
        public string? Address { get; set; }
        [NotMapped]
        public string? IdProof { get; set; }
        [NotMapped]
        public string? IdNumber { get; set; }
        [NotMapped]
        public string? GovtId1 { get; set; }
        [NotMapped]
        public string? GovtIdNumber1 { get; set; }
        [NotMapped]
        public string? GovtId2 { get; set; }
        [NotMapped]
        public string? GovtIdNumber2 { get; set; }
        [NotMapped]
        public string? Country { get; set; }
        [NotMapped]
        public string? Area { get; set; }
        [NotMapped]
        public string? ReceiptNumber { get; set; }
        [NotMapped]
        public decimal? RegFee { get; set; }
        [NotMapped]
        public string? RecType { get; set; }
        [NotMapped]
        public int? BankId { get; set; }
        [NotMapped]
        public DateTime? Validity { get; set; }
        [NotMapped]
        public DateTime? RecDate { get; set; }
        [NotMapped]
        public string? PaymentType { get; set; }
        [NotMapped]
        public string? TxnNo { get; set; }
        [NotMapped]
        public string? RecRemarks { get; set; }
        [NotMapped]
        public int? ReceiptId { get; set; } = 0;
        [NotMapped]
        public decimal? Discount { get; set; }
        [NotMapped]
        public decimal? NetAmount { get; set; }
        [NotMapped]
        public int? DueAuthCode { get; set; }
        [NotMapped]
        public decimal? RequiredAmount { get; set; }
        [NotMapped]
        public int? OrganizationId { get; set; }

    }
    public class ReligionEntity
    {
        [Key]
        public int ReligionId { get; set; }
        public string? ReligionName { get; set; }

    }
    public class PatientRegistration
    {
        [Key]
        public int PatientRegistartionId { get; set; }
        public int? PatientId { get; set; }
        public string? RegistrationNo { get; set; }
        public string? RegistrationDate { get; set; }
        public string? RegistrationValidityDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

    }
    public class PatientAddress
    {
        [Key]
        public int PAddressId { get; set; }
        public int? PatientId { get; set; }
        public int? AreaCode { get; set; }
        public string? Address { get; set; }
        public string? IdProof { get; set; }
        public string? IdNumber { get; set; }
        public string? GovtId1 { get; set; }
        public string? GovtIdNumber1 { get; set; }
        public string? GovtId2 { get; set; }
        public string? GovtIdNumber2 { get; set; }
        public string? Country { get; set; }
        public string? Area { get; set; }
    }
    public class PatientReceiptDetailsEntity
    {
        [Key]
        public int ReceiptId { get; set; }
        public int? PCId { get; set; }
        public string? ReceiptNumber { get; set; }
        public decimal? RegFee { get; set; }
        public string? RecType { get; set; }
        public int? BankId { get; set; }
        public DateTime? Validity { get; set; }
        public DateTime? RecDate { get; set; }
        public string? PaymentType { get; set; }
        public string? TxnNo { get; set; }
        public string? RecRemarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public decimal? Discount { get; set; }
        public decimal? NetAmount { get; set; }
        public int? DueAuthCode { get; set; }
        public decimal? RequiredAmount { get; set; }
    }
}