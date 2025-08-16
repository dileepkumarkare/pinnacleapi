using DevExpress.Office.NumberConverters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;
using Pinnacle.Entities;
using Pinnacle.Helpers;
using Pinnacle.Helpers.JWT;
using Serilog;
using System.Security.AccessControl;
using System.Text.RegularExpressions;

namespace Pinnacle.Models
{
    public class OpConsultationModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();
        CommonLogic CL = new CommonLogic();
        public Ret SaveOpConsultation(OpConsultationCollection entity, JwtStatus jwtData)
        {
            try
            {
                string msg = "";
                string extension = "";

                if (entity.ConsultationId == 0)
                {

                    var reservedTokenCount = db.Doctors.Where(h => h.DoctorId == entity.DoctorId).Select(h => h.ReservedTokens).FirstOrDefault();
                    var lastToken = db.OpConsultation.AsEnumerable().Where(op => Convert.ToDateTime(op.ConsultationDate).Date == Convert.ToDateTime(op.ConsultationDate).Date && op.DoctorId == entity.DoctorId).Select(op => op.TokenNo).OrderByDescending(op => op).FirstOrDefault();
                    var _opConsultation = new OpConsultationEntity()
                    {
                        TokenNo = lastToken != null && lastToken > 0 ? Convert.ToInt32(lastToken) + 1 : reservedTokenCount != null ? Convert.ToInt32(reservedTokenCount) + 1 : 1,
                        HospitalId = entity.HospitalId != null ? entity.HospitalId : jwtData.HospitalId,
                        CreatedBy = jwtData.Id,
                        ReferralType = entity.ConsReferral,
                        ReferredBy = entity.ConsReferredBy,
                        ConsultationNo = entity.ConsultationNo,
                        PatientId = entity.PatientId,
                        RefNo = entity.RefNo,
                        ConsultationDate = entity.ConsultationDate,
                        LastValidityDate = entity.LastValidityDate,
                        VisitType = entity.VisitType,
                        PaymentBy = entity.PaymentBy,
                        OrganizationId = entity.OrganizationId,
                        DoctorId = entity.DoctorId,
                        ConsultantFee = entity.ConsultantFee,
                        Visit = entity.Visit,
                        RemainingVisits = entity.RemainingVisits,
                        PaymentStatus = entity.PaymentStatus,
                        Status = entity.Status,
                        Discount = entity.Discount,
                        TeleConsultation = entity.TeleConsultation,
                    };


                    var _existingOpConsultation = db.OpConsultation.Where(a => a.PatientId == entity.PatientId).OrderByDescending(cons => cons).FirstOrDefault();
                    db.OpConsultation.Add(_opConsultation);
                    db.SaveChanges();
                    entity.ConsultationId = _opConsultation.ConsultationId;
                    msg = "Consultation saved successfully!";
                }
                else
                {
                    var _existingOpConsultation = db.OpConsultation.FirstOrDefault(cons => cons.ConsultationId == entity.ConsultationId);
                    _existingOpConsultation.VisitType = entity.VisitType;
                    _existingOpConsultation.PaymentBy = entity.PaymentBy;
                    _existingOpConsultation.OrganizationId = entity.OrganizationId;
                    _existingOpConsultation.DoctorId = entity.DoctorId;
                    _existingOpConsultation.ConsultantFee = entity.ConsultantFee;
                    _existingOpConsultation.ReferralType = entity.ConsReferral;
                    _existingOpConsultation.ReferredBy = entity.ConsReferredBy;
                    _existingOpConsultation.Discount = entity.Discount;
                    _existingOpConsultation.UpdatedBy = jwtData.Id;
                    _existingOpConsultation.UpdatedDate = DateTime.Now;
                    msg = "Consultation updated successfully!";
                }
                db.SaveChanges();
                if (entity.ConsultationId > 0)
                {
                    SavePatientReceipt(entity, jwtData);
                }
                return new Ret { status = true, message = msg, data = new { ConsultationId = entity.ConsultationId } };
            }
            catch (Exception ex)
            {
                return new Ret();
            }
        }

