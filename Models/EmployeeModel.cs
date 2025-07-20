using Microsoft.Data.SqlClient;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Serilog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using Azure.Identity;
using System.Text.Json;
using Newtonsoft.Json;
using Pinnacle.Helpers;
using FastMember;
using System.Net.NetworkInformation;

namespace Pinnacle.Models
{
    public class EmployeeModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();

        public Ret GenerateEmployeeId(string MedicalTestRequired)
        {
            var MedicalTest = new SqlParameter("@MedicalTestRequired", MedicalTestRequired);

            var res = db.PKG_GenerateEmployeeId.FromSqlRaw("exec PKG_GenerateEmployeeId @MedicalTestRequired", MedicalTest).ToList();

            return new Ret { status = true, message = FetchMessage(res, "EmployeeId"), data = res.FirstOrDefault() };
        }
        public Ret SaveEmployeeBasicDetails(UserEntity entity, JwtStatus jwtData)
        {
            try
            {
                entity.AddedBy = jwtData.Id;
                entity.HospitalId = jwtData.HospitalId;
                //entity.IsMedicalCheckUpStatus = entity.IsMedicalTestRequired == "Yes" ? "Pending" : "";
                if (entity.Id == 0)
                {
                    entity.UserName = string.IsNullOrEmpty(entity.TempEmpCode) ? "TEMP00001" : entity.TempEmpCode;
                    db.Users.Add(entity);
                    db.SaveChanges();

                    var newEmployee = new EmployeeEntity
                    {
                        IsMedicalCheckUpStatus = entity.IsMedicalTestRequired == "Yes" ? "Pending" : "",
                        UserId = entity.Id,
                        TempEmpCode = entity.TempEmpCode
                    };

                    db.Employee.Add(newEmployee);
                    db.SaveChanges();
                    //}
                    return new Ret
                    {
                        status = true,
                        message = "Employee record added successfully! & User record also created."

                    };
                }
                else
                {
                    var resData = db.Users.AsNoTracking().FirstOrDefault(h => h.Id == entity.Id);

                    if (resData != null)
                    {
                        //entity.Address = JsonConvert.SerializeObject(entity.Address);
                        db.Users.Update(entity);
                        db.SaveChanges();
                        return new Ret { status = true, message = "Employee updated successfully!" };
                    }
                    else
                    {
                        return new Ret { status = false, message = "Employee record not found for update." };
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error at " + DateTime.Now.ToString() + " - " + ex.Message);
                return new Ret { status = false, message = "Failed to save employee record." };
            }
        }

        public Ret SaveEmployeeDetails(EmployeeDetails entity)
        {
            try
            {
                if (entity.EmpId == 0)
                {

                }
                else if (entity.EmpId > 0)
                {
                    var _existingemployee = db.Employee.Where(emp => emp.EmpId == entity.EmpId).AsNoTracking().FirstOrDefault();
                    _existingemployee.DateOfJoining = entity.DateOfJoining;
                    _existingemployee.EmployeeCode = entity.EmployeeCode;
                    _existingemployee.DepartmentId = entity.DepartmentId;
                    _existingemployee.DesignationId = entity.DesignationId;
                    _existingemployee.SubDepartmentId = entity.SubDepartmentId;
                    _existingemployee.CostDeptId = entity.CostDeptId;
                    _existingemployee.DesignationId = entity.DesignationId;
                    _existingemployee.EmployeeType = entity.EmployeeType;
                    _existingemployee.EmployeeCadre = entity.EmployeeCadre;
                    _existingemployee.EmployeeCategory = entity.EmployeeCategory;
                    _existingemployee.EmpShift = entity.EmpShift;
                    _existingemployee.WeekOff = entity.WeekOff;
                    _existingemployee.PaymentMode = entity.PaymentMode;
                    _existingemployee.BiometricrefNo = entity.BiometricrefNo;
                    _existingemployee.MemberId = entity.MemberId;
                    _existingemployee.EmployeelivingType = entity.EmployeelivingType;
                    _existingemployee.Salary = entity.Salary;
                    _existingemployee.Stipend = entity.Stipend;
                    _existingemployee.EmpStatus = entity.EmpStatus;
                    _existingemployee.TerminatedDate = entity.TerminatedDate;
                    _existingemployee.ResignedDate = entity.ResignedDate;
                    _existingemployee.RelievedDate = entity.RelievedDate;

                    db.Employee.Update(_existingemployee);

                    var _existingUser = db.Users.Where(emp => emp.Id == _existingemployee.UserId).AsNoTracking().FirstOrDefault();
                    _existingUser.TitleId = entity.TitleId;
                    _existingUser.UserFullName = entity.UserFullName;
                    _existingUser.DOB = entity.DOB;
                    _existingUser.Gender = entity.Gender;
                    _existingUser.ContactNo = entity.ContactNo;
                    _existingUser.Email = entity.Email;
                    db.Users.Update(_existingUser);
                    db.SaveChanges();
                    return new Ret { status = true, message = "Employee details updated successfully!" };

                }
                return new Ret { };
            }
            catch (Exception ex)
            {
                return new Ret { };
            }
        }

        public Ret EmployeeHospitalMapping(int EmpId, string HospitalIds)
        {
            try
            {
                return new Ret { };
            }
            catch (Exception ex)
            {
                return new Ret { };
            }
        }

        public Ret EmployeeGetById(int Id)
        {
            try
            {
                var res = (from a in db.Employee.Where(emp => emp.UserId == Id)
                           join b in db.Users on a.UserId equals b.Id
                           select new
                           {
                               a.EmpId,
                               a.EmployeeCode,
                               a.EmployeeType,
                               a.DateOfJoining,
                               a.ReligionId,
                               a.MaritalStatus,
                               a.DepartmentId,
                               a.DesignationId,
                               Id = a.UserId,
                               //a.MedicalTestRequired,
                               a.TempEmpCode,
                               a.DateofInitiation,
                               a.UserIdRequired,
                               a.ProfileMaintain,
                               a.Qualification,
                               a.PreviousExperience,
                               a.BloodGroups,
                               a.Nationality,
                               a.Caste,
                               a.EmployeelivingType,
                               a.RelationPrefixType,
                               a.RelationName,
                               a.PanNumber,
                               a.UanNumber,
                               a.AadharNumber,
                               a.AccountType,
                               a.AccountNumber,
                               a.BankName,
                               a.PfNumber,
                               a.BranchCode,
                               a.IFSCCode,
                               a.EmpProfile,
                               a.IsMedicalCheckUpStatus,
                               a.IsSystemNeedAccess,
                               a.JobNatureId,
                               a.EmployeeCadre,
                               b.UserName,
                               b.UserFullName,
                               b.TitleId,
                               b.Email,
                               b.Gender,
                               DOB = Convert.ToDateTime(b.DOB).ToString("yyyy-MM-dd"),
                               b.Password,
                               b.ContactNo,
                               b.Status,
                               b.AddedBy,
                               b.OrganizationId,
                               b.UserProfileId,
                               b.UserId,
                               b.RoleId,
                               b.HospitalId,
                               b.CreatedDate,
                               b.UpdatedBy,
                               b.UpdatedDate,
                               b.IsMedicalTestRequired,
                           }).AsNoTracking().ToList();
                return new Ret
                {
                    status = true,
                    message = FetchMessage(res, "Employee"),
                    data = res
                };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret StatusUpdate(StatusUpdate entity)
        {
            try
            {
                var res = db.Users.Where(x => x.Id == entity.Id).FirstOrDefault();
                res.Status = entity.Status;
                db.SaveChanges();
                return new Ret { status = true, message = UpdateStatusMessage("Employee") };

            }
            catch
            {
                return new Ret { status = false, message = NoDataMessage() };
            }
        }
        public Ret SaveEmployeeEducation(EmployeeEducation entity)
        {
            try
            {


                if (entity.Id == 0)
                {
                    db.EmployeeEducation.Add(entity);
                    db.SaveChanges();
                    return new Ret { status = true, message = "Employee Education added successfully!" };
                }
                else
                {
                    var resData = db.EmployeeEducation.AsNoTracking().FirstOrDefault(h => h.Id == entity.Id);

                    if (resData != null)
                    {
                        db.EmployeeEducation.Update(entity);
                        db.SaveChanges();
                        return new Ret { status = true, message = "Employee Education updated successfully!" };
                    }
                    else
                    {
                        return new Ret { status = false, message = "Employee  Education record not found for update." };
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error at " + DateTime.Now.ToString() + " - " + ex.Message);
                return new Ret { status = false, message = "Failed to save employee record." };
            }
        }
        public Ret SaveEmployeeExperience(EmployeeExperience entity)
        {
            try
            {
                if (entity.Id == 0)
                {
                    db.EmployeeExperience.Add(entity);
                    db.SaveChanges();
                    return new Ret { status = true, message = "Employee Experience added successfully!" };
                }
                else
                {
                    var resData = db.EmployeeExperience.AsNoTracking().FirstOrDefault(h => h.Id == entity.Id);

                    if (resData != null)
                    {
                        db.EmployeeExperience.Update(entity);
                        db.SaveChanges();
                        return new Ret { status = true, message = "Employee Experience updated successfully!" };
                    }
                    else
                    {
                        return new Ret { status = false, message = "Employee  Experience record not found for update." };
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error at " + DateTime.Now.ToString() + " - " + ex.Message);
                return new Ret { status = false, message = "Failed to save employee record." };
            }
        }
        public Ret GetAllEducation(int Id)
        {
            try
            {
                var res = db.EmployeeEducation.Where(a => a.EmpId == Id).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Employee Education"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret GetAllExperience(int Id)
        {
            try
            {
                var res = db.EmployeeExperience.Where(a => a.EmpId == Id).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Employee Experience"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }

        public Ret SaveEmployeeAddress(EmployeeAddress entity, JwtStatus jwtData)
        {
            try
            {
                entity.AddedBy = jwtData.Id;

                var existingAddress = db.EmployeeAddress.FirstOrDefault(h => h.EmpId == entity.EmpId);

                if (existingAddress != null)
                {

                    entity.Id = existingAddress.Id;
                    db.Entry(existingAddress).CurrentValues.SetValues(entity);
                    db.SaveChanges();
                    return new Ret { status = true, message = "Employee Address updated successfully!" };
                }
                else
                {
                    db.EmployeeAddress.Add(entity);
                    db.SaveChanges();
                    return new Ret { status = true, message = "Employee Address added successfully!" };
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error at {DateTime.Now} - {ex.Message}");
                return new Ret { status = false, message = "Failed to save employee address." };
            }
        }

        public Ret GetAllEmployee(Pagination entity, JwtStatus jwtData)
        {
            try
            {
                var query = (from a in db.Employee
                             join b in db.Users on a.UserId equals b.Id
                             where b.HospitalId == jwtData.HospitalId
                             select new
                             {
                                 a.EmpId,
                                 a.EmployeeCode,
                                 a.EmployeeType,
                                 a.DateOfJoining,
                                 a.ReligionId,
                                 a.MaritalStatus,
                                 a.DepartmentId,
                                 a.DesignationId,
                                 Id = a.UserId,
                                 //a.MedicalTestRequired,
                                 a.TempEmpCode,
                                 a.DateofInitiation,
                                 a.UserIdRequired,
                                 a.ProfileMaintain,
                                 a.Qualification,
                                 a.PreviousExperience,
                                 a.BloodGroups,
                                 a.Nationality,
                                 a.Caste,
                                 a.EmployeelivingType,
                                 a.RelationPrefixType,
                                 a.RelationName,
                                 a.PanNumber,
                                 a.UanNumber,
                                 a.AadharNumber,
                                 a.AccountType,
                                 a.AccountNumber,
                                 a.BankName,
                                 a.PfNumber,
                                 a.BranchCode,
                                 a.IFSCCode,
                                 a.EmpProfile,
                                 a.IsMedicalCheckUpStatus,
                                 a.IsSystemNeedAccess,
                                 a.JobNatureId,
                                 a.EmployeeCadre,
                                 b.UserName,
                                 b.UserFullName,
                                 b.TitleId,
                                 b.Email,
                                 b.Gender,
                                 DOB = Convert.ToDateTime(b.DOB).ToString("yyyy-MM-dd"),
                                 b.Password,
                                 b.ContactNo,
                                 b.Status,
                                 b.AddedBy,
                                 b.OrganizationId,
                                 b.UserProfileId,
                                 b.UserId,
                                 b.RoleId,
                                 b.HospitalId,
                                 b.CreatedDate,
                                 b.UpdatedBy,
                                 b.UpdatedDate,
                                 b.IsMedicalTestRequired,
                             }).AsNoTracking();

                //var query = db.Employee.Where(a => entity.Id == 0 || a.EmpId == entity.Id).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.UserFullName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Employee"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret AddressGetById(int Id)
        {
            try
            {
                var res = db.EmployeeAddress.Where(a => a.EmpId == Id).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Employee Address"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret GetAllAddress(Pagination entity)
        {
            try
            {
                var query = (from a in db.EmployeeAddress
                             join b in db.Cities on a.City equals b.CityId into lg1
                             from x in lg1.DefaultIfEmpty()
                             join c in db.States on a.State equals c.Id into lg2
                             from y in lg2.DefaultIfEmpty()
                             join d in db.Countries on a.Country equals d.CountryId into lg3
                             from z in lg3.DefaultIfEmpty()
                             select new
                             {
                                 a,
                                 a.EmpId,
                                 a.City,
                                 a.State,
                                 a.ContactPersonEmail,
                                 x.CityName,
                                 y.StateName,
                                 z.CountryName
                             })




                    .Where(a => entity.Id == 0 || a.EmpId == entity.Id).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(a => a.ContactPersonEmail.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Employee"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret SaveDocuments(FileUploadEntity File)
        {
            try
            {
                if (File?.DocumentName == null || string.IsNullOrEmpty(File.DocumentName.FileName))
                {
                    return new Ret { status = false, message = "No file selected." };
                }

                CommonLogic CL = new CommonLogic();
                string extension = Path.GetExtension(File.DocumentName.FileName);
                string uploadPath = Path.GetFullPath("Uploads/EmpDocuments/");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string UploadFileName = Path.GetFileNameWithoutExtension(File.DocumentName.FileName) + "_EmpDocuments" + File.Id;
                string NewFileNameWithFullPath = Path.Combine(uploadPath, UploadFileName + extension);

                bool uploadstatus = CL.upload(File.DocumentName, NewFileNameWithFullPath);

                if (!uploadstatus)
                {
                    return new Ret { status = false, message = "File upload failed." };
                }
                var newDocument = new DocumentsEntity
                {
                    EmpId = File.Id,
                    DocumentName = UploadFileName + extension,
                    FileDescription = File.FileDescription
                };

                db.EmployeeDocuments.Add(newDocument);
                db.SaveChanges();

                return new Ret { status = true, message = "File uploaded successfully.", data = newDocument.DocumentName };
            }
            catch (Exception ex)
            {
                Log.Information("Error " + DateTime.Now + " message " + ex.Message);
                return new Ret { status = false, message = "Failed to save document." };
            }
        }
        public Ret GetEmpDocuments(int Id)
        {
            try
            {
                var res = db.EmployeeDocuments.Where(x => x.EmpId == Id).ToList();
                return new Ret { status = true, message = FetchMessage(res, "EmployeeDocuments"), data = res };

            }
            catch (Exception ex)
            {
                Log.Information("GetEmpDocuments Error at " + DateTime.Now.ToString() + " message " + ex.Message);
                return new Ret { status = false, message = "Something went wrong. Please try again later!!" };
            }
        }

        public Ret DeleteEmpDoc(int Id)
        {
            try
            {
                var res = db.EmployeeDocuments.SingleOrDefault(x => x.Id == Id);

                if (res == null)
                {
                    return new Ret { status = false, message = "No data found." };
                }
                string filePath = Path.GetFullPath("Uploads/EmpDocuments/" + res.DocumentName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                db.EmployeeDocuments.Remove(res);
                db.SaveChanges();

                return new Ret { status = true, message = "File and record deleted successfully." };
            }
            catch (Exception ex)
            {
                Log.Information("DeleteEmpDoc at " + DateTime.Now.ToString() + " message " + ex.Message);
                return new Ret { status = false, message = "Something went wrong. Please try again later!!" };
            }
        }

        public Ret SaveProfilePhoto(ProfilePhoto profilephoto)
        {
            try
            {
                CommonLogic CL = new CommonLogic();
                string UploadFileName = "";
                string extension = "";

                if (profilephoto.EmpProfile != null)
                {
                    extension = Path.GetExtension(profilephoto.EmpProfile.FileName.ToString());
                    if (!Directory.Exists(Path.GetFullPath("Uploads/UserProfiles/")))
                    {
                        Directory.CreateDirectory(Path.GetFullPath("Uploads/UserProfiles/"));
                    }

                    UploadFileName = Path.GetFileNameWithoutExtension(profilephoto.EmpProfile.FileName.ToString()) + "_UserProfiles" + profilephoto.Id;
                    string NewFileNameWithFullPath = Path.GetFullPath("Uploads/UserProfiles/" + UploadFileName + extension).Replace("~\\", "");


                    bool uploadstatus = CL.upload(profilephoto.EmpProfile, NewFileNameWithFullPath);
                }

                var users = db.Employee.Where(x => x.EmpId == profilephoto.Id).FirstOrDefault();
                users.EmpProfile = UploadFileName + extension.ToString();
                db.SaveChanges();

                return new Ret { status = true, message = SaveSuccessMessage(1, "Profile Photo"), data = users.EmpProfile };

            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }
        public Ret UpdateMedicalCheckUpStatus(StatusUpdate entity)
        {
            try
            {
                var res = db.Employee.Where(x => x.EmpId == entity.Id).FirstOrDefault();
                res.IsMedicalCheckUpStatus = entity.Status;
                db.SaveChanges();
                return new Ret { status = true, message = UpdateStatusMessage("Employee MedicalCheckUpStatus") };

            }
            catch
            {
                return new Ret { status = false, message = NoDataMessage() };
            }
        }
        public Ret GetAllEmployeeLabel(int Id, JwtStatus jwtData)
        {
            try
            {
                var res = (from a in db.Users
                           join b in db.Employee on a.Id equals b.UserId
                           where a.HospitalId == jwtData.HospitalId && (Id == 0 || a.Id == Id)
                           select new { value = a.Id, label = a.UserFullName, a.TempEmpCode }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Employee"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
    }
}
