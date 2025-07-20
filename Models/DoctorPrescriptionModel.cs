using DevExpress.DataProcessing.InMemoryDataProcessor;
using Microsoft.EntityFrameworkCore;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Serilog;

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
                var res = db.DoctorPrescription.Where(x => x.PatientId == Id).AsNoTracking().ToList();
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

    }
}
