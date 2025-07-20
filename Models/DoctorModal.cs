using DevExpress.Emf;
using DevExpress.Office;
using DevExpress.Pdf.Native.BouncyCastle.Ocsp;
using DevExpress.PivotGrid.PivotTable;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraRichEdit.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using Pinnacle.Entities;
using Pinnacle.Helpers;
using Pinnacle.Helpers.JWT;
using Serilog;
using System.Data;
using System.Text.Json.Nodes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
namespace Pinnacle.Models
{
    public class DoctorModal : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();
        CommonLogic cl = new CommonLogic();
        public Ret SaveDoctor(DoctorEntity entity, JwtStatus jwt, string IpAddress)
        {
            try
            {
                entity.CreatedBy = jwt.Id;
                string msg;
                var _auditLog = new AuditLog();

                if (entity.DoctorId == 0)
                {
                    var lastDoctor = db.Users.Where(x => x.UserName.StartsWith("DM") && x.RoleId == 4).OrderByDescending(x => x.UserName)
                        .Select(x => x.UserName).FirstOrDefault();

                    string newDoctorCode = "DM0001";

                    if (!string.IsNullOrEmpty(lastDoctor) && lastDoctor.Length > 3)
                    {
                        string numberPart = lastDoctor.Substring(3);
                        if (int.TryParse(numberPart, out int lastNumber))
                        {
                            newDoctorCode = $"DM{(lastNumber + 1):D4}";
                        }
                    }
                    entity.DoctorCode = newDoctorCode;
                    var newUser = new UserEntity
                    {
                        UserName = entity.DoctorCode,
                        UserFullName = entity.DoctorName,
                        RoleId = entity.RoleId,
                        HospitalId = jwt.HospitalId,
                        ContactNo = entity.ContactNo,
                        Email = entity.Email,
                        Gender = entity.Gender,
                        DOB = entity.DOB,
                        Status = "Active",
                        AddedBy = jwt.Id
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();
                    entity.UserId = newUser.Id;
                    db.Doctors.Add(entity);
                    db.SaveChanges();
                    msg = "Doctor saved successfully!";
                }
                else
                {
                    var existingDoctor = db.Doctors.AsNoTracking().FirstOrDefault(x => x.DoctorId == entity.DoctorId);
                    var prevData = JsonConvert.SerializeObject(existingDoctor);
                    if (existingDoctor != null)
                    {
                        existingDoctor.Qualification = entity.Qualification;
                        existingDoctor.DoctorType = entity.DoctorType;
                        existingDoctor.DoctorName = entity.DoctorName;
                        existingDoctor.DepartmentId = entity.DepartmentId;
                        existingDoctor.DesignationId = entity.DesignationId;
                        existingDoctor.SpecializationId1 = entity.SpecializationId1;
                        existingDoctor.SpecializationId2 = entity.SpecializationId2;
                        existingDoctor.ConsultationLimitPerDay = entity.ConsultationLimitPerDay;
                        existingDoctor.AdmissionLimit = entity.AdmissionLimit;
                        existingDoctor.DateOfJoining = entity.DateOfJoining;
                        existingDoctor.PaymentType = entity.PaymentType;
                        existingDoctor.Teleconsultation = entity.Teleconsultation;
                        existingDoctor.ConsultationType = entity.ConsultationType;
                        existingDoctor.ConsultingType = entity.ConsultingType;
                        existingDoctor.RegistrationNo = entity.RegistrationNo;
                        existingDoctor.IsHOD = string.IsNullOrEmpty(entity.IsHOD) ? "No" : entity.IsHOD;
                        existingDoctor.Title = entity.Title;
                        db.Doctors.Update(existingDoctor);
                        msg = "Doctor updated successfully!";


                        var existingUser = db.Users.Where(user => user.Id == existingDoctor.UserId).FirstOrDefault();
                        if (existingUser is not null)
                        {
                            existingUser.UserFullName = entity.DoctorName;
                            existingUser.Email = entity.Email;
                            existingUser.DOB = entity.DOB;
                            existingUser.ContactNo = entity.ContactNo;
                            existingUser.Gender = entity.Gender;
                            db.Users.Update(existingUser);
                        }

                        //_auditLog.Module = "Doctor";
                        //_auditLog.Action = "Update";
                        //_auditLog.PreviousData = prevData;
                        //_auditLog.ActionBy = jwt.Id;
                        //_auditLog.ActionDate = DateTime.Now;
                        //_auditLog.IPAddress = IpAddress;
                        //cl.SaveAuditLog(_auditLog);
                    }
                    else
                    {
                        return new Ret { status = false, message = "Doctor not found." };
                    }
                }
                db.SaveChanges();

                return new Ret { status = true, message = msg, data = new { DoctorId = entity.DoctorId } };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Failed to save Doctor." };
            }
        }
        public Ret SaveDoctorDetails(DoctorDetailsEntity entity, JwtStatus jwt, string IpAddress)
        {
            try
            {
                string msg = "";
                DoctorDetails doctorEntity = new DoctorDetails
                {
                    DoctorId = entity.DoctorId,
                    BloodGroup = entity.BloodGroup,
                    Religion = entity.Religion,
                    Nationality = entity.Nationality,
                    MaritalStatus = entity.MaritalStatus,
                    RelationPrefix = entity.RelationPrefixType,
                    RelationName = entity.RelationName,
                    RelationMobileNo = entity.RelationMobileNo,
                    AadharNo = entity.AadharNumber,
                    PanNo = entity.PanNumber,
                    UanoNo = entity.UanNumber,
                    ESINo = entity.ESINo,
                    Country = entity.Country,
                    AreaCode = entity.AreaCode,
                    Area = entity.Area,
                    Address = entity.Address,
                    IdProof = entity.IdProof,
                    IdNumber = entity.IdNumber,
                    CreatedBy = jwt.Id,
                    CreatedDate = entity.CreatedDate
                };
                if (entity.Id == 0)
                {
                    db.DoctorDetails.Add(doctorEntity);
                    db.SaveChanges();
                    SaveBankDetails(entity, jwt);
                    msg = "Doctor personal details saved successfully.";
                }
                else
                {
                    var _existingDetails = db.DoctorDetails.Where(details => details.DoctorId == entity.DoctorId).AsNoTracking().FirstOrDefault();
                    var prevData = JsonConvert.SerializeObject(_existingDetails);
                    _existingDetails.BloodGroup = entity.BloodGroup;
                    _existingDetails.Religion = entity.Religion;
                    _existingDetails.Nationality = entity.Nationality;
                    _existingDetails.MaritalStatus = entity.MaritalStatus;
                    _existingDetails.RelationPrefix = entity.RelationPrefixType;
                    _existingDetails.RelationName = entity.RelationName;
                    _existingDetails.RelationMobileNo = entity.RelationMobileNo;
                    _existingDetails.AadharNo = entity.AadharNumber;
                    _existingDetails.PanNo = entity.PanNumber;
                    _existingDetails.UanoNo = entity.UanNumber;
                    _existingDetails.ESINo = entity.ESINo;
                    _existingDetails.Country = entity.Country;
                    _existingDetails.AreaCode = entity.AreaCode;
                    _existingDetails.Area = entity.Area;
                    _existingDetails.Address = entity.Address;
                    _existingDetails.IdProof = entity.IdProof;
                    _existingDetails.IdNumber = entity.IdNumber;
                    _existingDetails.ModifyBy = jwt.Id;
                    _existingDetails.ModifyDate = DateTime.Now;

                    msg = "Doctor personal details updated successfully.";
                    var _auditLog = new AuditLog();

                    //_auditLog.Module = "Doctor Details";
                    //_auditLog.Action = "Update";
                    //_auditLog.PreviousData = prevData;
                    //_auditLog.ActionBy = jwt.Id;
                    //_auditLog.ActionDate = DateTime.Now;
                    //_auditLog.IPAddress = IpAddress;
                    //db.DoctorDetails.Update(_existingDetails);
                    //db.SaveChanges();
                    //cl.SaveAuditLog(_auditLog);
                    SaveBankDetails(entity, jwt);
                }


                return new Ret
                {
                    status = true,
                    message = msg,
                    data = new
                    {
                        DoctorId = entity.DoctorId
                    }
                };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Failed to save doctor personal details." };
            }
        }
        public Ret SaveBankDetails(DoctorDetailsEntity entity, JwtStatus jwtData)
        {
            try
            {
                EmployeeBankDetails employeeBankDetails = new EmployeeBankDetails
                {

                    EmpId = entity.DoctorId,
                    AccountType = entity.AccountType,
                    AccNo = entity.AccountNumber,
                    BankName = entity.BankName,
                    Branch = entity.Branch,
                    IFSCCode = entity.IfscCode,
                    IsPrimary = "Yes",
                    IsRemoved = "No",
                    RoleId = 4,
                    AddedBy = jwtData.Id,
                    CreatedDate = entity.CreatedDate
                };
                if (!db.EmployeeBankDetails.Any(bank => bank.EmpId == entity.DoctorId && bank.RoleId == 4))
                {
                    db.EmployeeBankDetails.Add(employeeBankDetails);
                    db.SaveChanges();
                }
                else
                {
                    var _existingDetails = db.EmployeeBankDetails.Where(empBank => empBank.EmpId == entity.DoctorId && empBank.RoleId == 4).AsNoTracking().FirstOrDefault();
                    var prevData = JsonConvert.SerializeObject(_existingDetails);

                    _existingDetails.AccountType = entity.AccountType;
                    _existingDetails.AccNo = entity.AccountNumber;
                    _existingDetails.BankName = entity.BankName;
                    _existingDetails.Branch = entity.Branch;
                    _existingDetails.IFSCCode = entity.IfscCode;
                    _existingDetails.UpdatedBy = jwtData.Id;
                    _existingDetails.UpdatedDate = DateTime.Now;
                    db.EmployeeBankDetails.Update(_existingDetails);
                    db.SaveChanges();
                }
                return new Ret { };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong." };
            }
        }
        public Ret GetPersonalDetails(int Id)
        {
            try
            {
                var doctorData = (from a in db.DoctorDetails
                                  join b in db.EmployeeBankDetails on a.DoctorId equals b.EmpId
                                  join c in db.PincodeData on a.AreaCode equals c.Id into pincodeData
                                  from pin in pincodeData.DefaultIfEmpty()
                                  join district in db.District on pin.DistrictId equals district.Id into districtData
                                  from dist in districtData.DefaultIfEmpty()
                                  join s in db.States on dist.StateId equals s.Id into states
                                  from state in states.DefaultIfEmpty()

                                  where a.DoctorId == Id && b.RoleId == 4
                                  select new
                                  {
                                      a.Id,
                                      a.DoctorId,
                                      a.BloodGroup,
                                      a.Religion,
                                      a.Nationality,
                                      a.MaritalStatus,
                                      RelationPrefixType = a.RelationPrefix,
                                      a.RelationName,
                                      a.RelationMobileNo,
                                      AadharNumber = a.AadharNo,
                                      PanNumber = a.PanNo,
                                      UanNumber = a.UanoNo,
                                      a.ESINo,
                                      a.Country,
                                      a.AreaCode,
                                      a.Area,
                                      a.Address,
                                      a.IdProof,
                                      a.IdNumber,
                                      a.CreatedBy,
                                      CreatedDate = Convert.ToDateTime(a.CreatedDate).ToString("yyyy-MM-dd"),
                                      a.ModifyBy,
                                      ModifyDate = Convert.ToDateTime(a.ModifyDate).ToString("yyyy-MM-dd"),
                                      b.AccountType,
                                      AccountNumber = b.AccNo,
                                      b.BankName,
                                      b.Branch,
                                      b.IFSCCode,
                                      b.IsPrimary,
                                      b.IsRemoved,
                                      dist.DistrictName,
                                      pin.DistrictId,
                                      dist.StateId,
                                      state.StateName
                                  })
                                .AsNoTracking().ToList();

                return new Ret { status = true, message = FetchMessage(doctorData, "Doctors personal details"), data = doctorData };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + ex.Message);
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }

        public Ret GetAllDoctors(Pagination entity, JwtStatus jwtData)
        {
            try
            {
                var query = (from d in db.Doctors
                             join e in db.Department on d.DepartmentId equals e.DepartmentId into depJoin
                             from f in depJoin.DefaultIfEmpty()
                             join g in db.Designation on d.DesignationId equals g.DesignationId into desJoin
                             from h in desJoin.DefaultIfEmpty()
                             join i in db.Users on d.UserId equals i.Id into users
                             from j in users.DefaultIfEmpty()
                             join k in db.Users on d.CreatedBy equals k.Id
                             join l in db.Users on d.UpdateBy equals l.Id into modifyUser
                             from m in modifyUser.DefaultIfEmpty()
                             where j.HospitalId == jwtData.HospitalId
                             orderby d.DoctorId descending
                             select new
                             {
                                 d.DoctorId,
                                 DoctorCode = j.UserName,
                                 d.DoctorType,
                                 d.Status,
                                 d.DoctorName,
                                 d.DepartmentId,
                                 d.DesignationId,
                                 f.DepartmentName,
                                 h.DesignationName,
                                 d.DoctorProfile,
                                 d.Qualification,
                                 DOB = Convert.ToDateTime(j.DOB).ToString("yyyy-MM-dd"),
                                 j.Email,
                                 j.ContactNo,
                                 j.Gender,
                                 CreatedDate = Convert.ToDateTime(d.CreatedDate).ToString("yyyy-MM-dd hh:mm:ss"),
                                 CreatedBy = k.UserName,
                                 UpdatedDate = Convert.ToDateTime(d.UpdatedDate).ToString("yyyy-MM-dd hh:mm:ss"),
                                 UpdatedBy = m.UserName,
                             })
                            .Where(a => entity.Id == 0 || a.DoctorId == entity.Id).AsNoTracking();

                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(a => a.DoctorName.Contains(entity.SearchKey) || a.DoctorCode.Contains(entity.SearchKey) || a.DepartmentName.Contains(entity.SearchKey));

                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Doctors"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret GetDoctorsList(Pagination entity, JwtStatus jwtData)
        {
            try
            {
                var query = (from d in db.Doctors
                             join e in db.Department on d.DepartmentId equals e.DepartmentId into depJoin
                             from f in depJoin.DefaultIfEmpty()
                             join g in db.Designation on d.DesignationId equals g.DesignationId into desJoin
                             from h in desJoin.DefaultIfEmpty()
                             join i in db.Users on d.UserId equals i.Id into users
                             from j in users.DefaultIfEmpty()
                             join k in db.Users on d.CreatedBy equals k.Id
                             join l in db.Users on d.UpdateBy equals l.Id into modifyUser
                             from m in modifyUser.DefaultIfEmpty()
                             where j.HospitalId == jwtData.HospitalId
                             orderby d.DoctorId descending
                             select new
                             {
                                 d.DoctorId,
                                 DoctorCode = j.UserName,
                                 d.DoctorType,
                                 d.Status,
                                 d.DoctorName,
                                 d.DepartmentId,
                                 d.DesignationId,
                                 f.DepartmentName,
                                 h.DesignationName,
                                 d.DoctorProfile,
                                 d.Qualification,
                                 DOB = Convert.ToDateTime(j.DOB).ToString("yyyy-MM-dd"),
                                 j.Email,
                                 j.ContactNo,
                                 j.Gender,
                                 CreatedDate = Convert.ToDateTime(d.CreatedDate).ToString("yyyy-MM-dd hh:mm:ss"),
                                 CreatedBy = k.UserName,
                                 UpdatedDate = Convert.ToDateTime(d.UpdatedDate).ToString("yyyy-MM-dd hh:mm:ss"),
                                 UpdatedBy = m.UserName

                             })
                            .Where(a => a.Status == "Active").AsNoTracking();

                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(a => a.DoctorName.Contains(entity.SearchKey) || a.DoctorCode.Contains(entity.SearchKey) || a.DepartmentName.Contains(entity.SearchKey));

                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Doctors"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }

        public Ret DoctorGetById(int Id)
        {
            try
            {
                var doctorData = (from a in db.Doctors.Where(a => a.DoctorId == Id)
                                  join b in db.Users on a.UserId equals b.Id
                                  select new
                                  {
                                      a.DoctorId,
                                      DoctorCode = b.UserName,
                                      a.DoctorType,
                                      a.Status,
                                      a.DoctorName,
                                      a.DepartmentId,
                                      a.DesignationId,
                                      a.DoctorProfile,
                                      a.Qualification,
                                      DOB = Convert.ToDateTime(b.DOB).ToString("yyyy-MM-dd"),
                                      b.Email,
                                      b.ContactNo,
                                      b.Gender,
                                      a.SpecializationId1,
                                      a.SpecializationId2,
                                      a.ConsultationLimitPerDay,
                                      a.AdmissionLimit,
                                      a.DateOfJoining,
                                      a.PaymentType,
                                      a.Teleconsultation,
                                      a.ConsultationType,
                                      a.ConsultingType,
                                      a.RegistrationNo,
                                      a.IsHOD,
                                      a.Title
                                  })
                                .AsNoTracking().ToList();


                return new Ret { status = true, message = FetchMessage(doctorData, "Doctors"), data = doctorData };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + ex.Message);
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret SaveDoctorEducation([FromForm] DoctorEducationUpload entity, JwtStatus jwtData)
        {
            try
            {

                string extension = "";
                string UploadFileName = "";
                if (entity.id == 0)
                {
                    if (entity.certificate != null)
                    {
                        extension = Path.GetExtension(entity.certificate.FileName.ToString());
                        if (!Directory.Exists(Path.GetFullPath("Uploads/DoctorEducation/")))
                        {
                            Directory.CreateDirectory(Path.GetFullPath("Uploads/DoctorEducation/"));
                        }

                        UploadFileName = Path.GetFileNameWithoutExtension(entity.certificate.FileName.ToString()) + "_Education" + entity.doctorId;
                        string NewFileNameWithFullPath = Path.GetFullPath("Uploads/DoctorEducation/" + UploadFileName + extension).Replace("~\\", "");
                        bool uploadstatus = cl.upload(entity.certificate, NewFileNameWithFullPath);
                    }
                    DoctorEducation edu = new DoctorEducation
                    {

                        DoctorId = entity.doctorId,
                        DegreeName = entity.degreeName,
                        CourseType = entity.courseType,
                        SpecializationId = entity.specializationId,
                        University = entity.university,
                        Country = entity.country,
                        YearofPass = entity.yearofPass,
                        MedicalCouncilName = entity.medicalCouncilName,
                        CertificateName = UploadFileName + extension,
                        CreatedBy = jwtData.Id,
                        CreatedDate = entity.createdDate

                    };

                    db.DoctorEducation.Add(edu);
                    db.SaveChanges();
                    return new Ret { status = true, message = "Doctor Education added successfully!" };
                }
                return new Ret { status = false, message = "Failed to save doctor education." };
            }
            catch (Exception ex)
            {
                Log.Error("Error at " + DateTime.Now.ToString() + " - " + ex.Message);
                return new Ret { status = false, message = "Something went wrong." };
            }
        }
        public Ret RemoveEducationDetails(int Id, string IpAddress, JwtStatus jwtData)
        {
            try
            {
                var _existingDetails = db.DoctorEducation.Where(edu => edu.Id == Id).FirstOrDefault();
                var prevData = JsonConvert.SerializeObject(_existingDetails);
                var FilePath = Path.GetFullPath("Uploads/DoctorEducation/" + _existingDetails.CertificateName);
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                    db.DoctorEducation.Remove(_existingDetails);
                    db.SaveChanges();
                    //AuditLog _auditLog = new AuditLog
                    //{
                    //    Module = "Doctor Education",
                    //    IPAddress = IpAddress,
                    //    Action = "Delete",
                    //    ActionBy = jwtData.Id,
                    //    ActionDate = DateTime.Now,
                    //    PreviousData = prevData
                    //};
                    //db.AuditLog.Add(_auditLog);
                    //db.SaveChanges();
                }
                return new Ret { status = true, message = "Education data removed successfully." };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong" };
            }
        }
        public Ret SaveDoctorExperience(DoctorExperienceUpload entity, JwtStatus jwtData)
        {
            try
            {

                string extension = "";
                string UploadFileName = "";
                if (entity.id == 0)
                {
                    if (entity.certificate != null)
                    {
                        extension = Path.GetExtension(entity.certificate.FileName.ToString());
                        if (!Directory.Exists(Path.GetFullPath("Uploads/DoctorExperience/")))
                        {
                            Directory.CreateDirectory(Path.GetFullPath("Uploads/DoctorExperience/"));
                        }

                        UploadFileName = Path.GetFileNameWithoutExtension(entity.certificate.FileName.ToString()) + "_Experience" + entity.doctorId;
                        string NewFileNameWithFullPath = Path.GetFullPath("Uploads/DoctorExperience/" + UploadFileName + extension).Replace("~\\", "");
                        bool uploadstatus = cl.upload(entity.certificate, NewFileNameWithFullPath);
                    }
                    DoctorExperience experience = new DoctorExperience
                    {

                        DoctorId = entity.doctorId,
                        Designation = entity.designation,
                        SpecializationId = entity.specializationId,
                        HospitalName = entity.hospitalName,
                        Location = entity.location,
                        EmploymentType = entity.employmentType,
                        StartDate = entity.StartDate,
                        EndDate = entity.endDate,
                        CertificateName = UploadFileName + extension,
                        CreatedBy = jwtData.Id,
                        CreatedDate = DateTime.Now
                    };

                    db.DoctorExperience.Add(experience);
                    db.SaveChanges();
                    return new Ret { status = true, message = "Doctor experience added successfully!" };
                }
                return new Ret { status = false, message = "Failed to save doctor experience." };
            }
            catch (Exception ex)
            {
                Log.Error("Error at " + DateTime.Now.ToString() + " - " + ex.Message);
                return new Ret { status = false, message = "Something went wrong." };
            }
        }


        public Ret GetAllDoctorEducation(int Id)
        {
            try
            {
                var res = (from a in db.DoctorEducation
                           join b in db.Specializations on a.SpecializationId equals b.SpecializationId
                           where a.Id == 0 || a.DoctorId == Id
                           select new
                           {
                               a.Id,
                               a.DoctorId,
                               a.DegreeName,
                               a.CourseType,
                               a.SpecializationId,
                               a.University,
                               a.Country,
                               a.YearofPass,
                               a.MedicalCouncilName,
                               a.CertificateName,
                               a.CreatedBy,
                               CreatedDate = Convert.ToDateTime(a.CreatedDate).ToString("yyyy-MM-dd"),
                               a.ModifyBy,
                               ModifyDate = Convert.ToDateTime(a.ModifyDate).ToString("yyyy-MM-dd"),
                               b.SpecializationName,
                               a.RegistrationNumber
                           })
                    .AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Doctor Education"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret GetAllDoctorExperience(int Id)
        {
            try
            {
                var res = (from a in db.DoctorExperience
                           join b in db.Specializations on a.SpecializationId equals b.SpecializationId
                           where a.Id == 0 || a.DoctorId == Id
                           select new
                           {
                               a.Id,
                               a.DoctorId,
                               a.Designation,
                               a.SpecializationId,
                               a.HospitalName,
                               a.Location,
                               a.EmploymentType,
                               StartDate = Convert.ToDateTime(a.StartDate).ToString("yyyy-MM-dd"),
                               EndDate = Convert.ToDateTime(a.EndDate).ToString("yyyy-MM-dd"),
                               a.CertificateName,
                               a.CreatedBy,
                               CreatedDate = Convert.ToDateTime(a.CreatedDate).ToString("yyyy-MM-dd"),
                               a.ModifyBy,
                               ModifyDate = Convert.ToDateTime(a.ModifyDate).ToString("yyyy-MM-dd"),
                               b.SpecializationName,

                           })
                    .AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Doctor Education"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret RemoveExperienceDetails(int Id, string IpAddress, JwtStatus jwtData)
        {
            try
            {
                var _existingDetails = db.DoctorExperience.Where(edu => edu.Id == Id).FirstOrDefault();
                var prevData = JsonConvert.SerializeObject(_existingDetails);
                var FilePath = Path.GetFullPath("Uploads/DoctorExperience/" + _existingDetails.CertificateName);
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                    db.DoctorExperience.Remove(_existingDetails);
                    db.SaveChanges();
                    //AuditLog _auditLog = new AuditLog
                    //{
                    //    Module = "Doctor Experience",
                    //    IPAddress = IpAddress,
                    //    Action = "Delete",
                    //    ActionBy = jwtData.Id,
                    //    ActionDate = DateTime.Now,
                    //    PreviousData = prevData
                    //};
                    //db.AuditLog.Add(_auditLog);
                    //db.SaveChanges();
                }
                return new Ret { status = true, message = "Experience data removed successfully." };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong" };
            }
        }
        public Ret SaveDoctorProfile(DoctorProfileUpload profilephoto)
        {
            try
            {
                CommonLogic CL = new CommonLogic();
                string UploadFileName = "";
                string extension = "";

                if (profilephoto.DoctorProfile != null)
                {
                    extension = Path.GetExtension(profilephoto.DoctorProfile.FileName.ToString());
                    if (!Directory.Exists(Path.GetFullPath("Uploads/DoctorProfile/")))
                    {
                        Directory.CreateDirectory(Path.GetFullPath("Uploads/DoctorProfile/"));
                    }

                    UploadFileName = Path.GetFileNameWithoutExtension(profilephoto.DoctorProfile.FileName.ToString()) + "_DoctorProfile" + profilephoto.Id;
                    string NewFileNameWithFullPath = Path.GetFullPath("Uploads/DoctorProfile/" + UploadFileName + extension).Replace("~\\", "");


                    bool uploadstatus = CL.upload(profilephoto.DoctorProfile, NewFileNameWithFullPath);
                }

                var users = db.Doctors.Where(x => x.DoctorId == profilephoto.Id).FirstOrDefault();
                users.DoctorProfile = UploadFileName + extension.ToString();
                db.SaveChanges();

                return new Ret { status = true, message = SaveSuccessMessage(1, "Profile Photo"), data = users.DoctorProfile };

            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }

        public Ret GetAllDoctorLabel(int Id, JwtStatus jwtData)
        {
            try
            {
                var _doctorList = (from a in db.Doctors
                                   join b in db.Department on a.DepartmentId equals b.DepartmentId
                                   join c in db.Users on a.UserId equals Convert.ToInt32(c.Id)
                                   where c.HospitalId == jwtData.HospitalId &&
                                   (Id == 0 || a.DoctorId == Id)
                                   select new
                                   {
                                       value = a.DoctorId,
                                       label = a.DoctorName,
                                       departmentId = a.DepartmentId,
                                       departmentName = b.DepartmentName,
                                       a.Teleconsultation
                                   }).AsNoTracking().ToList();

                return new Ret { status = true, message = FetchMessage(_doctorList, "Doctor"), data = _doctorList };
            }
            catch (Exception ex)
            {
                Log.Information("Error at " + DateTime.Now.ToString() + " message " + ex.Message);
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret GetOpConsultationByDoctorId(Pagination entity, JwtStatus JwtData)
        {
            try
            {
                var query = (from a in db.OpConsultation
                             join c in db.Patient on a.PatientId equals c.PatientId
                             join d in db.Users on c.UserId equals d.Id
                             join e in db.Doctors on a.DoctorId equals e.DoctorId
                             join f in db.Users on e.UserId equals f.Id
                             join g in db.DoctorPrescription on a.ConsultationId equals g.ConsultationId into prescription
                             from h in prescription.DefaultIfEmpty()
                             where f.Id == JwtData.Id
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
                                 IsAudApprove = a.IsAudApprove ?? "No",
                                 Investigation = (h.Investigations != null && h.Investigations == "[]" || h.Investigations == null) ? "No" : "Yes",
                                 Medication = (h.Medication != null && h.Medication == "[]" || h.Medication == null) ? "No" : "Yes"
,
                             }).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(a => a.PatientName.Contains(entity.SearchKey));

                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Op consultation"), data = res, totalCount = totalCount };

            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong!" };
            }
        }
        public Ret GetOpConsultationByNurseId(PrescriptionFilter entity, JwtStatus JwtData)
        {
            try
            {
                var result = (from a in db.OpConsultation
                              join c in db.Patient on a.PatientId equals c.PatientId
                              join d in db.Users on c.UserId equals d.Id
                              join e in db.Doctors on a.DoctorId equals e.DoctorId
                              join f in db.Users on e.UserId equals f.Id
                              join g in db.DoctorPrescription on a.ConsultationId equals g.ConsultationId into prescription
                              from h in prescription.DefaultIfEmpty()
                              where e.AssistantId == JwtData.Id
                                 && a.ConsultationDate.HasValue
                                 && a.ConsultationDate.Value.Date >= entity.FromDate.Date
                                 && a.ConsultationDate.Value.Date <= entity.ToDate.Date
                              orderby a.ConsultationId descending
                              select new
                              {
                                  ConsultationId = a.ConsultationId,
                                  ConsultationNo = a.ConsultationNo ?? "",
                                  a.PatientId,
                                  a.RefNo,
                                  LastValidityDate = a.LastValidityDate.HasValue ? a.LastValidityDate.Value.Date : (DateTime?)null,
                                  a.VisitType,
                                  a.PaymentBy,
                                  OrganizationId = a.OrganizationId ?? 0,
                                  a.DoctorId,
                                  a.ConsultantFee,
                                  a.Visit,
                                  a.ReferralType,
                                  ReferredBy = a.ReferredBy ?? "",
                                  a.CreatedBy,
                                  a.Status,
                                  ConstultationDate = a.ConsultationDate.HasValue
                                      ? a.ConsultationDate.Value.ToString("yyyy-MM-dd hh:mm tt")
                                      : "",
                                  a.CreatedDate,
                                  PatientName = d.UserFullName ?? "",
                                  umrNumber = d.UserName ?? "",
                                  ContactNo = d.ContactNo ?? "",
                                  DOB = d.DOB.HasValue ? d.DOB.Value.ToString("yyyy-MM-dd") : "",
                                  DoctorName = f.UserFullName ?? "",
                                  DoctorCode = f.UserName ?? "",
                                  Gender = d.Gender ?? "",
                                  Age = c.Age ?? "",
                                  IsReqforCancel = (a.CancelBy.HasValue && a.CancelBy.Value > 0 && a.Status == "Booked") ? "Yes" : "No",
                                  IsDocApproved = a.IsDocApproved ?? "No",
                                  IsAudApprove = a.IsAudApprove ?? "No",
                                  Investigation = string.IsNullOrWhiteSpace(h.Investigations) || h.Investigations == "[]" ? "No" : "Yes",
                                  Medication = string.IsNullOrWhiteSpace(h.Medication) || h.Medication == "[]" ? "No" : "Yes"
                              }).ToList();

                var op = result.AsQueryable();

                return new Ret { status = true, message = FetchMessage(result, "Op consultation"), data = result };

            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong!" };
            }
        }

        public Ret ScheduleAvailability(JsonEntity je, JwtStatus jwtData)
        {
            try
            {
                DBHelper _dbHelper = new DBHelper(db.GetConnectionString());
                JsonObject json = JsonObject.Parse(System.Text.Json.JsonSerializer.Serialize(je.JsonString));
                json["hospitalId"] = jwtData.HospitalId.ToString() ?? "0";
                SqlParameter[] objParams = new SqlParameter[2];
                objParams[0] = new SqlParameter("@JsonString", json.ToString());
                objParams[1] = new SqlParameter("@OUTPUT", SqlDbType.NVarChar, 100);
                objParams[1].Direction = ParameterDirection.Output;
                object obj = _dbHelper.ExecuteNonQuerySP("PKG_SETSCHEDULEAVAILABILITY", objParams, true);
                if (obj != null && obj.ToString().Equals("Success"))
                {
                    return new Ret { status = true, message = "Set availability saved successfully!!" };
                }
                else
                {
                    return new Ret { status = false, message = "Failed to save availability!!" };
                }
            }
            catch (Exception ex)
            {
                Log.Information("scheduleModel at :" + DateTime.Now + " Error :" + ex.Message);
                return new Ret { status = false, message = "Failed to save availability!!" };
            }
        }//
        public Ret GetAvailability(JsonEntity je, JwtStatus jwtData)
        {
            try
            {
                DBHelper _dbHelper = new DBHelper(db.GetConnectionString());
                SqlParameter[] parameters = new SqlParameter[5];
                JsonObject json = JsonObject.Parse(System.Text.Json.JsonSerializer.Serialize(je.JsonString));
                parameters[0] = new SqlParameter("@HospitalId", jwtData.HospitalId.ToString());
                parameters[1] = new SqlParameter("@DoctorId", json.ContainsKey("doctorId") ? json["doctorId"]?.ToString() : "ALL");
                parameters[2] = new SqlParameter("@Activity", json.ContainsKey("activity") ? json["activity"]?.ToString() : "ALL");
                parameters[3] = new SqlParameter("@Date", json.ContainsKey("date") ? json["date"]?.ToString() : "ALL");
                parameters[4] = new SqlParameter("@Type", json.ContainsKey("type") ? json["type"]?.ToString() : "ALL");
                DataSet ds = _dbHelper.ExecuteDataSetSP("hth_GetSchedule", parameters);
                if (ds is not null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return new Ret { status = true, data = CommonLogic.GetJsonObject(ds), message = "Clinician availability loaded successfully!!" };
                }
                else
                {
                    return new Ret { status = false, message = "Failed to load clinician availability details" };
                }
            }
            catch (Exception ex)
            {
                Log.Information("Clinician Model => GetAvailability Method error at => " + DateTime.Now.ToString() + " Error Message=>" + ex.Message);
                return new Ret { status = false, message = "Something went wrong,Please try again!!" };
            }

        }//End

        public Ret GetAvailabilityWithAppointment(JsonEntity je)
        {
            try
            {
                DBHelper _dbHelper = new DBHelper(db.GetConnectionString());
                SqlParameter[] parameters = new SqlParameter[5];
                JsonObject json = (JsonObject)System.Text.Json.JsonSerializer.Serialize(je);
                parameters[0] = new SqlParameter("@OrganizationId", json["organizationId"]?.ToString());
                parameters[1] = new SqlParameter("@ClinicianId", json.ContainsKey("clinicianId") ? json["clinicianId"]?.ToString() : "ALL");
                parameters[2] = new SqlParameter("@Activity", json.ContainsKey("activity") ? json["activity"]?.ToString() : "ALL");
                parameters[3] = new SqlParameter("@Date", json.ContainsKey("date") ? json["date"]?.ToString() : "ALL");
                parameters[4] = new SqlParameter("@Type", json.ContainsKey("type") ? json["type"]?.ToString() : "ALL");
                DataSet ds = _dbHelper.ExecuteDataSetSP("hth_GetSchedule_dublicate", parameters);
                if (ds is not null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return new Ret { status = true, data = CommonLogic.GetJsonObject(ds), message = "Clinician availability loaded successfully!!" };
                }
                else
                {
                    return new Ret { status = false, message = "Failed to load clinician availability details" };
                }
            }
            catch (Exception ex)
            {
                Log.Information("Clinician Model => GetAvailabilityWithAppointment Method error at => " + DateTime.Now.ToString() + " Error Message=>" + ex.Message);
                return new Ret { status = false, message = "Something went wrong,Please try again!!" };
            }

        }//End


        public Ret ScheduleStatusUpdate(JsonEntity je, JwtStatus jwtData)
        {
            try
            {
                DBHelper _dbHelper = new DBHelper(db.GetConnectionString());
                SqlParameter[] parameters = new SqlParameter[1];
                JsonObject json = (JsonObject)(System.Text.Json.JsonSerializer.Serialize(je.JsonString));

                json["addedBy"] = jwtData.Id;
                json["hospitalId"] = jwtData.HospitalId;
                parameters[0] = new SqlParameter("@JsonString", json.ToString());

                DataSet ds = _dbHelper.ExecuteDataSetSP("PKG_SCHEDULE_STATUS", parameters);
                return new Ret { status = Convert.ToBoolean(ds.Tables[0].Rows[0]["status"].ToString()), message = ds.Tables[0].Rows[0]["message"].ToString() };
            }
            catch (Exception ex)
            {
                Log.Information("Clinician Model => ScheduleStatusUpdate Method error at => " + DateTime.Now.ToString() + " Error Message=>" + ex.Message);
                return new Ret { status = false, message = "Something went wrong,Please try again!!" };
            }
        }//End
        public Ret GetAvailabilityWithAppointmentMobile(JsonEntity je, JwtStatus jwtData)
        {
            try
            {
                DBHelper _dbHelper = new DBHelper(db.GetConnectionString());
                SqlParameter[] parameters = new SqlParameter[4];
                JsonObject json = JsonObject.Parse(System.Text.Json.JsonSerializer.Serialize(je.JsonString));
                //JsonObject json = CommonLogic.GetTokenValues(je, _token);
                //parameters[0] = new SqlParameter("@HospitalId", jwtData.HospitalId);
                parameters[0] = new SqlParameter("@DoctorId", json.ContainsKey("doctorId") ? json["doctorId"]?.ToString() : "ALL");
                parameters[1] = new SqlParameter("@Activity", json.ContainsKey("activity") ? json["activity"]?.ToString() : "ALL");
                parameters[2] = new SqlParameter("@Date", json.ContainsKey("date") ? json["date"]?.ToString() : "ALL");
                parameters[3] = new SqlParameter("@Type", json.ContainsKey("type") ? json["type"]?.ToString() : "ALL");
                DataSet ds = _dbHelper.ExecuteDataSetSP("hth_GetSchedule_Mobile", parameters);
                if (ds is not null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return new Ret { status = true, data = CommonLogic.GetJsonObject(ds), message = "Doctor availability loaded successfully!!" };
                }
                else
                {
                    return new Ret { status = false, message = "Failed to load clinician availability details" };
                }
            }
            catch (Exception ex)
            {
                Log.Information("Clinician Model => GetAvailabilityWithAppointment Method error at => " + DateTime.Now.ToString() + " Error Message=>" + ex.Message);
                return new Ret { status = false, message = "Something went wrong,Please try again!!" };
            }

        }//End

        public Ret GetAllHospitalDoctorsList()
        {
            try
            {
                var query = (from d in db.Doctors
                             join e in db.Department on d.DepartmentId equals e.DepartmentId into depJoin
                             from f in depJoin.DefaultIfEmpty()
                             join g in db.Designation on d.DesignationId equals g.DesignationId into desJoin
                             from h in desJoin.DefaultIfEmpty()
                             join i in db.Users on d.UserId equals i.Id into users
                             from j in users.DefaultIfEmpty()
                             join k in db.Users on d.CreatedBy equals k.Id
                             join l in db.Users on d.UpdateBy equals l.Id into modifyUser
                             from m in modifyUser.DefaultIfEmpty()
                             join hos in db.Hospital on j.HospitalId equals hos.HospitalId
                             where j.Status == "Active"
                             orderby d.DoctorId descending
                             select new
                             {
                                 d.DoctorId,
                                 DoctorCode = j.UserName,
                                 d.DoctorType,
                                 d.Status,
                                 d.DoctorName,
                                 d.DepartmentId,
                                 d.DesignationId,
                                 f.DepartmentName,
                                 h.DesignationName,
                                 d.DoctorProfile,
                                 d.Qualification,
                                 DOB = Convert.ToDateTime(j.DOB).ToString("yyyy-MM-dd"),
                                 j.Email,
                                 j.ContactNo,
                                 j.Gender,
                                 CreatedDate = Convert.ToDateTime(d.CreatedDate).ToString("yyyy-MM-dd hh:mm:ss"),
                                 CreatedBy = k.UserName,
                                 UpdatedDate = Convert.ToDateTime(d.UpdatedDate).ToString("yyyy-MM-dd hh:mm:ss"),
                                 UpdatedBy = m.UserName,
                                 hos.HospitalId,
                                 hos.HospitalName

                             })
                            .Where(a => a.Status == "Active").AsNoTracking();


                return new Ret { status = true, message = FetchMessage(query, "Doctors"), data = query };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }

    }
}
