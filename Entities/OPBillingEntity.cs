using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{

    public class OPBillingCollection
    {
        public int OpBillId { get; set; }
        public string? RefNo { get; set; }
        public string? BillType { get; set; }
        public int? ConsultationId { get; set; }
        public int? OrganizationId { get; set; }
        public int? PatientId { get; set; }
        public string? BillNo { get; set; }
        public int? DoctorId { get; set; }
        public DateTime? BillDate { get; set; }
        public string? IsFree { get; set; }
        public string? VIPSource { get; set; }
        public string? VIPRemarks { get; set; }
        public int? FreeAuthorizedBy { get; set; }
        public decimal? OverAllConcPercentage { get; set; } = 0;
        public decimal? OverAllConcAmount { get; set; } = 0;
        public decimal? GrossAmount { get; set; } = 0;
        public decimal? NetAmount { get; set; } = 0;
        public string? IsCancelled { get; set; } = "No";
        public int? CanAuthorizedBy { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string? Remarks { get; set; }
        public string? OspNo { get; set; }
        public string? PatientName { get; set; }
        public string? Email { get; set; }
        public string? ContactNo { get; set; }
        public string? Relation { get; set; }
        public string? RelationName { get; set; }
        public DateTime? Dob { get; set; }
        public string? Age { get; set; }
        public string? Gender { get; set; }
        public int? OccupationId { get; set; }
        public string? Refferal { get; set; }
        public string? ReferredBy { get; set; }
        public string? BloodGroup { get; set; }
        public string? Nationality { get; set; }
        public List<OPServiceBookingEntity>? Services { get; set; }
        public string? OpBillRecNo { get; set; }
        public DateTime? RecDate { get; set; }
        public decimal? RecAmount { get; set; }
        public decimal? Concession { get; set; }
        public int? ConcAuthorizedBy { get; set; }
        public decimal? DueAmount { get; set; }
        public int? DueAuthorizedBy { get; set; }
        public string? PaymentType { get; set; }
        public string? TxnNo { get; set; }
        public int? BankId { get; set; }
        public string? PaymentStatus { get; set; }

    }
    public class OPBillingEntity
    {
        [Key]
        public int OPBillId { get; set; }
        public string? RefNo { get; set; }
        public string? BillType { get; set; }
        public int? ConsultationId { get; set; }
        public int? OrganizationId { get; set; }
        public int? PatientId { get; set; }
        public string? BillNo { get; set; }
        public int? ConsultantId { get; set; }
        public DateTime? BillDate { get; set; }
        public string? IsFree { get; set; }
        public string? VIPSource { get; set; }
        public string? VIPRemarks { get; set; }
        public int? FreeAuthorizedBy { get; set; }
        public decimal? OverAllConcPercentage { get; set; }
        public decimal? OverAllConcAmount { get; set; }
        public decimal? GrossAmount { get; set; }
        public decimal? NetAmount { get; set; }
        public string? IsCancelled { get; set; } = "No";
        public int? CanAuthorizedBy { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string? Remarks { get; set; }
        public int? HospitalId { get; set; }
        public string? SourceofRef { get; set; }
        public int? ReferredBy { get; set; }
    }

    public class OSPatientEntity
    {
        [Key]
        public int? PatientId { get; set; }
        public string? OspNo { get; set; }
        public string? PatientName { get; set; }
        public string? Email { get; set; }
        public string? ContactNo { get; set; }
        public string? Relation { get; set; }
        public string? RelationName { get; set; }
        public DateTime? Dob { get; set; }
        public string? Age { get; set; }
        public string? Gender { get; set; }
        public int? OccupationId { get; set; }
        public string? RefSource { get; set; }
        public string? RefBy { get; set; }
        public string? BloodGroup { get; set; }
        public string? Nationality { get; set; }
    }
    public class OPServiceBookingEntity
    {
        [Key]
        public int ServiceBookingId { get; set; }
        public int? OpBillId { get; set; }
        public int? ServiceId { get; set; }
        public int? Qty { get; set; }
        public decimal? Rate { get; set; }
        public decimal? TotalAmount { get; set; }
    }
    public class OPBillReceiptEntity
    {
        [Key]
        public int OpBillRecId { get; set; }
        public string? OpBillRecNo { get; set; }
        public int? OpBillId { get; set; }
        public DateTime? RecDate { get; set; }
        public decimal? RecAmount { get; set; }
        public decimal? Concession { get; set; }
        public int? ConcAuthorizedBy { get; set; }
        public decimal? DueAmount { get; set; }
        public int? DueAuthorizedBy { get; set; }
        public string? PaymentType { get; set; }
        public string? TxnNo { get; set; }
        public int? BankId { get; set; }
        public string? PaymentStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
    public class OPBillingFilter
    {
        public int? Id { get; set; }
        public string? PatientType { get; set; }
        public int? OrganizationId { get; set; }
        public int? ServiceGroupId { get; set; }
        public string? RefNo { get; set; }
        public string? ConsultationNo { get; set; }

    }

    public class OpTariffServicesVertical
    {
        public int? Priority { get; set; }
        public int? TariffId { get; set; }
        public decimal? Discount { get; set; }
    }
    public class Investigations
    {
        public int? LabTestId { get; set; }
        public string? TestName { get; set; }
    }
    public class ServiceCharges
    {
        public int? Value { get; set; }
        public string? Label { get; set; }
        public string? ServiceCode { get; set; }
        public double? Rate { get; set; }
        public string? IsGeneral { get; set; }
    }
}