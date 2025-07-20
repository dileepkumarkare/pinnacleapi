using DevExpress.XtraRichEdit.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Pinnacle.IServices;
using Serilog;
using System.Net;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;

namespace Pinnacle.Models
{
    public class PatientModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();
        private readonly IWhatsappService _whatsappService;
        public PatientModel(IWhatsappService whatsappService)
        {
            _whatsappService = whatsappService;
        }

        public Ret SavePatientBasicDetails(PatientEntity entity, JwtStatus jwtData)
        {
            try
            {
                string condition = DateTime.Now.ToString("yyMMdd");
                entity.CreatedBy = jwtData.Id;
                entity.HospitalId = jwtData.HospitalId;
                int userId = 0;
                var lastUMR = db.Users
            .Where(p => p.UserName.StartsWith(condition) && p.RoleId == 5 && p.UserId == null)
            .Select(p => p.UserName)
            .OrderByDescending(umr => umr)
            .FirstOrDefault();

                entity.UMRNumber = $"{DateTime.Now.ToString("yyMMdd")}001";

                if (!string.IsNullOrEmpty(lastUMR))
                {
                    string numericPart = lastUMR.Substring(0);
                    entity.UMRNumber = int.TryParse(numericPart, out int lastNumber) && lastUMR.Substring(0, 6) == DateTime.UtcNow.ToString("yyMMdd") ? $"{(lastNumber + 1):D9}" : entity.UMRNumber;
                }
                if (entity.PatientId == 0)
                {
                    var _users = new UserEntity()
                    {
                        Id = entity.UserId,
                        UserFullName = entity.PatientName,
                        ContactNo = entity.ContactNo,
                        Email = entity.Email,
                        DOB = Convert.ToDateTime(entity.DOB),
                        Gender = entity.Gender,
                        TitleId = entity.TitleId,
                        HospitalId = entity.HospitalId,
                        RoleId = 5,
                        UserName = entity.UMRNumber,
                        AddedBy = entity.CreatedBy
                    };
                    this.SaveUser(_users, out userId);
                    entity.UserId = userId;
                    entity.PatientProfile = (entity != null && !string.IsNullOrEmpty(entity.ProfilePic)) ? Convert.FromBase64String(entity.ProfilePic) : null;
                    db.Patient.Add(entity);
                    db.SaveChanges();
                    if (entity.PaymentType == "Corporate")
                    {
                        CorporateRegistrationEntity cpe = new CorporateRegistrationEntity();
                        cpe.PatientId = entity.PatientId;
                        cpe.OrganizationId = entity.OrganizationId;
                        db.CorporateRegistration.Add(cpe);
                    }
                    string registrationNumber = GenerateRegistrationNumber();
                    DateTime registrationDate = DateTime.Now;
                    DateTime registrationValidityDate = registrationDate.AddYears(1);
                    var registration = new PatientRegistration
                    {
                        PatientId = entity.PatientId,
                        RegistrationNo = registrationNumber,
                        RegistrationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        RegistrationValidityDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        CreatedBy = jwtData.Id,
                        CreatedDate = DateTime.Now
                    };

                    db.PatientRegistartion.Add(registration);
                    db.SaveChanges();
                    if (entity.PatientId > 0)
                    {
                        SavePatientReceipt(entity, jwtData);
                        SavePatientAddress(entity);
                    }

                    return new Ret { status = true, message = "Patient record added successfully!", data = new { patientId = entity.PatientId } };
                }
                else
                {
                    var resData = db.Patient.AsNoTracking().FirstOrDefault(h => h.PatientId == entity.PatientId);



                    if (resData != null)
                    {

                        resData.Vip = entity.Vip;
                        resData.NewBorn = entity.NewBorn;
                        resData.PatientType = entity.PatientType;
                        resData.Age = entity.Age;
                        resData.MaritalStatus = entity.MaritalStatus;
                        resData.Relation = entity.Relation;
                        resData.RelationName = entity.RelationName;
                        resData.Nationality = entity.Nationality;
                        resData.BloodGroup = entity.BloodGroup;
                        resData.OccupationId = entity.OccupationId;
                        resData.Refferal = entity.Refferal;
                        resData.VipSour = entity.VipSour;
                        resData.Remarks = entity.Remarks;
                        resData.UpdatedDate = DateTime.UtcNow;
                        resData.ReferredBy = entity.ReferredBy;
                        resData.PrimarycontactNo = entity.PrimarycontactNo;
                        resData.DoctorId = entity.DoctorId;
                        resData.PassportNo = entity.PassportNo;
                        resData.PCountryId = entity.PCountryId;
                        resData.PIssueDate = entity.PIssueDate;
                        resData.PExpiryDate = entity.PExpiryDate;
                        resData.VisaNo = entity.VisaNo;
                        resData.VCountryId = entity.VCountryId;
                        resData.VIssueDate = entity.VIssueDate;
                        resData.VExpiryDate = entity.VExpiryDate;
                        resData.HospitalId = resData.HospitalId == 0 ? entity.HospitalId : resData.HospitalId;
                        entity.PatientProfile = (entity != null && !string.IsNullOrEmpty(entity.ProfilePic)) ? Convert.FromBase64String(entity.ProfilePic) : null;
                        db.Patient.Update(entity);
                        db.SaveChanges();
                        var _users = new UserEntity()
                        {
                            Id = entity.UserId,
                            UserFullName = entity.PatientName,
                            ContactNo = entity.ContactNo,
                            Email = entity.Email,
                            DOB = Convert.ToDateTime(entity.DOB),
                            Gender = entity.Gender,
                            TitleId = entity.TitleId,
                            HospitalId = entity.HospitalId,
                            RoleId = 5,
                            UserName = entity.UMRNumber,
                            AddedBy = entity.CreatedBy
                        };
                        //var userData = db.Users.Where(u => u.Id == resData.UserId && u.UserName.StartsWith("T")).FirstOrDefault();
                        //if (userData != null)
                        //{
                        //    userData.UserName = entity.UMRNumber;
                        //}
                        if (entity.PatientId > 0)
                        {
                            SavePatientReceipt(entity, jwtData);
                            SavePatientAddress(entity);
                        }
                        return new Ret { status = true, message = "Patient updated successfully!", data = new { patientId = entity.PatientId } };
                    }
                    else
                    {
                        return new Ret { status = false, message = "Patient record not found for update." };
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error("Error at " + DateTime.Now.ToString() + " - " + ex.Message);
                return new Ret { status = false, message = "Failed to save Patient record." };
            }
        }

        public void SaveUser(UserEntity entity, out int userId)
        {
            if (entity.Id == 0)
            {
                db.Users.Add(entity);
            }
            else
            {
                var _existingUser = db.Users.FirstOrDefault(user => user.Id == entity.Id);
                if (_existingUser != null)
                {
                    _existingUser.UserFullName = entity.UserFullName;
                    _existingUser.ContactNo = entity.ContactNo;
                    _existingUser.Email = entity.Email;
                    _existingUser.DOB = entity.DOB;
                    _existingUser.Gender = entity.Gender;
                    _existingUser.TitleId = entity.TitleId;
                    _existingUser.HospitalId = entity.HospitalId;
                    _existingUser.UserName = _existingUser.UserName.StartsWith("T") ? entity.UserName : _existingUser.UserName;
                    db.Users.Update(_existingUser);
                }
            }
            db.SaveChanges();
            userId = entity.Id;
        }
        private string GenerateRegistrationNumber()
        {
            var lastReg = db.PatientRegistartion
                .OrderByDescending(r => r.RegistrationNo)
                .Select(r => r.RegistrationNo)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(lastReg) && lastReg.StartsWith("REG"))
            {
                string numericPart = lastReg.Replace("REG", "");
                if (int.TryParse(numericPart, out int lastNumber))
                {
                    return "REG" + (lastNumber + 1).ToString("D6");
                }
            }
            return "REG000001";
        }
        public Ret GetUMRNumber()
        {
            try
            {
                string condition = DateTime.Now.ToString("yyMMdd");
                var lastUMR = db.Users
                    .Where(p => p.UserName.StartsWith(condition) && p.RoleId == 5 && p.UserId == null)
                    .Select(p => p.UserName)
                    .OrderByDescending(umr => umr)
                    .FirstOrDefault();

                string nextUMR = $"{DateTime.Now.ToString("yyMMdd")}001";

                if (!string.IsNullOrEmpty(lastUMR))
                {
                    string numericPart = lastUMR.Substring(0);
                    nextUMR = int.TryParse(numericPart, out int lastNumber) && lastUMR.Substring(0, 6) == DateTime.UtcNow.ToString("yyMMdd") ? $"{(lastNumber + 1):D9}" : nextUMR;
                }

                return new Ret { status = true, message = "UHID number generated successfully.", data = nextUMR };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Failed to generate UMR number." };
            }
        }

        public Ret GetAllPatient(Pagination entity, JwtStatus jwtData)
        {
            try
            {
                var query = (from a in db.Patient
                             join b in db.Users on a.UserId equals b.Id
                             join c in db.Doctors on a.DoctorId equals c.DoctorId into doctors
                             from d in doctors.DefaultIfEmpty()
                             join e in db.Department on d.DepartmentId equals e.DepartmentId into departments
                             from f in departments.DefaultIfEmpty()
                             join g in db.Users on a.CreatedBy equals g.Id
                             join h in db.TitleMaster on b.TitleId equals h.TitleId into titleMasters
                             from title in titleMasters.DefaultIfEmpty()
                             join i in db.CorporateRegistration on a.PatientId equals i.PatientId into corporatepatients
                             from j in corporatepatients.DefaultIfEmpty()
                             where ((entity.Id == 0 || a.PatientId == entity.Id) && (b.HospitalId == jwtData.HospitalId || b.HospitalId == 0) && b.RoleId == 5)
                             orderby a.PatientId descending
                             select new
                             {
                                 a.PatientId,
                                 a.PatientAdmissionType,
                                 a.Vip,
                                 a.NewBorn,
                                 a.PatientType,
                                 a.Age,
                                 a.MaritalStatus,
                                 a.Relation,
                                 a.RelationName,
                                 a.Nationality,
                                 a.BloodGroup,
                                 a.OccupationId,
                                 a.Refferal,
                                 a.VipSour,
                                 a.Remarks,
                                 a.CreatedBy,
                                 a.CreatedDate,
                                 a.UpdatedDate,
                                 a.ReferredBy,
                                 a.UserId,
                                 PatientName = b.UserFullName,
                                 b.Id,
                                 UMRNumber = b.UserName,
                                 b.ContactNo,
                                 b.Email,
                                 b.Password,
                                 b.Status,
                                 b.UserProfileId,
                                 b.RoleId,
                                 b.HospitalId,
                                 b.TitleId,
                                 b.Gender,
                                 b.IsMedicalTestRequired,
                                 b.UserFullName,
                                 b.DOB,
                                 a.PrimarycontactNo,
                                 a.DoctorId,
                                 f.DepartmentName,
                                 CreatedByName = g.UserName,
                                 title.Title,
                                 j.OrganizationId,
                                 a.PassportNo,
                                 a.PCountryId,
                                 PIssueDate = Convert.ToDateTime(a.PIssueDate).ToString("yyyy-MM-dd"),
                                 PExpiryDate = Convert.ToDateTime(a.PExpiryDate).ToString("yyyy-MM-dd"),
                                 a.VisaNo,
                                 a.VCountryId,
                                 VIssueDate = Convert.ToDateTime(a.VIssueDate).ToString("yyyy-MM-dd"),
                                 VExpiryDate = Convert.ToDateTime(a.VExpiryDate).ToString("yyyy-MM-dd"),
                                 a.PatientProfile

                             }).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.UMRNumber.Contains(entity.SearchKey) || c.PatientName.Contains(entity.SearchKey) || c.ContactNo.Contains(entity.SearchKey));
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
        public Ret PatientGetById(int Id)
        {
            try
            {
                var res = (from a in db.Patient
                           join b in db.Users on a.UserId equals b.Id
                           join cu in db.Users on a.CreatedBy equals cu.Id
                           join c in db.PatientAddress on a.PatientId equals c.PatientId into _patientAddress
                           from address in _patientAddress.DefaultIfEmpty()
                           join d in db.PatientReceiptDetails.Where(rec => rec.RecType == "Registration") on a.PatientId equals d.PCId into _patientReceiptDetails
                           from receipt in _patientReceiptDetails.DefaultIfEmpty()
                           join pincodeData in db.PincodeData on address.AreaCode equals pincodeData.Id into _pincodeData
                           from e in _pincodeData.DefaultIfEmpty()
                           join district in db.District on e.DistrictId equals district.Id into _district
                           from f in _district.DefaultIfEmpty()
                           join state in db.States on f.StateId equals state.Id into _states
                           from g in _states.DefaultIfEmpty()
                           join h in db.Doctors on a.DoctorId equals h.DoctorId into doctors
                           from i in doctors.DefaultIfEmpty()
                           join j in db.Department on i.DepartmentId equals j.DepartmentId into departments
                           from k in departments.DefaultIfEmpty()
                           join l in db.TitleMaster on b.TitleId equals l.TitleId into titles
                           from m in titles.DefaultIfEmpty()
                           join n in db.CorporateRegistration on a.PatientId equals n.PatientId into corporatepatients
                           from o in corporatepatients.DefaultIfEmpty()
                           join p in db.Countries on a.PCountryId equals p.CountryId into pcountries
                           from q in pcountries.DefaultIfEmpty()
                           join r in db.Countries on a.VCountryId equals r.CountryId into vcountries
                           from s in vcountries.DefaultIfEmpty()
                           where (a.PatientId == Id)
                           select new
                           {
                               a.PatientId,
                               a.PatientAdmissionType,
                               a.Vip,
                               a.NewBorn,
                               a.PatientType,
                               a.Age,
                               a.MaritalStatus,
                               a.Relation,
                               a.RelationName,
                               a.Nationality,
                               a.BloodGroup,
                               a.OccupationId,
                               a.Refferal,
                               a.VipSour,
                               a.Remarks,
                               a.CreatedBy,
                               a.CreatedDate,
                               a.UpdatedDate,
                               a.ReferredBy,
                               a.UserId,
                               PatientName = b.UserFullName,
                               b.Id,
                               UMRNumber = b.UserName,
                               b.ContactNo,
                               b.Email,
                               b.Password,
                               b.Status,
                               o.OrganizationId,
                               b.UserProfileId,
                               b.RoleId,
                               b.HospitalId,
                               b.TitleId,
                               b.Gender,
                               b.IsMedicalTestRequired,
                               DOB = Convert.ToDateTime(b.DOB).ToString("yyyy-MM-dd"),
                               a.PrimarycontactNo,
                               ReceiptId = receipt.ReceiptId == null ? 0 : receipt.ReceiptId,
                               receipt.PCId,
                               receipt.ReceiptNumber,
                               receipt.RegFee,
                               receipt.RecType,
                               receipt.BankId,
                               Validity = Convert.ToDateTime(receipt.Validity).Date.ToString("yyyy-MM-dd"),
                               receipt.RecDate,
                               receipt.PaymentType,
                               receipt.TxnNo,
                               receipt.RecRemarks,
                               PAddressId = address.PAddressId == null ? 0 : address.PAddressId,
                               address.AreaCode,
                               address.Address,
                               address.IdProof,
                               address.IdNumber,
                               address.GovtId1,
                               address.GovtIdNumber1,
                               address.GovtId2,
                               address.GovtIdNumber2,
                               address.Country,
                               address.Area,
                               e.DistrictId,
                               f.DistrictName,
                               g.StateName,
                               PinCode = e.Pincode,
                               k.DepartmentName,
                               a.DoctorId,
                               DoctorName = (i.Title ?? "" + i.DoctorName).Trim(),
                               CreatedUserName = cu.UserFullName,
                               m.Title,
                               a.PassportNo,
                               a.PCountryId,
                               PIssueDate = Convert.ToDateTime(a.PIssueDate).ToString("yyyy-MM-dd"),
                               PExpiryDate = Convert.ToDateTime(a.PExpiryDate).ToString("yyyy-MM-dd"),
                               a.VisaNo,
                               a.VCountryId,
                               VIssueDate = Convert.ToDateTime(a.VIssueDate).ToString("yyyy-MM-dd"),
                               VExpiryDate = Convert.ToDateTime(a.VExpiryDate).ToString("yyyy-MM-dd"),
                               a.PatientProfile,
                               PCountryName = q.CountryName,
                               VCountryName = s.CountryName
                           }).AsNoTracking().FirstOrDefault();
                return new Ret { status = true, message = FetchMessage(res, "Patient"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret GetAllReligionLabel()
        {
            try
            {
                var res = db.Religion.Select(a => new { value = a.ReligionId, label = a.ReligionName }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Employee"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret PatientUMRGetByNumber(Search entity)
        {
            try
            {
                string RefNo;
                var _patient = (from a in db.Users
                                join b in db.Patient on a.Id equals b.UserId
                                where b.PatientId == entity.PatientId
                                select new
                                {
                                    umrNumber = a.UserName,
                                    PatientName = a.UserFullName,
                                    b.PatientType,
                                    b.Age,
                                    a.Gender,
                                    a.ContactNo,
                                    a.Email,
                                    a.DOB,
                                    b.PatientId,
                                    a.HospitalId,
                                    b.ReferredBy,
                                    ReferralType = b.Refferal,
                                    b.Relation,
                                    b.RelationName,
                                    b.Remarks,
                                    b.Nationality,
                                    b.BloodGroup,
                                    b.MaritalStatus,
                                    b.OccupationId
                                }
                               ).AsNoTracking().FirstOrDefault();
                OpConsultationEntity _opCons = this.GetOpOConsultationValidity(Convert.ToInt32(entity.OrganizationId), _patient.PatientId, Convert.ToInt32(_patient.HospitalId), out RefNo, entity.PatientType, Convert.ToInt32(entity.DoctorId));
                string _ReceiptNumber = GetConsultationReceiptNumber();

                if (_opCons.RefNo == "NA" && entity.PatientType != "Self")
                {
                    return new Ret { status = false, message = "Please ensure the letter entry is submitted." };
                }
                return new Ret
                {
                    status = _patient != null,
                    message = FetchMessage(_patient, "Patient"),
                    data = new
                    {
                        _patient.umrNumber,
                        _patient.PatientName,
                        _patient.PatientId,
                        _patient.PatientType,
                        DOB = Convert.ToDateTime(_patient.DOB).ToString("yyyy-MM-dd"),
                        _patient.Age,
                        _patient.ContactNo,
                        _patient.Email,
                        _patient.Gender,
                        ConsultationNo = _opCons.ConsultationNo,
                        RefNo = _opCons.RefNo,
                        Visit = _opCons.Visit,
                        LastValidityDate = _opCons.LastValidityDate,
                        ReceiptNumber = _ReceiptNumber,
                        VisitType = _opCons.VisitType,
                        RemainingVisits = _opCons.RemainingVisits < 0 ? 0 : _opCons.RemainingVisits,
                        LatestRefNo = RefNo,
                        _patient.MaritalStatus,
                        _patient.OccupationId,
                        _patient.BloodGroup,
                        _patient.Nationality,
                        _patient.Relation,
                        _patient.RelationName,
                        _patient.ReferralType,
                        LastConsultationDate = Convert.ToDateTime(_opCons.ConsultationDate).ToString("yyyy-MM-dd"),
                        ConsReferredBy = _opCons.ReferredBy,
                        ConsReferral = _opCons.ReferralType
                    }
                };
            }
            catch (Exception ex)
            {
                Log.Information("Error " + DateTime.Now.ToString() + " message " + ex.Message);
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }

        public string GetConsultationReceiptNumber()
        {
            try
            {
                var _lastReceiptNumber = db.PatientReceiptDetails
                    .Where(p => p.ReceiptNumber.StartsWith("CR") && p.RecType == "Consultation")
                    .Select(p => p.ReceiptNumber)
                    .OrderByDescending(umr => umr)
                    .FirstOrDefault();

                string nextUMR = $"CR{DateTime.Now.ToString("yyMMdd")}001";

                if (!string.IsNullOrEmpty(_lastReceiptNumber))
                {
                    string numericPart = _lastReceiptNumber.Substring(2);
                    nextUMR = int.TryParse(numericPart, out int lastNumber) && _lastReceiptNumber.Substring(2, 6) == DateTime.UtcNow.ToString("yyMMdd") ? $"CR{(lastNumber + 1):D3}" : nextUMR;
                }
                return nextUMR;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public OpConsultationEntity GetOpOConsultationValidity(int OrganizationId, int PatientId, int HospitalId, out string RefNo, string PatientType, int DoctorId)
        {
            HospitalEntity _visitDays = db.Hospital.Where(h => h.HospitalId == HospitalId).FirstOrDefault();
            string _consultationNo = string.Concat(HospitalId == 1 ? "COP" : "COS", DateTime.Now.ToString("yyMMdd"), "001");
            string _condition = string.Concat(HospitalId == 1 ? "COP" : "COS", DateTime.Now.ToString("yyMMdd"));
            var _refNo = (from a in db.CoLetterDetails.AsEnumerable()
                          join b in db.CorporateRegistration on a.CoRegId equals b.CoRegId
                          where b.OrganizationId == OrganizationId && b.PatientId == PatientId && Convert.ToDateTime(a.RefNoValidUpto).Date >= DateTime.Now.Date
                          select new { a.RefNo, a.RefNoValidUpto }).FirstOrDefault();

            var _lastConsultationNo = db.OpConsultation.Where(cons => cons.ConsultationNo.StartsWith(_condition)).Select(cons => cons.ConsultationNo).OrderByDescending(cons => cons).FirstOrDefault();
            if (!string.IsNullOrEmpty(_lastConsultationNo))
            {
                string _lastNumber = _lastConsultationNo.Substring(3);
                string a = _lastConsultationNo.Substring(3, 6);
                _consultationNo = int.TryParse(_lastNumber, out int lastNo) &&
                                  _lastConsultationNo.Substring(3, 6) == DateTime.Now.ToString("yyMMdd")
                    ? (HospitalId == 1
                        ? $"COP{(lastNo + 1):D3}"
                        : $"COS{(lastNo + 1):D3}")
                    : _consultationNo;
            }
            OpConsultationEntity _opConsultation = db.OpConsultation.Where(cons => cons.PatientId == PatientId && cons.PaymentBy == PatientType && cons.DoctorId == DoctorId).OrderByDescending(cons => cons).FirstOrDefault();
            int _visit = _opConsultation is not null && _opConsultation.Visit != null && _opConsultation.Visit < _visitDays.Visits ? Convert.ToInt32(_opConsultation.Visit) : 0;
            DateTime _lastVisitDate = _opConsultation is not null && _opConsultation.LastValidityDate != null && _opConsultation.Visit < _visitDays.Visits ? Convert.ToDateTime(_opConsultation.LastValidityDate) : PatientType == "Corporate" && _refNo != null ? Convert.ToDateTime(_refNo.RefNoValidUpto) : DateTime.Now.AddDays(Convert.ToInt32(_visitDays.Days)).Date;
            string _visitType = (_visit + 1 > 1 && _visit + 1 <= _visitDays.Visits && _lastVisitDate != null && DateTime.Now <= Convert.ToDateTime(_lastVisitDate)) ? "Revisit" : "Normal";
            int doctorId = 0;
            var _consultationDate = _opConsultation is not null ? _opConsultation.ConsultationDate : DateTime.MinValue;
            if (_opConsultation != null && int.TryParse(_opConsultation.DoctorId?.ToString(), out int parsedDoctorId))
            {
                doctorId = parsedDoctorId;
            }

            RefNo = _refNo is not null ? _refNo.RefNo.ToString() : "NA";
            return new OpConsultationEntity
            {
                ConsultationNo = _consultationNo,
                RefNo = _refNo is not null ? _refNo.RefNo.ToString() : "NA",
                Visit = _visit + 1,
                RemainingVisits = (Convert.ToInt32(_visitDays.Visits) - (_visit + 1)),
                LastValidityDate = _lastVisitDate,
                VisitType = _visitType,
                DoctorId = doctorId,
                ConsultationDate = _consultationDate,
                ReferralType = _opConsultation is OpConsultationEntity && _opConsultation.ReferralType != null ? _opConsultation.ReferralType : "",
                ReferredBy = _opConsultation is OpConsultationEntity && _opConsultation.ReferredBy != null ? _opConsultation.ReferredBy : ""
            };
        }
        public Ret SavePatientAddress(PatientEntity entity)
        {
            try
            {
                var _patientAddress = new PatientAddress
                {
                    AreaCode = entity.AreaCode,
                    Address = entity.Address,
                    IdProof = entity.IdProof,
                    IdNumber = entity.IdNumber,
                    GovtId1 = entity.GovtId1,
                    GovtIdNumber1 = entity.GovtIdNumber1,
                    GovtId2 = entity.GovtId2,
                    GovtIdNumber2 = entity.GovtIdNumber2,
                    PatientId = entity.PatientId,
                    Country = entity.Country,
                    Area = entity.Area
                };
                if (!db.PatientAddress.Any(address => address.PatientId == entity.PatientId))
                {
                    db.PatientAddress.Add(_patientAddress);
                }
                else
                {
                    var _existingAddress = db.PatientAddress.FirstOrDefault(addr => addr.PatientId == entity.PatientId);
                    _existingAddress.AreaCode = entity.AreaCode;
                    _existingAddress.Address = entity.Address;
                    _existingAddress.IdProof = entity.IdProof;
                    _existingAddress.IdNumber = entity.IdNumber;
                    _existingAddress.GovtId1 = entity.GovtId1;
                    _existingAddress.GovtIdNumber1 = entity.GovtIdNumber1;
                    _existingAddress.GovtId2 = entity.GovtId2;
                    _existingAddress.GovtIdNumber2 = entity.GovtIdNumber2;
                    _existingAddress.Country = entity.Country;
                    _existingAddress.Area = entity.Area;
                    db.PatientAddress.Update(_existingAddress);
                }
                db.SaveChanges();
                return new Ret { status = true, message = "Patient address saved successfully!" };
            }
            catch (Exception ex)
            {
                Log.Information("Error " + DateTime.Now.ToString() + " message " + ex.Message);
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }

        public void SavePatientReceipt(PatientEntity entity, JwtStatus jwtData)
        {
            try
            {
                entity.ReceiptNumber = "REC000001";
                var latestReceiptNumber = db.PatientReceiptDetails.Where(rec => rec.ReceiptNumber.StartsWith("REC") && rec.RecType == "Registration").Select(rec => rec.ReceiptNumber).OrderByDescending(rec => rec).FirstOrDefault();
                if (!string.IsNullOrEmpty(latestReceiptNumber))
                {
                    string lastestNumber = latestReceiptNumber.Substring(3);
                    if (int.TryParse(lastestNumber, out int lastnumber))
                    {
                        entity.ReceiptNumber = $"REC{(lastnumber + 1):D6}";
                    }
                }
                var receiptDetails = new PatientReceiptDetailsEntity
                {
                    PCId = entity.PatientId,
                    ReceiptNumber = entity.ReceiptNumber,
                    RegFee = entity.RegFee,
                    RecType = "Registration",
                    BankId = entity.BankId,
                    Validity = DateTime.Now.AddYears(100).Date,
                    RecDate = DateTime.Now,
                    PaymentType = entity.PaymentType,
                    TxnNo = entity.TxnNo,
                    RecRemarks = entity.RecRemarks,
                    CreatedBy = jwtData.Id,
                    CreatedDate = entity.CreatedDate,
                };
                if (entity.ReceiptId == 0)
                {
                    db.PatientReceiptDetails.Add(receiptDetails);
                }
                else
                {
                    var _existingRecDetails = db.PatientReceiptDetails.FirstOrDefault(rec => rec.ReceiptId == entity.ReceiptId);
                    _existingRecDetails.RegFee = entity.RegFee;
                    _existingRecDetails.BankId = entity.BankId;
                    _existingRecDetails.RecDate = DateTime.Now.Date;
                    _existingRecDetails.TxnNo = entity.TxnNo;
                    _existingRecDetails.RecRemarks = entity.Remarks;
                    _existingRecDetails.UpdatedBy = jwtData.Id;
                    _existingRecDetails.UpdatedDate = DateTime.Now;
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }

        public Ret GetPatientAddress(int Id)
        {
            try
            {
                var res = from patientAddress in db.PatientAddress
                          join pincodeData in db.PincodeData on patientAddress.AreaCode equals pincodeData.Id
                          join district in db.District on pincodeData.DistrictId equals district.Id
                          join state in db.States on district.StateId equals state.Id
                          where patientAddress.PatientId == Id
                          select new
                          {
                              patientAddress.PAddressId,
                              patientAddress.PatientId,
                              patientAddress.AreaCode,
                              patientAddress.Address,
                              patientAddress.IdProof,
                              patientAddress.IdNumber,
                              patientAddress.GovtId1,
                              patientAddress.GovtIdNumber1,
                              patientAddress.GovtId2,
                              patientAddress.GovtIdNumber2,
                              patientAddress.Country,
                              patientAddress.Area,
                              pincodeData.DistrictId,
                              district.DistrictName,
                              state.StateName,
                              PinCode = pincodeData.Pincode
                          };

                return new Ret { status = res != null, message = FetchMessage(res, "Patient"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information("Error " + DateTime.Now.ToString() + " message " + ex.Message);
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret GetPatientReceiptNumber()
        {
            try
            {
                var _lastReceiptNumber = db.PatientReceiptDetails
                    .Where(p => p.ReceiptNumber.StartsWith("RR") && p.RecType == "Registration")
                    .Select(p => p.ReceiptNumber)
                    .OrderByDescending(umr => umr)
                    .FirstOrDefault();

                string nextUMR = $"RR{DateTime.Now.ToString("yyMMdd")}001";

                if (!string.IsNullOrEmpty(_lastReceiptNumber))
                {

                    string numericPart = _lastReceiptNumber.Substring(2);
                    nextUMR = int.TryParse(numericPart, out int lastNumber) && _lastReceiptNumber.Substring(2, 6) == DateTime.UtcNow.ToString("yyMMdd") ? $"RR{(lastNumber + 1):D3}" : nextUMR;

                }
                // var patientEntity = new PatientEntity { UMRNumber = nextUMR };

                return new Ret { status = true, message = "Receipt number generated successfully.", data = nextUMR };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, data = ex.Message };
            }
        }
        public Ret GetPatientUmrNumbersList(JwtStatus jwtData)
        {
            try
            {
                var _patientUrmLists = (from user in db.Users
                                        join patient in db.Patient on user.Id equals patient.UserId
                                        where user.HospitalId == jwtData.HospitalId && user.RoleId == 5
                                        select new { umrNumber = user.UserName, PatientName = user.UserFullName, user.ContactNo, user.Email, patient.PatientId }).
                                        OrderByDescending(user => user.umrNumber).AsNoTracking();
                if (_patientUrmLists is not null)
                    return new Ret { status = true, message = "URM number list loaded successfully!", data = _patientUrmLists };
                else
                    return new Ret { status = true, message = "No data loaded!", data = _patientUrmLists };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong" };
            }
        }
        public Ret SavePatientProfile(PatientEntity entity)
        {
            try
            {
                var patientData = db.Patient.Where(p => p.PatientId == entity.PatientId).FirstOrDefault();
                if (patientData is PatientEntity)
                {
                    patientData.PatientProfile = entity.PatientProfile;
                    db.Patient.Update(patientData);
                    db.SaveChanges();
                }
                return new Ret { status = true, message = "Profile updated successfully." };
            }
            catch (Exception ex)
            {
                Log.Information("Patient Model=> SavePatientProfile error at " + DateTime.UtcNow.ToString() + " message " + ex.Message);
                return new Ret { status = false, message = "Something went wrong." };
            }
        }

        public Ret GetOpConsultations(Pagination entity, JwtStatus JwtData)
        {
            try
            {
                var query = (from a in db.OpConsultation
                             join c in db.Patient on a.PatientId equals c.PatientId
                             join d in db.Users on c.UserId equals d.Id
                             join e in db.Doctors on a.DoctorId equals e.DoctorId
                             join f in db.Users on e.UserId equals f.Id
                             where d.Id == JwtData.Id
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
                                 ConstultationDate = Convert.ToDateTime(a.ConsultationDate).ToString("yyyy-MM-dd hh:mm:tt") ?? DateTime.MinValue.ToString("dd-MM-yyyy"),
                                 a.CreatedDate,
                                 a.UpdatedBy,
                                 a.UpdatedDate,
                                 PatientName = d.UserFullName,
                                 umrNumber = d.UserName,
                                 d.ContactNo,
                                 d.Email,
                                 DOB = Convert.ToDateTime(d.DOB).ToString("yyyy-MM-dd"),
                                 DoctorName = f.UserFullName,
                                 DoctorCode = f.UserName,
                                 d.Gender,
                                 c.Age,
                                 IsReqforCancel = a.CancelBy > 0 && a.Status == "Booked" ? "Yes" : "No",
                                 IsDocApproved = a.IsDocApproved ?? "No",
                                 IsAudApprove = a.IsAudApprove ?? "No"
                             }).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(a => a.PatientName.Contains(entity.SearchKey) || a.ConsultationNo.Contains(entity.SearchKey));

                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Op consultation"), data = res, totalCount = totalCount };

            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong!" };
            }
        }

        public Ret Registration(PatientEntity entity)
        {
            try
            {
                int userId = 0;
                if (db.Users.Any(usr => usr.ContactNo == entity.ContactNo))
                {
                    return new Ret { status = false, message = "This contact number is already registered, Please use a different number." };
                }
                else if (entity.PatientId == 0)
                {
                    var _condition = $"T{DateTime.UtcNow.ToString("yyMMdd")}";
                    var userName = $"T{DateTime.UtcNow.ToString("yyMMdd")}001";
                    var _prevTempUserId = db.Users.Where(usr => usr.UserId.StartsWith(_condition)).Select(usr => usr.UserId).OrderByDescending(user => user).AsNoTracking().FirstOrDefault();
                    if (!string.IsNullOrEmpty(_prevTempUserId))
                    {
                        string LastNumber = _prevTempUserId.Substring(1);
                        userName = int.TryParse(LastNumber, out int _lastNumber) ? $"T{(_lastNumber + 1):D9}" : userName;
                    }
                    var _users = new UserEntity()
                    {
                        UserFullName = entity.PatientName,
                        ContactNo = entity.ContactNo,
                        Email = entity.Email,
                        DOB = Convert.ToDateTime(entity.DOB),
                        Gender = entity.Gender,
                        UserId = userName,
                        UserName = userName,
                        RoleId = 5,
                        HospitalId = 0,
                        CreatedDate = DateTime.Now
                    };
                    this.SaveUser(_users, out userId);
                    var existinguser = db.Users.Where(usr => usr.Id == userId).FirstOrDefault();
                    if (existinguser != null)
                    {
                        existinguser.AddedBy = userId;
                        db.Users.Update(existinguser);
                        db.SaveChanges();
                    }
                    var patientEntity = new PatientEntity()
                    {
                        UserId = userId,
                        Age = entity.Age,
                        PatientType = "General",
                        PatientAdmissionType = "New",
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = userId
                    };
                    db.Patient.Add(patientEntity);
                    db.SaveChanges();
                    entity.PatientId = patientEntity.PatientId;
                }
                return new Ret { status = true, message = "Thank you for registering with Pinnacle Hospitals. We're here for your health and well-being.", data = new { patientId = entity.PatientId } };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong, Please contact IT Admin" };
            }
        }
    }
}





