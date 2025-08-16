using Pinnacle.Entities;
using Pinnacle.Helpers;
using Serilog;
using Pinnacle.Helpers.JWT;
using System.Data;
using DevExpress.Pdf.Native.BouncyCastle.Security.Certificates;
using System.Linq;
using DevExpress.Pdf;




namespace Pinnacle.Models
{
    public class AuthenticationModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();
        public Ret Login(LoginEntity entity)
        {
            try
            {

                var user = db.Users
                            .FirstOrDefault(_user => _user.UserName == entity.UserName &&
                            _user.Password == CommonLogic.Encrypt(entity.Password));
                var profileName = db.UserProfile
               .Where(p => p.UserProfileId == user.UserProfileId)
               .Select(p => p.UserProfileName)
               .FirstOrDefault();
                if (user is not null && user.Status != "Active" && user.Status != "Pending")
                {
                    return new Ret { status = false, message = "Your account is inactive, Please contact Administrator." };
                }
                else if (user != null)
                {
                    user.LastLoginDate = DateTime.Now;
                    db.Users.Update(user);
                    db.SaveChanges();

                    var hospitalIds = user.HospitalId;
                    if (user.RoleId != 4)
                    {
                        var res = new
                        {
                            user.UserId,
                            user.Status,
                            user.UserName,
                            user.ContactNo,
                            user.Email,
                            user.HospitalId,
                            user.RoleId,
                            user.Id,
                            user.UserProfileId,
                            user.LastLoginDate,
                            user.UserFullName,
                            profileName,
                            user.SystemName,
                            Employee = db.Employee
                                .Where(e => e.TempEmpCode == user.UserName)
                                .Select(e => new { e.EmpId, Profile = e.EmpProfile })
                                .ToList(),

                            Doctor = (new[] { "MATERIAL REQUSET", "NS-USER", "NS-ADMIN" }.Contains(profileName))
                                      ? db.Doctors
                                        .Where(g => g.AssistantId == user.Id)
                                        .Select(g => new { g.DoctorId, g.DoctorName })
                                        .Cast<object>()
                                        .ToList()
                                        : new List<object>(),
                            //Doctor = Array.Empty<string>(),

                            Hospital = (from hospital in db.Hospital
                                            //join uhospital in db.UserHospital on hospital.HospitalId equals uhospital.HospitalId
                                            //where uhospital.UserId == user.Id
                                        select new { value = hospital.HospitalId, label = hospital.HospitalName, hospital.Logo, hospital.RegFee })
                                .ToList(),

                            Designation = (from desig in db.Designation
                                           join doc in db.Doctors on desig.DesignationId equals doc.DesignationId
                                           where doc.UserId == user.Id
                                           select new { desig.DesignationName }).FirstOrDefault(),
                            Patient = (from a in db.Patient
                                       where a.UserId == user.Id
                                       select new { a.PatientId }).FirstOrDefault()
                        };
                        return new Ret { status = true, message = "Logged in Successfully.", data = res };
                    }
                    else
                    {
                        var res = new
                        {
                            user.UserId,
                            user.Status,
                            user.UserName,
                            user.ContactNo,
                            user.Email,
                            user.HospitalId,
                            user.RoleId,
                            user.Id,
                            user.UserProfileId,
                            user.LastLoginDate,
                            profileName,
                            user.UserFullName,
                            //Employee = db.Employee
                            //    .Where(e => e.TempEmpCode == user.UserName)
                            //    .Select(e => new { e.EmpId, e.EmpProfile })
                            //    .ToList(),

                            Employee = (from a in db.Doctors
                                        join b in db.Designation on a.DesignationId equals b.DesignationId
                                        join d in db.Specializations on a.SpecializationId1 equals d.SpecializationId
                                        where a.UserId == user.Id
                                        select new
                                        {
                                            EmpId = a.DoctorId,
                                            b.DesignationName,
                                            d.SpecializationName,
                                            Profile = a.DoctorProfile,
                                            a.Qualification
                                        }).ToList(),

                            Hospital = (from uhospital in db.UserHospital
                                        join hospital in db.Hospital on uhospital.HospitalId equals hospital.HospitalId
                                        where uhospital.UserId == user.Id
                                        select new { value = hospital.HospitalId, label = hospital.HospitalName, hospital.Logo, hospital.RegFee })
                                .ToList(),

                            Designation = (from desig in db.Designation
                                           join doc in db.Doctors on desig.DesignationId equals doc.DesignationId
                                           where doc.UserId == user.Id
                                           select new { desig.DesignationName }).FirstOrDefault(),
                            Patient = (from a in db.Patient
                                       where a.UserId == user.Id
                                       select new { a.PatientId }).ToList()
                        };


                        return new Ret { status = true, message = "Logged in Successfully.", data = res };
                    }
                }
                else return new Ret { status = false, message = "Failed to login. You have entered incorrect credentials." };

            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = "Failed to login." };
            }
        }
        public Ret SendForgotPasswordMail(string Email, string ServerName)
        {
            try
            {
                var empData = db.Users.Where(x => x.Email == Email).FirstOrDefault();

                if (empData == null)
                {
                    Log.Information("SendForgotPasswordMail  user at " + DateTime.Now.ToString());
                    return new Ret { status = false, message = "User not found." };
                }
                else if (empData.Status == "InActive")
                {
                    Log.Information("SendForgotPasswordMail  user at " + DateTime.Now.ToString());
                    return new Ret { status = false, message = "InActive User Access Denied!" };
                }
                else
                {
                    if (Email != null)
                    {
                        string body = MailContentForCreateNewPw(empData.UserName, Email, Convert.ToDateTime(DateTime.Now.ToString()), empData.Id, ServerName);
                        string subject = "Create New Password: ";
                        Thread t = new Thread(() => sendemail(Email, subject, body));
                        t.Priority = ThreadPriority.Highest;
                        t.Start();
                    }

                    return new Ret { status = true, data = empData, message = "Mail sent successfully to registered email id." };
                }
            }
            catch (Exception ex)
            {
                Log.Information("GetMailById  user at " + DateTime.Now.ToString());
                return new Ret { status = false, message = "Failed to load data. Error: " + ex.InnerException };
            }
        }

        public Ret UpdatePassword(UpdatePasswordEntity entity, JwtStatus jwtData)
        {
            try
            {
                var res = db.Users.Where(x => x.Id == jwtData.Id).SingleOrDefault();
                if (res.Password == CommonLogic.Encrypt(entity.CurrentPassword))
                {
                    res.Password = CommonLogic.Encrypt(entity.NewPassword);
                    db.SaveChanges();
                    return new Ret { status = true, message = "Password updated successfully." };
                }
                else return new Ret { status = false, message = "Please enter correct currentpassword." };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }

        private string MailContentForCreateNewPw(string UserName, string Email, DateTime Starttime, int userId, string ServerName)
        {
            CommonLogic commonLogic = new CommonLogic();
            ConfigKeyInfo config = commonLogic.getConfigValues();
            string html = "";
            string nl = "<br />";
            string url = "";

            //if (ServerName == "schedyo.com")
            url = "http://" + ServerName + "/createpassword/" + CommonLogic.Base64Encode(userId.ToString());
            //else if (ServerName == "oppunity.com")
            //    url = "http://oppunity.com" + "/createpassword/" + commonLogic.Base64Encode(userId.ToString());
            //else if (ServerName == "qa.oppunity.com")
            //    url = "http://qa.oppunity.com" + "/createpassword/" + commonLogic.Base64Encode(userId.ToString());
            //else
            //    url = "http://localhost:3000" + "/createpassword/" + commonLogic.Base64Encode(userId.ToString());


            html += "Hi " + UserName + "," + nl;
            html += "Please click on the below link to create new password:" + nl;
            html += "Link: " + url + nl + nl;
            html += "Auto Generated Email" + nl;
            html += "Baitna";
            return html;
        }

        private void sendemail(string to, string subject, string body)
        {
            try
            {
                var SM = new SendMail();
                SM.SendEmailWithTemplate(to, "", subject, body);

            }
            catch (Exception ex)
            {
                Log.Information("sendmail appointment at " + DateTime.Now.ToString() + " message " + (ex.Message));
            }
        }

        public Ret SetPassword(PasswordClass PCEntity)
        {
            try
            {
                var User = db.Users.Where(x => x.Id == Convert.ToInt32(CommonLogic.Base64Decode(PCEntity.Token))).FirstOrDefault();
                if (PCEntity.Password != null)
                {
                    User.Password = CommonLogic.Encrypt(PCEntity.Password);
                }
                db.SaveChanges();
                return new Ret { status = true, message = SaveSuccessMessage(1, "Password") };
            }
            catch (Exception ex)
            {
                Log.Information("GetById  user at " + DateTime.Now.ToString());
                return new Ret { status = false, message = "Failed to load data. Error: " + ex.InnerException };
            }
        }
        public void UpdateRefreshToken(int Id, string RefreshToken, DateTime ExpiryDateTime)
        {
            try
            {
                var user = db.Users.Where(user => user.Id == Id).FirstOrDefault();
                if (user != null)
                {
                    user.RefreshToken = RefreshToken;
                    user.ExpiryDateTime = ExpiryDateTime;
                    db.Users.Update(user);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }
        public Ret RefreshToken(string Token)
        {
            try
            {
                try
                {

                    var user = db.Users
                                .FirstOrDefault(_user => _user.RefreshToken == Token);
                    if (user == null || user.ExpiryDateTime < DateTime.UtcNow)
                    {
                        return new Ret { status = false, message = "Token expired" };
                    }

                    var profileName = db.UserProfile
                   .Where(p => p.UserProfileId == user.UserProfileId)
                   .Select(p => p.UserProfileName)
                   .FirstOrDefault();
                    if (user is not null && user.Status != "Active" && user.Status != "Pending")
                    {
                        return new Ret { status = false, message = "Your account is inactive, Please contact Administrator." };
                    }
                    else if (user != null)
                    {
                        user.LastLoginDate = DateTime.Now;
                        db.Users.Update(user);
                        db.SaveChanges();

                        var hospitalIds = user.HospitalId;
                        if (user.RoleId != 4)
                        {
                            var res = new
                            {
                                user.UserId,
                                user.Status,
                                user.UserName,
                                user.ContactNo,
                                user.Email,
                                user.HospitalId,
                                user.RoleId,
                                user.Id,
                                user.UserProfileId,
                                user.LastLoginDate,
                                user.UserFullName,
                                profileName,
                                user.SystemName,
                                Employee = db.Employee
                                    .Where(e => e.TempEmpCode == user.UserName)
                                    .Select(e => new { e.EmpId, Profile = e.EmpProfile })
                                    .ToList(),

                                Doctor = (new[] { "MATERIAL REQUSET", "NS-USER", "NS-ADMIN" }.Contains(profileName))
                                          ? db.Doctors
                                            .Where(g => g.AssistantId == user.Id)
                                            .Select(g => new { g.DoctorId, g.DoctorName })
                                            .Cast<object>()
                                            .ToList()
                                            : new List<object>(),
                                //Doctor = Array.Empty<string>(),

                                Hospital = (from hospital in db.Hospital
                                                //join uhospital in db.UserHospital on hospital.HospitalId equals uhospital.HospitalId
                                                //where uhospital.UserId == user.Id
                                            select new { value = hospital.HospitalId, label = hospital.HospitalName, hospital.Logo, hospital.RegFee })
                                    .ToList(),

                                Designation = (from desig in db.Designation
                                               join doc in db.Doctors on desig.DesignationId equals doc.DesignationId
                                               where doc.UserId == user.Id
                                               select new { desig.DesignationName }).FirstOrDefault(),
                                Patient = (from a in db.Patient
                                           where a.UserId == user.Id
                                           select new { a.PatientId }).FirstOrDefault()
                            };
                            return new Ret { status = true, message = "Logged in Successfully.", data = res };
                        }
                        else
                        {
                            var res = new
                            {
                                user.UserId,
                                user.Status,
                                user.UserName,
                                user.ContactNo,
                                user.Email,
                                user.HospitalId,
                                user.RoleId,
                                user.Id,
                                user.UserProfileId,
                                user.LastLoginDate,
                                profileName,
                                user.UserFullName,
                                //Employee = db.Employee
                                //    .Where(e => e.TempEmpCode == user.UserName)
                                //    .Select(e => new { e.EmpId, e.EmpProfile })
                                //    .ToList(),

                                Employee = (from a in db.Doctors
                                            join b in db.Designation on a.DesignationId equals b.DesignationId
                                            join d in db.Specializations on a.SpecializationId1 equals d.SpecializationId
                                            where a.UserId == user.Id
                                            select new
                                            {
                                                EmpId = a.DoctorId,
                                                b.DesignationName,
                                                d.SpecializationName,
                                                Profile = a.DoctorProfile,
                                                a.Qualification
                                            }).ToList(),

                                Hospital = (from uhospital in db.UserHospital
                                            join hospital in db.Hospital on uhospital.HospitalId equals hospital.HospitalId
                                            where uhospital.UserId == user.Id
                                            select new { value = hospital.HospitalId, label = hospital.HospitalName, hospital.Logo, hospital.RegFee })
                                    .ToList(),

                                Designation = (from desig in db.Designation
                                               join doc in db.Doctors on desig.DesignationId equals doc.DesignationId
                                               where doc.UserId == user.Id
                                               select new { desig.DesignationName }).FirstOrDefault(),
                                Patient = (from a in db.Patient
                                           where a.UserId == user.Id
                                           select new { a.PatientId }).ToList()
                            };


                            return new Ret { status = true, message = "Logged in Successfully.", data = res };
                        }
                    }
                    else return new Ret { status = false, message = "Failed to login. You have entered incorrect credentials." };

                }
                catch (Exception ex)
                {
                    Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                    return new Ret { status = false, message = "Failed to login." };
                }
                return new Ret { };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Access Denied." };
            }
        }

    }
}
