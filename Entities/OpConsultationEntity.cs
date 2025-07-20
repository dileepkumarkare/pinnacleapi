using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace Pinnacle.Entities
{
    public class OpConsultationCollection
    {
        public int ConsultationId { get; set; }
        public string? ConsultationNo { get; set; }
        public int? PatientId { get; set; }
        public string? RefNo { get; set; }
        public DateTime? ConsultationDate { get; set; }
        public DateTime? LastValidityDate { get; set; }
        public string? VisitType { get; set; }
        public string? PaymentBy { get; set; }
        public int? OrganizationId { get; set; }
        public int? DoctorId { get; set; }
        public decimal? ConsultantFee { get; set; }
        public int? RemainingVisits { get; set; }
        public decimal? Discount { get; set; }
        public int? Visit { get; set; }
        public int? TokenNo { get; set; }
        public string? PaymentStatus { get; set; } = "Pending";
        public string? ReferralType { get; set; }
        public string? Status { get; set; } = "Booked";
        public string? ReferredBy { get; set; }
        public int? CreatedBy { get; set; }
        public int? HospitalId { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public string? LetterFileName { get; set; }
        public DateTime? LetterDate { get; set; }
        public DateTime? LetterValidUpto { get; set; }
        public string? LetterRefNo { get; set; }
        public string? TeleConsultation { get; set; }
        public int? CancelBy { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? IsDocApproved { get; set; }
        public DateTime? DocApprovedDate { get; set; }
        public string? IsAudApprove { get; set; }
        public DateTime? AudApproveDate { get; set; }
        public int? ApproveDocId { get; set; }
        public int? ApproveAudId { get; set; }
        public string? CanReason { get; set; }
        public string? DocApproveRemarks { get; set; }
        public string? AudApproveRemarks { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? ReceiptNumber { get; set; }
        public decimal? RegFee { get; set; }
        public string? RecType { get; set; }
        public int? BankId { get; set; }
        public DateTime? Validity { get; set; }
        public DateTime? RecDate { get; set; }
        public string? PaymentType { get; set; }
        public string? TxnNo { get; set; }
        public string? RecRemarks { get; set; }
        public int? ReceiptId { get; set; } = 0;
        public string? Remarks { get; set; }
        public decimal? NetAmount { get; set; }
        public int? DueAuthCode { get; set; }
        public decimal? RequiredAmount { get; set; }
        public string? ConsReferral { get; set; }
        public string? ConsReferredBy { get; set; }
    }
    public class OpConsultationEntity
    {
        [Key]
        public int ConsultationId { get; set; }
        public string? ConsultationNo { get; set; }
        public int? PatientId { get; set; }
        public string? RefNo { get; set; }
        public DateTime? ConsultationDate { get; set; }
        public DateTime? LastValidityDate { get; set; }
        public string? VisitType { get; set; }
        public string? PaymentBy { get; set; }
        public int? OrganizationId { get; set; }
        public int? DoctorId { get; set; }
        public decimal? ConsultantFee { get; set; }
        public int? RemainingVisits { get; set; }
        public decimal? Discount { get; set; }
        public int? Visit { get; set; }
        public int? TokenNo { get; set; }
        public string? PaymentStatus { get; set; } = "Pending";
        public string? ReferralType { get; set; }
        public string? Status { get; set; } = "Booked";
        public string? ReferredBy { get; set; }
        public int? CreatedBy { get; set; }
        public int? HospitalId { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public string? TeleConsultation { get; set; }
        public int? CancelBy { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? IsDocApproved { get; set; } = "No";
        public DateTime? DocApprovedDate { get; set; }
        public string? IsAudApprove { get; set; } = "No";
        public DateTime? AudApproveDate { get; set; }
        public int? ApproveDocId { get; set; }
        public int? ApproveAudId { get; set; }
        public string? CanReason { get; set; }
        public string? DocApproveRemarks { get; set; }
        public string? AudApproveRemarks { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
    public class OpFileUpload
    {
        public int? ConsultationId { get; set; }
        public string? ConsultationNo { get; set; }
        public IFormFile? File { get; set; }

    }

    public class ApprovalEntity
    {
        public int Id { get; set; }
        public string? Status { get; set; }
        public string? CanReason { get; set; }
        public string? Remarks { get; set; }
    }
}
