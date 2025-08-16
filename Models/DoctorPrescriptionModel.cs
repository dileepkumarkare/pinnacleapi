using DevExpress.DataProcessing.InMemoryDataProcessor;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.X509;
using DevExpress.Printing.ExportHelpers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32.SafeHandles;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Serilog;
using static DevExpress.CodeParser.CodeStyle.Formatting.Rules;

namespace Pinnacle.Models
{
    public class DoctorPrescriptionModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();
        public Ret SaveDoctorPrescription(DoctorPrescription entity, JwtStatus jwtData)
        {
            try
            {
                var patient = db.OpConsultation.Where(c => c.ConsultationId == entity.ConsultationId).Select(c => c.PatientId).FirstOrDefault();
                if (!db.DoctorPrescription.Any(dp => dp.ConsultationId == entity.ConsultationId))
                {
                    entity.CreatedBy = jwtData.Id;
                    db.DoctorPrescription.Add(entity);
                    db.SaveChanges();
                    var _patienExists = db.Patient.Where(patient => patient.PatientId == patient.PatientId).FirstOrDefault();
                    if (_patienExists != null)
                    {
                        _patienExists.PatientProfile = (entity != null && !string.IsNullOrEmpty(entity.ProfilePic)) ? Convert.FromBase64String(entity.ProfilePic) : _patienExists.PatientProfile;
                        db.Patient.Update(_patienExists);
                        db.SaveChanges();
                    }
                    return new Ret { status = true, message = "Prescription saved successfully" };

                }
                else
                {
                    var _existingDp = db.DoctorPrescription.FirstOrDefault(dp => dp.ConsultationId.Equals(Convert.ToInt32(entity.ConsultationId)));
                    if (_existingDp is not null)
                    {
                        _existingDp.PastHistory = entity.PastHistory;
                        _existingDp.Height = entity.Height;
                        _existingDp.Weight = entity.Weight;
                        _existingDp.BloodPressure = entity.BloodPressure != null ? entity.BloodPressure.Trim() : "";
                        _existingDp.Pulse = entity.Pulse;
                        _existingDp.Temperature = entity.Temperature != null ? entity.Temperature.Trim() : "";
                        _existingDp.Spo2 = entity.Spo2 != null ? entity.Spo2.Trim() : "";
                        _existingDp.RespiratoryRate = entity.RespiratoryRate != null ? entity.RespiratoryRate.Trim() : "";
                        _existingDp.ChiefComplaint = entity.ChiefComplaint != null ? entity.ChiefComplaint.Trim() : "";
                        _existingDp.HistoryofPresentIllness = entity.HistoryofPresentIllness != null ? entity.HistoryofPresentIllness.Trim() : "";
                        _existingDp.PhysicalExamination = entity.PhysicalExamination != null ? entity.PhysicalExamination.Trim() : "";
                        _existingDp.ClinicalNotes = entity.ClinicalNotes != null ? entity.ClinicalNotes.Trim() : "";
                        _existingDp.Investigations = entity.Investigations != null ? entity.Investigations.Trim() : "";
                        _existingDp.ProvisionalDiagnosis = entity.ProvisionalDiagnosis != null ? entity.ProvisionalDiagnosis.Trim() : "";
                        _existingDp.Medication = entity.Medication != null ? entity.Medication.Trim() : "";
                        _existingDp.Allergies = entity.Allergies != null ? entity.Allergies.Trim() : "";
                        _existingDp.Advice = entity.Advice != null ? entity.Advice.Trim() : "";
                        _existingDp.FollowUpDate = entity.FollowUpDate != null ? entity.FollowUpDate.Trim() : "";
                        _existingDp.ModifyBy = jwtData.Id;
                        _existingDp.ModifyDate = DateTime.Now;
                        _existingDp.IsVitalsPrint = entity.IsVitalsPrint?.ToString() ?? "No";
                        _existingDp.IsChiefComplaintPrint = entity.IsChiefComplaintPrint?.ToString() ?? "No";
                        _existingDp.IsPastHistoryPrint = entity.IsPastHistoryPrint?.ToString() ?? "No";
                        _existingDp.IsHistoryofPresentIllnessPrint = entity.IsHistoryofPresentIllnessPrint?.ToString() ?? "No";
                        _existingDp.IsPhysicalExaminationPrint = entity.IsPhysicalExaminationPrint?.ToString() ?? "No";
                        _existingDp.IsClinicalNotesPrint = entity.IsClinicalNotesPrint?.ToString() ?? "No";
                        _existingDp.IsInvestigationsPrint = entity.IsInvestigationsPrint?.ToString() ?? "No";
                        _existingDp.IsProvisionalDiagnosisPrint = entity.IsProvisionalDiagnosisPrint?.ToString() ?? "No";
                        _existingDp.IsMedicinesPrint = entity.IsMedicinesPrint?.ToString() ?? "No";
                        _existingDp.isAllergiesPrint = entity.isAllergiesPrint?.ToString() ?? "No";
                        _existingDp.isFollowUpDatePrint = entity.isFollowUpDatePrint?.ToString() ?? "No";
                        _existingDp.HTN = entity.HTN?.ToString() ?? "";
                        _existingDp.DM = entity.DM?.ToString() ?? "";
                        _existingDp.IHD = entity.IHD?.ToString() ?? "";
                        _existingDp.Others = entity.Others?.ToString() ?? "";
                        _existingDp.InvFindings = entity.InvFindings?.ToString() ?? "";
                        _existingDp.SurgicalAdvice = entity.SurgicalAdvice?.ToString() ?? "";
                        _existingDp.Smoking = entity.Smoking?.ToString() ?? "";
                        _existingDp.Alcohol = entity.Alcohol?.ToString() ?? "";
                        _existingDp.CVS = entity.CVS?.ToString() ?? "";
                        _existingDp.FamilyHistory = entity.FamilyHistory?.ToString() ?? "";
                        _existingDp.NxtVisitTests = entity.NxtVisitTests?.ToString() ?? "";
                        _existingDp.CCDoctorId = entity.CCDoctorId != null ? Convert.ToInt32(entity.CCDoctorId) : 0;
                        _existingDp.CCReason = entity.CCReason?.ToString() ?? "";
                        _existingDp.Diagnosis = entity.Diagnosis?.ToString() ?? "";
                        _existingDp.AddInvestNotes = entity.AddInvestNotes?.ToString() ?? "";
                        _existingDp.AddMedNotes = entity.AddMedNotes?.ToString() ?? "";
                        _existingDp.FurtherworkPlan = entity.FurtherworkPlan?.ToString() ?? "";
                        _existingDp.HyperThyroid = entity.HyperThyroid?.ToString() ?? "";
                        db.DoctorPrescription.Update(_existingDp);
                        db.SaveChanges();

                    }
                    var _patienExists = db.Patient.Where(patient => patient.PatientId == patient.PatientId).FirstOrDefault();
                    if (_patienExists != null)
                    {
                        _patienExists.PatientProfile = (entity != null && !string.IsNullOrEmpty(entity.ProfilePic)) ? Convert.FromBase64String(entity.ProfilePic) : _patienExists.PatientProfile;
                        db.Patient.Update(_patienExists);
                        db.SaveChanges();
                    }
                    return new Ret { status = true, message = "Prescription updated successfully" };
                }

            }
            catch (Exception ex)
            {
                Log.Information("Doctor Prescription=> save prescription error at " + DateTime.Now.ToString() + " message" + ex.Message);
                return new Ret { status = false, message = "Something went wrong." };
            }
        }

