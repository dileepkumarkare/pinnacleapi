using Microsoft.EntityFrameworkCore;
using DevExpress.Xpo;
using Pinnacle.Entities;
using Pinnacle.Helpers;
using Pinnacle.IServices;
using Serilog;
using IAuthenticationService = Pinnacle.IServices.IAuthenticationService;
using DevExpress.Xpo.DB;

namespace Pinnacle.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly PinnacleDbContext _db = new PinnacleDbContext();


        public async Task<Ret> Login(LoginEntity entity)
        {
            try
            {
                var user = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_db.Users
                            .Where(_user => _user.UserName == entity.UserName &&
                            _user.Password == CommonLogic.Encrypt(entity.Password)
                            ));
                var profileName = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_db.UserProfile
.Where(p => p.UserProfileId == user.UserProfileId)
.Select(p => p.UserProfileName));
                if (user is not null && user.Status != "Active" && user.Status != "Pending")
                {
                    return new Ret { status = false, message = "Your account is inactive, Please contact Administrator." };
                }
                else if (user != null)
                {
                    user.RefreshToken = Pinnacle.Helpers.JWT.JwtHelpers.RefreshToken();
                    user.ExpiryDateTime = DateTime.UtcNow;
                    user.LastLoginDate = DateTime.Now;
                    _db.Users.Update(user);
                    _db.SaveChanges();

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
                            Employee = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(_db.Employee
                                .Where(e => e.TempEmpCode == user.UserName)
                                .Select(e => new { e.EmpId, Profile = e.EmpProfile })
                                ),

                            Doctor = (new[] { "MATERIAL REQUSET", "NS-USER", "NS-ADMIN" }.Contains(profileName))
                                      ? await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(_db.Doctors
                                        .Where(g => g.AssistantId == user.Id)
                                        .Select(g => new { g.DoctorId, g.DoctorName })
                                        .Cast<object>()
                                        )
                                        : new List<object>(),
                            //Doctor = Array.Empty<string>(),

                            Hospital = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(from hospital in _db.Hospital
                                                                                                                              //join uhospital in db.UserHospital on hospital.HospitalId equals uhospital.HospitalId
                                                                                                                              //where uhospital.UserId == user.Id
                                                                                                                          select new { value = hospital.HospitalId, label = hospital.HospitalName, hospital.Logo, hospital.RegFee })
                                ,

                            Designation = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(from desig in _db.Designation
                                                                                                                                     join doc in _db.Doctors on desig.DesignationId equals doc.DesignationId
                                                                                                                                     where doc.UserId == user.Id
                                                                                                                                     select new { desig.DesignationName }),
                            Patient = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(from a in _db.Patient
                                                                                                                                 where a.UserId == user.Id
                                                                                                                                 select new { a.PatientId })
                        };
                        return new Ret { status = true, message = "Logged in Successfully.", data = res, RefreshToken = user.RefreshToken };
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

                            Employee = (from a in _db.Doctors
                                        join b in _db.Designation on a.DesignationId equals b.DesignationId
                                        join d in _db.Specializations on a.SpecializationId1 equals d.SpecializationId
                                        where a.UserId == user.Id
                                        select new
                                        {
                                            EmpId = a.DoctorId,
                                            b.DesignationName,
                                            d.SpecializationName,
                                            Profile = a.DoctorProfile,
                                            a.Qualification
                                        }).ToList(),

                            Hospital = (from uhospital in _db.UserHospital
                                        join hospital in _db.Hospital on uhospital.HospitalId equals hospital.HospitalId
                                        where uhospital.UserId == user.Id
                                        select new { value = hospital.HospitalId, label = hospital.HospitalName, hospital.Logo, hospital.RegFee })
                                .ToList(),

                            Designation = (from desig in _db.Designation
                                           join doc in _db.Doctors on desig.DesignationId equals doc.DesignationId
                                           where doc.UserId == user.Id
                                           select new { desig.DesignationName }).FirstOrDefault(),
                            Patient = (from a in _db.Patient
                                       where a.UserId == user.Id
                                       select new { a.PatientId }).ToList()
                        };


                        return new Ret { status = true, message = "Logged in Successfully.", data = res, RefreshToken = user.RefreshToken };
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

    }
}
