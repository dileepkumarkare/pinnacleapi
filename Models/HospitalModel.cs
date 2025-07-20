using Microsoft.EntityFrameworkCore;
using Pinnacle.Entities;
using Pinnacle.Helpers;
using Pinnacle.Helpers.JWT;
using Serilog;
using System.Net;
using System.Text.RegularExpressions;


namespace Pinnacle.Models
{
    public class HospitalModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();
        public Ret SaveHospital(HospitalEntity entity, JwtStatus jwtData)
        {
            try
            {
                entity.AddedBy = jwtData.Id;

                if (entity.HospitalId == 0)
                {
                    db.Hospital.Add(entity);
                    db.SaveChanges();
                    return new Ret { status = true, message = "Hospital added successfully!" };
                }
                else
                {
                    var existingEntity = db.Hospital.AsNoTracking().FirstOrDefault(h => h.HospitalId == entity.HospitalId);
                    if (existingEntity != null)
                    {

                        existingEntity.HospitalName = string.IsNullOrWhiteSpace(entity.HospitalName) ? existingEntity.HospitalName : entity.HospitalName;
                        existingEntity.Branch = string.IsNullOrWhiteSpace(entity.Branch) ? existingEntity.Branch : entity.Branch;
                        existingEntity.Status = string.IsNullOrWhiteSpace(entity.Status) ? existingEntity.Status : entity.Status;
                        existingEntity.RegFee = entity.RegFee is null ? existingEntity.RegFee : entity.RegFee;
                        existingEntity.Visits = entity.Visits is null ? existingEntity.Visits : entity.Visits;
                        existingEntity.Days = entity.Days is null ? existingEntity.Days : entity.Days;
                        existingEntity.UpdatedDate = DateTime.Now;
                        existingEntity.Address = entity.Address is null ? entity.Address : entity.Address;
                        existingEntity.Contact = entity.Contact is null ? existingEntity.Contact : entity.Contact;
                        db.Hospital.Update(existingEntity);
                        db.SaveChanges();
                        return new Ret { status = true, message = "Hospital updated successfully!" };
                    }
                    else
                    {
                        return new Ret { status = false, message = "Hospital record not found for update." };
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error at " + DateTime.Now.ToString() + " - " + ex.Message);
                return new Ret { status = false, message = "Failed to save hospital record." };
            }
        }
        public Ret GetAllHospitalsLabel()
        {
            try
            {
                var res = db.Hospital.Select(a => new { value = a.HospitalId, label = a.HospitalName }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Hospital"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret GetAllHospitals(Pagination entity)
        {
            try
            {
                var query = db.Hospital.Where(a => entity.Id == 0 || a.HospitalId == entity.Id).OrderByDescending(a=>a).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.HospitalName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Hospital"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret SaveHospitalLogo(HospitalProfile profilephoto)
        {
            try
            {
                CommonLogic CL = new CommonLogic();
                string UploadFileName = "";
                string extension = "";

                if (profilephoto.Logo != null)
                {
                    extension = Path.GetExtension(profilephoto.Logo.FileName.ToString());
                    if (!Directory.Exists(Path.GetFullPath("Uploads/HospitalProfile/")))
                    {
                        Directory.CreateDirectory(Path.GetFullPath("Uploads/HospitalProfile/"));
                    }

                    UploadFileName = Path.GetFileNameWithoutExtension(profilephoto.Logo.FileName.ToString()) + "_HospitalLogo" + profilephoto.Id;
                    string NewFileNameWithFullPath = Path.GetFullPath("Uploads/HospitalProfile/" + UploadFileName + extension).Replace("~\\", "");


                    bool uploadstatus = CL.upload(profilephoto.Logo, NewFileNameWithFullPath);
                }

                var users = db.Hospital.Where(x => x.HospitalId == profilephoto.Id).FirstOrDefault();
                users.Logo = UploadFileName + extension.ToString();
                db.SaveChanges();

                return new Ret { status = true, message = SaveSuccessMessage(1, "Profile Photo"), data = users.Logo };

            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }

    }
}