        public Ret GetDoctorPrescription(int Id)
        {
            try
            {
                var res = db.DoctorPrescription.FirstOrDefault(x => x.ConsultationId == Id);
                if (res is not null)
                {
                    return new Ret { status = true, data = res, message = "Prescription loaded successfully." };
                }
                return new Ret { status = false, message = "Prescription not available" };
            }
            catch (Exception ex)
            {
                Log.Information("Doctor Prescription=> Get doctor prescription at " + DateTime.Now.ToString() + " message " + ex.Message);
                return new Ret { status = false, message = "Something went wrong." };
            }
        }
        public Ret GetPatientEHR(int Id)
        {
            try
            {
                var res = (from a in db.DoctorPrescription
                           join b in db.OpConsultation on a.ConsultationId equals b.ConsultationId
                           join c in db.Doctors on b.DoctorId equals c.DoctorId
                           join d in db.Department on c.DepartmentId equals d.DepartmentId
                           where a.PatientId == Id
                           orderby a.Id descending
                           select new
                           {
                               a.Id,
                               a.ConsultationId,
                               a.PatientId,
                               a.Height,
                               a.Weight,
                               a.BloodPressure,
                               a.Pulse,
                               a.Temperature,
                               a.Spo2,
                               a.RespiratoryRate,
                               a.ChiefComplaint,
                               a.HistoryofPresentIllness,
                               a.ClinicalNotes,
                               a.Investigations,
                               a.ProvisionalDiagnosis,
                               a.Medication,
                               a.Allergies,
                               a.Advice,
                               a.FollowUpDate,
                               a.CreatedBy,
                               a.CreatedDate,
                               a.ModifyBy,
                               a.ModifyDate,
                               a.PhysicalExamination,
                               a.PastHistory,
                               a.IsVitalsPrint,
                               a.IsChiefComplaintPrint,
                               a.IsPastHistoryPrint,
                               a.IsHistoryofPresentIllnessPrint,
                               a.IsPhysicalExaminationPrint,
                               a.IsClinicalNotesPrint,
                               a.IsInvestigationsPrint,
                               a.IsProvisionalDiagnosisPrint,
                               a.IsMedicinesPrint,
                               a.isAllergiesPrint,
                               a.isFollowUpDatePrint,
                               a.HTN,
                               a.DM,
                               a.IHD,
                               a.Others,
                               a.Smoking,
                               a.Alcohol,
                               a.SurgicalAdvice,
                               a.InvFindings,
                               a.CVS,
                               a.FamilyHistory,
                               a.NxtVisitTests,
                               a.CCDoctorId,
                               a.CCReason,
                               a.IsTreatmentStart,
                               a.IsApproved,
                               a.ApprovedDate,
                               a.ApprovedBy,
                               a.Diagnosis,
                               DoctorName = c.Title + c.DoctorName,
                               d.DepartmentName,
                               c.DoctorCode,
                               a.AddMedNotes,
                               a.AddInvestNotes,
                               a.FurtherworkPlan,
                               b.DoctorId,
                               a.HyperThyroid
                           }).
                          AsNoTracking().ToList();
                if (res is not null)
                {
                    return new Ret { status = true, data = res, message = "Prescription loaded successfully." };
                }
                return new Ret { status = false, message = "Prescription not available" };
            }
            catch (Exception ex)
            {
                Log.Information("Doctor Prescription=> Get doctor prescription at " + DateTime.Now.ToString() + " message " + ex.Message);
                return new Ret { status = false, message = "Something went wrong." };
            }
        }
        public Ret PrescriptionStatus(PrescriptionStatus entity, JwtStatus jwtData)
        {
            try
            {
                if (!db.DoctorPrescription.Any(dp => dp.ConsultationId == entity.ConsultationId) && entity.ConsultationId != null)
                {
                    DoctorPrescription doctorPrescription = new DoctorPrescription();
                    doctorPrescription.IsTreatmentStart = entity.IsTreatmentStart;
                    db.DoctorPrescription.Add(doctorPrescription);
                    db.SaveChanges();
                    return new Ret { status = true, message = "Treatment started successfully.", data = new { ConsultationId = doctorPrescription.ConsultationId, IsTreamtmentStart = doctorPrescription.IsTreatmentStart } };
                }
                else if (entity.ConsultationId != null && entity.IsApproved != null)
                {
                    var docprescription = db.DoctorPrescription.Where(dp => dp.ConsultationId == entity.ConsultationId).FirstOrDefault();
                    if (docprescription != null)
                        if (docprescription is DoctorPrescription)
                        {
                            docprescription.IsApproved = entity.IsApproved;
                            docprescription.ApprovedBy = jwtData.Id;
                            docprescription.ApprovedDate = DateTime.UtcNow;
                            db.DoctorPrescription.Update(docprescription);
                            db.SaveChanges();
                            return new Ret { status = true, message = "Prescription submitted successfully.", data = new { ConsultationId = docprescription.ConsultationId, IsTreamtmentStart = docprescription.IsApproved } };
                        }
                }
                return new Ret { status = false, message = entity.IsTreatmentStart == "Yes" ? "Failed to start the prescription." : "Failed to start the prescription." };

            }
            catch (Exception ex)
            {
                Log.Information("DoctorPrescription=>PrescriptionStatus error at" + DateTime.UtcNow.ToString() + " Message " + ex.Message);
                return new Ret { status = false, message = "Something went wrong, Please contact to IT Admin." };
            }
        }

    }
}