        public void checkLetterValidity(int patientId, int organizationId, DateTime consultationDate, out bool letterDate, out bool consultation)
        {
            try
            {

                var coRegId = db.CorporateRegistration.Where(cor => cor.PatientId == patientId && cor.OrganizationId == organizationId).Select(cr => cr.CoRegId).OrderByDescending(cr => cr).FirstOrDefault();
                CoLetterDetailsEntity cl = db.CoLetterDetails.Where(cl => cl.CoRegId == Convert.ToInt32(coRegId)).OrderByDescending(cl => cl).FirstOrDefault();
                letterDate = Convert.ToDateTime(cl.LetterDate) >= consultationDate ? true : false;
                consultation = !cl.PurposeofRef.Contains("Consultation") ? false : true;


            }
            catch (Exception ex)
            {
                letterDate = false;
                consultation = false;
            }
        }
        public void SavePatientReceipt(OpConsultationCollection entity, JwtStatus jwtData)
        {
            try
            {

                var receiptDetails = new PatientReceiptDetailsEntity
                {
                    PCId = entity.ConsultationId,
                    ReceiptNumber = entity.ReceiptNumber,
                    RegFee = entity.ConsultantFee,
                    RecType = "Consultation",
                    BankId = entity.BankId,
                    Validity = DateTime.Now.AddYears(100).Date,
                    RecDate = DateTime.Now,
                    PaymentType = entity.PaymentType,
                    TxnNo = entity.TxnNo,
                    RecRemarks = entity.Remarks,
                    CreatedBy = jwtData.Id,
                    CreatedDate = entity.CreatedDate,
                    DueAuthCode = entity.DueAuthCode,
                    NetAmount = entity.NetAmount,
                    Discount = entity.Discount,
                    RequiredAmount = entity.RequiredAmount
                };
                if (entity.ReceiptId == 0)
                {
                    db.PatientReceiptDetails.Add(receiptDetails);
                }
                else
                {
                    var _existingRecDetails = db.PatientReceiptDetails.FirstOrDefault(rec => rec.ReceiptId == entity.ReceiptId);
                    _existingRecDetails.RegFee = entity.ConsultantFee;
                    _existingRecDetails.BankId = entity.BankId;
                    _existingRecDetails.RecDate = DateTime.Now.Date;
                    _existingRecDetails.TxnNo = entity.TxnNo;
                    _existingRecDetails.RecRemarks = entity.Remarks;
                    _existingRecDetails.UpdatedBy = jwtData.Id;
                    _existingRecDetails.UpdatedDate = DateTime.Now;
                    _existingRecDetails.DueAuthCode = entity.DueAuthCode;
                    _existingRecDetails.NetAmount = entity.NetAmount;
                    _existingRecDetails.Discount = entity.Discount;
                    _existingRecDetails.RequiredAmount = entity.RequiredAmount;
                };

                db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }
        public Ret GetOpConsultationById(int id)
        {
            try
            {
                var res = (from a in db.OpConsultation
                           join c in db.Patient on a.PatientId equals c.PatientId
                           join d in db.Users on c.UserId equals d.Id
                           join e in db.Doctors on a.DoctorId equals e.DoctorId
                           join f in db.Users on e.UserId equals f.Id
                           join g in db.Organization on a.OrganizationId equals g.OrganizationId into _organizations
                           from h in _organizations.DefaultIfEmpty()
                           join i in db.PatientReceiptDetails.Where(pr => pr.RecType == "Consultation") on a.ConsultationId equals i.PCId into receipts
                           from j in receipts.DefaultIfEmpty().Take(1)
                           join k in db.BankMaster on j.BankId equals k.Id into banks
                           from l in banks.DefaultIfEmpty()
                           join m in db.Department on e.DepartmentId equals m.DepartmentId into departments
                           from n in departments.DefaultIfEmpty()
                           join o in db.Users on a.CreatedBy equals o.Id into createdUsers
                           from p in createdUsers
                           join q in db.TitleMaster on d.TitleId equals q.TitleId into ptitle
                           from r in ptitle.DefaultIfEmpty()
                           join s in db.Users on a.CancelBy equals s.Id into cancelups
                           from t in cancelups.DefaultIfEmpty()
                           join u in db.ReferralMaster.AsEnumerable() on a.ReferredBy equals Convert.ToString(u.Id) into _referredBy
                           from refBy in _referredBy.DefaultIfEmpty()
                           join v in db.DoctorPrescription on a.ConsultationId equals v.ConsultationId into prescription
                           from w in prescription.DefaultIfEmpty()
                           where a.ConsultationId == id
                           select new
                           {
                               a.ConsultationId,
                               a.ConsultationNo,
                               a.PatientId,
                               a.RefNo,
                               LastValidityDate = a.LastValidityDate.HasValue ? a.LastValidityDate.Value.Date : (DateTime?)null,
                               VisitType = a.VisitType ?? "",
                               PaymentBy = a.PaymentBy ?? "",
                               OrganizationId = a.OrganizationId ?? 0,
                               DoctorId = a.DoctorId ?? 0,
                               ConsultantFee = a.ConsultantFee ?? 0,
                               Visit = a.Visit ?? 0,
                               ConsReferral = a.ReferralType ?? "",
                               ConsReferredBy = a.ReferredBy ?? "",
                               CreatedBy = a.CreatedBy ?? 0,
                               a.Status,
                               ConsultationDate = Convert.ToDateTime(a.ConsultationDate).ToString("yyyy-MM-dd hh:mm:tt") ?? DateTime.MinValue.ToString("dd-MM-yyyy"),
                               CreatedDate = Convert.ToDateTime(a.CreatedDate).ToString("dd-MM-yyyy"),
                               UpdateBy = a.UpdatedBy ?? 0,
                               UpdateDate = Convert.ToDateTime(a.UpdatedDate).ToString("yyyy-MM-dd") ?? DateTime.MinValue.ToString("dd-MM-yyyy"),
                               TeleConsultation = a.TeleConsultation ?? "No",
                               PatientName = d.UserFullName ?? "",
                               Title = r.Title ?? "",
                               umrNumber = d.UserName,
                               d.ContactNo,
                               d.Email,
                               DOB = d.DOB.HasValue ? d.DOB.Value.ToString("yyyy-MM-dd") : "",
                               DoctorName = f.UserFullName,
                               DoctorCode = f.UserName,
                               h.OrganizationName,
                               d.Gender,
                               c.Age,
                               RecRemarks = j.RecRemarks ?? "",
                               ReceiptNumber = j.ReceiptNumber ?? "",
                               BankId = j.BankId ?? 0,
                               TxnNo = j.TxnNo ?? "",
                               RegFee = j.RegFee ?? 0,
                               RecDate = j.RecDate.HasValue ? j.RecDate.Value.ToString("yyyy-MM-dd") : "",
                               c.PatientType,
                               ReceiptId = j.ReceiptId == null ? 0 : j.ReceiptId,
                               l.BankName,
                               j.DueAuthCode,
                               j.NetAmount,
                               j.Discount,
                               j.RequiredAmount,
                               n.DepartmentName,
                               j.PaymentType,
                               CreatedUserName = p.UserFullName,
                               TokenNo = a.TokenNo ?? 0,
                               IsReqforCancel = a.CancelBy > 0 && a.Status == "Booked" ? "Yes" : "No",
                               IsDocApproved = a.IsDocApproved ?? "No",
                               IsAudApprove = a.IsAudApprove ?? "No",
                               ApproveAudId = a.ApproveAudId ?? 0,
                               ApproveDocId = a.ApproveDocId ?? 0,
                               CancelBy = a.CancelBy ?? 0,
                               CancelDate = a.CancelDate.HasValue ? a.CancelDate.Value.ToString("dd-MM-yyyy") : "",
                               DocApproveDate = a.DocApprovedDate.HasValue ? a.DocApprovedDate.Value.ToString("dd-MM-yyyy") : "",
                               AudApproveDate = a.AudApproveDate.HasValue ? a.AudApproveDate.Value.ToString("dd-MM-yyyy") : "",
                               CanReason = a.CanReason ?? "",
                               CancelUserId = t.UserName ?? "",
                               CancelUserName = t.UserFullName ?? "",
                               DocApproveRemarks = a.DocApproveRemarks ?? "",
                               AudApproveRemarks = a.AudApproveRemarks ?? "",
                               ReferenceName = refBy.ReferenceName ?? "",
                               AudApprove = a.IsAudApprove ?? "No",
                               Investigation = (w.Investigations != null && w.Investigations == "[]" || w.Investigations == null || string.IsNullOrEmpty(w.Investigations)) ? "No" : "Yes",
                               Medication = (w.Medication != null && w.Medication == "[]" || w.Medication == null || string.IsNullOrEmpty(w.Investigations)) ? "No" : "Yes",
                               h.CorpConsultation,
                               h.Pharmacy
                           }).AsNoTracking();
                return new Ret { status = true, data = res, message = "Op consultation loaded successfully?" };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong!" };
            }
        }
        public Ret GetAllOpConsultations(Pagination entity, JwtStatus jwtData)
        {
            try
            {
                var query = (from a in db.OpConsultation
                             join c in db.Patient on a.PatientId equals c.PatientId
                             join d in db.Users on c.UserId equals d.Id
                             join e in db.Doctors on a.DoctorId equals e.DoctorId
                             join f in db.Users on e.UserId equals f.Id
                             join g in db.TitleMaster on d.TitleId equals g.TitleId into ptitle
                             from h in ptitle.DefaultIfEmpty()
                             join i in db.ReferralMaster.AsEnumerable() on a.ReferredBy equals Convert.ToString(i.Id) into _referredBy
                             from refBy in _referredBy.DefaultIfEmpty()
                             join j in db.DoctorPrescription on a.ConsultationId equals j.ConsultationId into prescription
                             from k in prescription.DefaultIfEmpty()
                             where a.HospitalId == jwtData.HospitalId
                             orderby a.ConsultationId descending
                             select new
                             {
                                 a.ConsultationId,
                                 a.ConsultationNo,
                                 a.PatientId,
                                 a.RefNo,
                                 LastValidityDate = Convert.ToDateTime(a.LastValidityDate).Date,
                                 a.VisitType,
                                 a.PaymentBy,
                                 a.OrganizationId,
                                 a.DoctorId,
                                 a.ConsultantFee,
                                 a.Visit,
                                 a.ReferralType,
                                 a.ReferredBy,
                                 a.CreatedBy,
                                 a.Status,
                                 ConsultationDate = Convert.ToDateTime(a.ConsultationDate).ToString("yyyy-MM-dd hh:mm:tt") ?? DateTime.MinValue.ToString("dd-MM-yyyy"),
                                 a.CreatedDate,
                                 a.UpdatedBy,
                                 a.UpdatedDate,
                                 a.TeleConsultation,
                                 PatientName = d.UserFullName,
                                 h.Title,
                                 umrNumber = d.UserName,
                                 d.ContactNo,
                                 d.Email,
                                 DOB = Convert.ToDateTime(d.DOB).ToString("yyyy-MM-dd"),
                                 DoctorName = f.UserFullName,
                                 DoctorCode = f.UserName,
                                 IsReqforCancel = a.CancelBy > 0 && a.Status == "Booked" ? "Yes" : "No",
                                 IsDocApproved = a.IsDocApproved ?? "No",
                                 IsAudApprove = a.IsAudApprove ?? "No",
                                 DocApproveRemarks = a.DocApproveRemarks ?? "",
                                 AudApproveRemarks = a.AudApproveRemarks ?? "",
                                 ReferenceName = refBy.ReferenceName ?? "",
                                 AudApprove = a.IsAudApprove ?? "No",
                                 Investigation = ((k.Investigations != null && k.Investigations == "[]") || k.Investigations == null || string.IsNullOrEmpty(k.Investigations)) ? "No" : "Yes",
                                 Medication = ((k.Medication != null && k.Medication == "[]") || k.Medication == null || string.IsNullOrEmpty(k.Medication)) ? "No" : "Yes",
                                 ChiefComplaint = k.ChiefComplaint != null && !string.IsNullOrEmpty(k.ChiefComplaint) ? "Yes" : "No",
                                 PastHistory = k.PastHistory != null && !string.IsNullOrEmpty(k.PastHistory) ? "Yes" : "No",
                                 ProvisionalDiagnosis = k.ProvisionalDiagnosis != null && !string.IsNullOrEmpty(k.ProvisionalDiagnosis) ? "Yes" : "No",
                                 Allergies = k.Allergies != null && !string.IsNullOrEmpty(k.Allergies) ? "Yes" : "No",
                                 HistoryofPresentIllness = k.HistoryofPresentIllness != null && !string.IsNullOrEmpty(k.HistoryofPresentIllness) ? "Yes" : "No",
                                 PhysicalExamination = k.PhysicalExamination != null && !string.IsNullOrEmpty(k.PhysicalExamination) ? "Yes" : "No",
                                 ClinicalNotes = k.ClinicalNotes != null && !string.IsNullOrEmpty(k.ClinicalNotes) ? "Yes" : "No"

                             }).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.PatientName.Contains(entity.SearchKey) || c.umrNumber.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Patient"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret UploadOpLetter(OpFileUpload entity)
        {
            try
            {
                string extension = string.Empty;
                string LetterFileName = string.Empty;
                if (entity.File is not null)
                {
                    extension = Path.GetExtension(entity.File.FileName.ToString());
                    if (!Directory.Exists(Path.GetFullPath("Uploads/OPLetters/")))
                    {
                        Directory.CreateDirectory(Path.GetFullPath("Uploads/OPLetters/"));
                    }

                    LetterFileName = Path.GetFileNameWithoutExtension(entity.File.FileName.ToString()) + "_" + entity.ConsultationNo;
                    string NewFileNameWithFullPath = Path.GetFullPath("Uploads/OPLetters/" + LetterFileName + extension).Replace("~\\", "");
                    bool uploadstatus = CL.upload(entity.File, NewFileNameWithFullPath);


                    var _existingOp = db.OpConsultation.FirstOrDefault(op => op.ConsultationId == entity.ConsultationId);
                    //  _existingOp.LetterFileName = LetterFileName + extension;
                    db.OpConsultation.Update(_existingOp);
                    db.SaveChanges();
                    return new Ret { status = true, message = "File uploaded successfully!" };
                }

                return new Ret { status = false, message = "Failed to upload!" };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong" };
            }
        }

        public Ret CancelRequest(ApprovalEntity entity, JwtStatus jwtData)
        {
            try
            {
                var _existData = db.OpConsultation.Where(op => op.ConsultationId == entity.Id).AsNoTracking().FirstOrDefault();
                if (_existData is OpConsultationEntity)
                {
                    _existData.CancelBy = jwtData.Id;
                    _existData.CanReason = entity.CanReason;
                    _existData.CancelDate = DateTime.UtcNow;
                    _existData.Status = "Cancellation Requested";
                    db.Update(_existData);
                    db.SaveChanges();
                }
                return new Ret { status = true, message = "Your cancellation request has been submitted for approval." };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }
        public Ret DoctorApproval(ApprovalEntity entity, JwtStatus jwtData)
        {
            try
            {
                var _existData = db.OpConsultation.Where(op => op.ConsultationId == entity.Id).AsNoTracking().FirstOrDefault();
                if (_existData is OpConsultationEntity)
                {
                    _existData.ApproveDocId = jwtData.Id;
                    _existData.IsDocApproved = entity.Status;
                    _existData.DocApproveRemarks = entity.Remarks;
                    _existData.DocApprovedDate = DateTime.UtcNow;
                    db.Update(_existData);
                    db.SaveChanges();
                }
                return new Ret { status = true, message = "Status updated successfully." };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret AuditorApproval(ApprovalEntity entity, JwtStatus jwtData)
        {
            try
            {
                var _existData = db.OpConsultation.Where(op => op.ConsultationId == entity.Id).AsNoTracking().FirstOrDefault();
                if (_existData is OpConsultationEntity)
                {
                    _existData.ApproveAudId = jwtData.Id;
                    _existData.IsAudApprove = entity.Status;
                    _existData.AudApproveRemarks = entity.Remarks;
                    _existData.AudApproveDate = DateTime.UtcNow;
                    _existData.Status = "Canceled";
                    db.Update(_existData);
                    db.SaveChanges();
                }
                return new Ret { status = true, message = "Status updated successfully." };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }

    }
}
