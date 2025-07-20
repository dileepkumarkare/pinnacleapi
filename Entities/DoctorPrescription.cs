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
    }
    public class PrescriptionFilter
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
