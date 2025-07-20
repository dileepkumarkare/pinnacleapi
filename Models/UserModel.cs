using Microsoft.EntityFrameworkCore;
using Pinnacle.Entities;
using Pinnacle.Helpers;
using Pinnacle.Helpers.JWT;
using Serilog;


namespace Pinnacle.Models
{
    public class UserModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();
        public Ret SaveUser(UserEntity entity, JwtStatus jwtData)
        {
            try
            {

                entity.AddedBy = jwtData.Id;

                if (entity.Id == 0)
                {
                    db.Users.Add(entity);
                    db.SaveChanges();
                    return new Ret { status = true, message = "User added successfully!" };
                }
                else
                {
                    var resData = db.Users.AsNoTracking().FirstOrDefault(h => h.Id == entity.Id);

                    if (resData != null)
                    {
                        db.Users.Update(entity);
                        db.SaveChanges();
                        return new Ret { status = true, message = "User updated successfully!" };
                    }
                    else
                    {
                        return new Ret { status = false, message = "User record not found for update." };
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error at " + DateTime.Now.ToString() + " - " + ex.Message);
                return new Ret { status = false, message = "Failed to save hospital record." };
            }
        }
        public Ret GetAllUsers(Pagination entity)
        {
            try
            {
                var query = db.Users.Where(a => entity.Id == 0 || a.RoleId == 2).AsNoTracking();

                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.UserName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "User"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret UserGetById(Pagination entity)
        {
            try
            {
                var query = db.Users.Where(a => a.Id == entity.Id).Select(a => new
                {
                    a.Id,
                    a.RoleId,
                    a.UserFullName,
                    a.UserName,
                    a.TitleId,
                    a.UserProfileId,
                    a.Gender,
                    a.HospitalId,
                    a.ContactNo,
                    a.DOB
                }).AsNoTracking();

                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.UserName.Contains(entity.SearchKey) || c.UserFullName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "User"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
    }
}
