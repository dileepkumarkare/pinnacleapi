using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Serilog;

namespace Pinnacle.Models
{
    public class UserGroupModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();

        public Ret GetAllUserGroup(Pagination entity)
        {

            try
            {
                var query = db.UserGroup.Where(a => entity.Id == 0 || a.UserGroupId == entity.Id).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.UserGroupName.Contains(entity.SearchKey) || c.UserGroupCode.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity).OrderByDescending(grp => grp.UserGroupId);
                return new Ret { status = true, message = FetchMessage(res, "UserGroup"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }

        public Ret GetAllUserGroupLabel(int Id, JwtStatus jwtData)
        {
            try
            {
                var res = db.UserGroup.Where(a => Id == 0 || a.DepartmentId == Id && a.HospitalId == jwtData.HospitalId && a.IsActive == "Yes")
                                      .Select(a => new { value = a.UserGroupId, label = a.UserGroupName, a.UserGroupCode })
                                      .OrderBy(a => a.label)
                                      .AsNoTracking()
                                      .ToList();
                return new Ret { status = true, message = FetchMessage(res, "UserGroup"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }

        public Ret SaveUserGroup(UserGroupEntity entity, JwtStatus jwtData)
        {
            try
            {
                entity.CreatedBy = jwtData.Id;
                entity.HospitalId = jwtData.HospitalId;
                string msg;

                if (entity.UserGroupId == 0)
                {
                    db.UserGroup.Add(entity);
                    msg = "UserGroup saved successfully!";
                }
                else
                {
                    var existingUserGroup = db.UserGroup.AsNoTracking().FirstOrDefault(x => x.UserGroupId == entity.UserGroupId);
                    if (existingUserGroup != null)
                    {
                        db.UserGroup.Update(entity);
                        msg = "UserGroup updated successfully!";
                    }
                    else
                    {
                        return new Ret { status = false, message = "UserGroup not found." };
                    }
                }

                db.SaveChanges();
                return new Ret { status = true, message = msg };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Failed to save UserGroup." };
            }
        }
    }
}
