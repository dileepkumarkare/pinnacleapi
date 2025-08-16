using DevExpress.DataAccess.Native.Sql;
using DevExpress.Office;
using DevExpress.PivotGrid.PivotTable;
using DevExpress.XtraCharts;
using DevExpress.XtraRichEdit.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Pinnacle.Entities;
using Pinnacle.Helpers;
using Pinnacle.Helpers.JWT;
using Serilog;

namespace Pinnacle.Models
{
    public class CorporateRegistrationModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();
        CommonLogic CL = new CommonLogic();
        public Ret GetAllCorporatePatients(Pagination entity, JwtStatus jwtData)
        {

            try
            {
                var query = (from a in db.CorporateRegistration
                             join b in db.Patient on a.PatientId equals b.PatientId
                             join c in db.Users on b.UserId equals c.Id
                             join d in db.Organization on a.OrganizationId equals d.OrganizationId
                             where (a.CoRegId == entity.Id || entity.Id == 0) && a.HospitalId == jwtData.HospitalId
                             orderby a.CoRegId descending
                             select new
                             {
                                 a.CoRegId,
                                 a.OrganizationId,
                                 a.PatientId,
                                 a.HospitalId,
                                 a.MedicalCardNo,
                                 CardValidUpto = Convert.ToDateTime(a.CardValidUpto).ToString("yyyy-MM-dd"),
                                 a.RelationToEmp,
                                 a.RelationType,
                                 a.EmpNo,
                                 a.EmpName,
                                 a.CreatedBy,
                                 CreatedDate = Convert.ToDateTime(a.CreatedDate).ToString("yyyy-MM-dd"),
                                 a.UpdatedBy,
                                 UpdatedDate = Convert.ToDateTime(a.UpdatedDate).ToString("yyyy-MM-dd"),
                                 a.Remarks,
                                 umrNumber = c.UserName,
                                 PatientName = c.UserFullName,
                                 c.ContactNo,
                                 c.Email,
                                 c.Gender,
                                 d.OrganizationCode,
                                 d.OrganizationName
                             }
                             ).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.umrNumber.Contains(entity.SearchKey) || c.PatientName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Corporate patient list"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }
        public Ret SaveCorporateRegistration(CorporateRegistrationEntity entity, JwtStatus jwtData)
        {
            try
            {
                entity.CreatedBy = jwtData.Id;
                entity.HospitalId = jwtData.HospitalId;
                string msg;

                if (!db.CorporateRegistration.Any(cr => cr.OrganizationId == entity.OrganizationId && cr.PatientId == entity.PatientId))
                {
                    db.CorporateRegistration.Add(entity);
                    msg = "Corporate registration saved successfully!";
                }
                else
                {
                    var _existingReg = db.CorporateRegistration.AsNoTracking().FirstOrDefault(x => x.CoRegId == entity.CoRegId);
                    if (_existingReg != null)
                    {
                        _existingReg.UpdatedDate = DateTime.Now;
                        _existingReg.UpdatedBy = jwtData.Id;
                        _existingReg.CardValidUpto = entity.CardValidUpto;
                        _existingReg.MedicalCardNo = entity.MedicalCardNo;
                        _existingReg.RelationToEmp = entity.RelationToEmp;
                        _existingReg.RelationType = entity.RelationType;
                        _existingReg.EmpNo = entity.EmpNo;
                        _existingReg.EmpName = entity.EmpName;
                        db.CorporateRegistration.Update(_existingReg);
                        msg = "Corporate patient details updated successfully!";
                    }
                    else
                    {
                        return new Ret { status = false, message = "This patient is already registered in the selected organization." };
                    }
                }

                db.SaveChanges();
                return new Ret { status = true, message = msg };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong!" };
            }
        }
        public Ret SaveCoLetterDetails(LetterUpload entity, JwtStatus jwtData)
        {
            try
            {

                string msg = "";
                string extension = string.Empty;
                string LetterFileName = string.Empty;


                if (entity.File is not null)
                {
                    extension = Path.GetExtension(entity.File.FileName.ToString());
                    if (!Directory.Exists(Path.GetFullPath("Uploads/OPLetters/")))
                    {
                        Directory.CreateDirectory(Path.GetFullPath("Uploads/OPLetters/"));
                    }

                    LetterFileName = Path.GetFileNameWithoutExtension(entity.File.FileName.ToString()) + "_" + entity.refLetterNo;
                    string NewFileNameWithFullPath = Path.GetFullPath("Uploads/OPLetters/" + LetterFileName + extension).Replace("~\\", "");
                    bool uploadstatus = CL.upload(entity.File, NewFileNameWithFullPath);
                }
                if (entity.letterId == 0)
                {
                    var _letterDetails = new CoLetterDetailsEntity
                    {
                        CoRegId = entity.coRegId,
                        RefLetterNo = entity.refLetterNo,
                        RefNo = entity.refNo,
                        LetterDate = entity.letterDate,
                        LetterValidUpto = entity.letterValidUpto,
                        LetterFor = entity.letterFor,
                        LetterIssueBy = entity.letterIssueBy,
                        PurposeofRef = entity.purposeofRef,
                        //ReferralFor = entity.referralFor,
                        Remarks = entity.remarks,
                        RefNoValidUpto = entity.refNoValidUpto,
                        LetterFileName = entity.File is not null ? LetterFileName + extension : "",
                        CreatedBy = jwtData.Id,
                        CreatedDate = DateTime.UtcNow
                    };
                    db.CoLetterDetails.Add(_letterDetails);
                    msg = "Corporate reference letter details saved successfully!";
                }

                else
                {
                    var _existingLetterDetails = db.CoLetterDetails.AsNoTracking().FirstOrDefault(x => x.LetterId == entity.letterId);
                    if (_existingLetterDetails != null)
                    {
                        _existingLetterDetails.RefLetterNo = entity.refLetterNo;
                        _existingLetterDetails.LetterDate = entity.letterDate;
                        _existingLetterDetails.LetterValidUpto = entity.letterValidUpto;
                        _existingLetterDetails.LetterFor = entity.letterFor;
                        _existingLetterDetails.PurposeofRef = entity.purposeofRef;
                        _existingLetterDetails.LetterIssueBy = entity.letterIssueBy;
                        // _existingLetterDetails.ReferralFor = entity.referralFor;
                        _existingLetterDetails.UpdatedBy = jwtData.Id;
                        _existingLetterDetails.UpdatedDate = DateTime.Now;
                        _existingLetterDetails.Remarks = entity.remarks;
                        _existingLetterDetails.UpdatedDate = DateTime.Now;
                        _existingLetterDetails.UpdatedBy = jwtData.Id;
                        _existingLetterDetails.LetterFileName = LetterFileName != string.Empty ? LetterFileName + extension : _existingLetterDetails.LetterFileName;
                        db.CoLetterDetails.Update(_existingLetterDetails);
                        msg = "Corporate reference letter details updated successfully!";
                    }

                }

                db.SaveChanges();
                return new Ret { status = true, message = msg };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong!" };
            }
        }
        public Ret GetCorporatePatientsData(PatientFilter entity, JwtStatus jwtData)
        {

            try
            {
                var _newRefNo = "REF" + DateTime.UtcNow.ToString("yyMMdd") + "001";
                var _refNoValidUpto = DateTime.Now;
                var Days = (from a in db.CorporateRegistration
                            join b in db.Organization on a.OrganizationId equals b.OrganizationId
                            where a.OrganizationId == entity.OrganizationId && a.PatientId == entity.PatientId
                            select new { b.IpDays, b.OpDays, a.CoRegId }).FirstOrDefault();

                CoLetterDetailsEntity _CoLetter = db.CoLetterDetails.Where(col => col.RefNo.StartsWith("REF")).OrderByDescending(col => col).FirstOrDefault();

                if (_CoLetter is CoLetterDetailsEntity && !string.IsNullOrEmpty(_CoLetter.RefNo))
                {
                    string _lastNumber = _CoLetter.RefNo.Substring(3);
                    _newRefNo = int.TryParse(_lastNumber, out int lastNo) && _CoLetter.RefNo.Substring(3, 6).ToString() == DateTime.UtcNow.ToString("yyMMdd") ? $"REF{(lastNo + 1):D9}" : _newRefNo;
                }
                if (Days.OpDays != null)
                {
                    // _refNoValidUpto = Days.OpDays == 1 ? DateTime.Now : DateTime.Now.AddDays(Convert.ToInt32(Days.OpDays));
                    _refNoValidUpto = DateTime.Now.AddDays(Convert.ToInt32(Days.OpDays) - 1);
                }

                var query = (from a in db.CorporateRegistration
                             join b in db.Patient on a.PatientId equals b.PatientId
                             join c in db.Users on b.UserId equals c.Id
                             join d in db.Organization on a.OrganizationId equals d.OrganizationId
                             where (a.PatientId == entity.PatientId && a.OrganizationId == entity.OrganizationId)
                             orderby a.CoRegId descending
                             select new
                             {
                                 a.CoRegId,
                                 a.OrganizationId,
                                 a.PatientId,
                                 a.HospitalId,
                                 a.MedicalCardNo,
                                 CardValidUpto = Convert.ToDateTime(a.CardValidUpto).ToString("yyyy-MM-dd"),
                                 a.RelationToEmp,
                                 a.RelationType,
                                 a.EmpNo,
                                 a.EmpName,
                                 a.CreatedBy,
                                 a.CreatedDate,
                                 a.UpdatedBy,
                                 a.UpdatedDate,
                                 a.Remarks,
                                 umrNumber = c.UserName,
                                 PatientName = c.UserFullName,
                                 c.ContactNo,
                                 c.Email,
                                 c.Gender,
                                 d.OrganizationCode,
                                 d.OrganizationName,
                                 RefNo = _newRefNo,
                                 RefNoValidUpto = Convert.ToDateTime(_refNoValidUpto).ToString("yyyy-MM-dd")
                             }
                             ).AsNoTracking().FirstOrDefault();

                return new Ret { status = true, message = FetchMessage(query, "Corporate patient list"), data = query };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }
        public Ret GetLetterDetails(Pagination entity, JwtStatus jwtData)
        {

            try
            {

                var query = (from letter in db.CoLetterDetails
                             join a in db.CorporateRegistration on letter.CoRegId equals a.CoRegId
                             join b in db.Patient on a.PatientId equals b.PatientId
                             join c in db.Users on b.UserId equals c.Id
                             join d in db.Organization on a.OrganizationId equals d.OrganizationId

                             where (letter.LetterId == entity.Id || entity.Id == 0) && a.HospitalId == jwtData.HospitalId
                             orderby a.CoRegId descending
                             select new
                             {
                                 letter.LetterId,
                                 letter.RefNo,
                                 letter.CoRegId,
                                 letter.RefLetterNo,
                                 LetterDate = Convert.ToDateTime(letter.LetterDate).ToString("yyyy-MM-dd"),
                                 LetterValidUpto = Convert.ToDateTime(letter.LetterValidUpto).ToString("yyyy-MM-dd"),
                                 letter.LetterFor,
                                 letter.PurposeofRef,
                                 letter.LetterIssueBy,
                                 letter.LetterFileName,
                                 //letter.ReferralFor,
                                 letter.CreatedBy,
                                 CreatedDate = Convert.ToDateTime(letter.CreatedDate).ToString("yyyy-MM-dd"),
                                 letter.UpdatedBy,
                                 UpdatedDate = Convert.ToDateTime(letter.UpdatedDate).ToString("yyyy-MM-dd"),
                                 letter.Remarks,
                                 a.OrganizationId,
                                 a.PatientId,
                                 a.HospitalId,
                                 a.MedicalCardNo,
                                 CardValidUpto = Convert.ToDateTime(a.CardValidUpto).ToString("yyyy-MM-dd"),
                                 a.RelationToEmp,
                                 a.RelationType,
                                 a.EmpNo,
                                 a.EmpName,
                                 umrNumber = c.UserName,
                                 PatientName = c.UserFullName,
                                 c.ContactNo,
                                 c.Email,
                                 c.Gender,
                                 d.OrganizationCode,
                                 d.OrganizationName,
                                 RefNoValidUpto = Convert.ToDateTime(letter.RefNoValidUpto).ToString("yyyy-MM-dd")
                             }
                             ).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.RefLetterNo.Contains(entity.SearchKey) || c.RefNo.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Corporate patient list"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }
    }
}
