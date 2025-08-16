using System.ComponentModel.DataAnnotations.Schema;

namespace Pinnacle.Entities
{
    public class DoctorPrescription
    {
        public int Id { get; set; }
        public int? ConsultationId { get; set; }
        public int? PatientId { get; set; }
        public string? PastHistory { get; set; }
        public string? Height { get; set; }
        public string? Weight { get; set; }
        public string? BloodPressure { get; set; }
        public string? Pulse { get; set; }
        public string? Temperature { get; set; }
        public string? Spo2 { get; set; }
        public string? RespiratoryRate { get; set; }
        public string? ChiefComplaint { get; set; }
        public string? HistoryofPresentIllness { get; set; }
        public string? PhysicalExamination { get; set; }
        public string? ClinicalNotes { get; set; }
        public string? Investigations { get; set; }
        public string? ProvisionalDiagnosis { get; set; }
        public string? Medication { get; set; }
        public string? Allergies { get; set; }
        public string? Advice { get; set; }
        public string? FollowUpDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        [NotMapped]
        public dynamic? ProfilePic { get; set; }
        public string? IsVitalsPrint { get; set; } = "Yes";
        public string? IsChiefComplaintPrint { get; set; } = "Yes";
        public string? IsPastHistoryPrint { get; set; } = "Yes";
        public string? IsHistoryofPresentIllnessPrint { get; set; } = "Yes";
        public string? IsPhysicalExaminationPrint { get; set; } = "Yes";
        public string? IsClinicalNotesPrint { get; set; } = "Yes";
        public string? IsInvestigationsPrint { get; set; } = "Yes";
        public string? IsProvisionalDiagnosisPrint { get; set; } = "Yes";
        public string? IsMedicinesPrint { get; set; } = "Yes";
        public string? isAllergiesPrint { get; set; } = "Yes";
        public string? isFollowUpDatePrint { get; set; } = "Yes";
        public string? Smoking { get; set; }
        public string? Alcohol { get; set; }
        public string? HTN { get; set; }
        public string? DM { get; set; }
        public string? IHD { get; set; }
        public string? Others { get; set; }
        public string? InvFindings { get; set; }
        public string? SurgicalAdvice { get; set; }
        public string? CVS { get; set; }
        public string? FamilyHistory { get; set; }
        public string? NxtVisitTests { get; set; }
        public int? CCDoctorId { get; set; }
        public string? CCReason { get; set; }
        public string? IsTreatmentStart { get; set; }
        public string? IsApproved { get; set; }
        public DateTime? ApprovedDate { get; set; } = DateTime.UtcNow;
        public int? ApprovedBy { get; set; }
        public string? Diagnosis { get; set; }
        public string? AddInvestNotes { get; set; }
        public string? AddMedNotes { get; set; }
        public string? FurtherworkPlan { get; set; }
        public string? HyperThyroid { get; set; }
    }
    public class PrescriptionFilter
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

    }
    public class PrescriptionStatus
    {
        public int? ConsultationId { get; set; }
        public string? IsTreatmentStart { get; set; }
        public string? IsApproved { get; set; }
    }

}
