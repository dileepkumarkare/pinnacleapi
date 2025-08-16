using Microsoft.EntityFrameworkCore;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Serilog;

namespace Pinnacle.Models
{
    public class BillingHeaderModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();

        public Ret GetAllBillingHeader(Pagination entity, JwtStatus jwtData)
        {

            try
            {
                var query = db.BillingHeader.Where(a => (entity.Id == 0 || a.BillingHeaderId == entity.Id) && a.HospitalId == jwtData.HospitalId).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.BillingHeaderName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Billing header"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }

        public Ret GetAllBillingHeaderLabel(int Id, JwtStatus jwtData)
        {
            try
            {
                var res = db.BillingHeader.Where(a => (Id == 0 || a.BillingHeaderId == Id)).Select(a => new { value = a.BillingHeaderId, label = a.BillingHeaderName }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Billing header"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }


        public Ret SaveBillingHeader(BillingHeaderEntity entity, JwtStatus jwtData)
        {
            try
            {
                entity.CreatedBy = jwtData.Id;
                entity.HospitalId = jwtData.HospitalId;
                string msg;

                if (entity.BillingHeaderId == 0)
                {
                    db.BillingHeader.Add(entity);
                    msg = "Billing header saved successfully!";
                }
                else
                {
                    var existingService = db.BillingHeader.AsNoTracking().FirstOrDefault(x => x.BillingHeaderId == entity.BillingHeaderId);
                    if (existingService != null)
                    {
                        existingService.BillingHeaderName = entity.BillingHeaderName;
                        existingService.ServiceType = entity.ServiceType;
                        existingService.UpdatedBy = jwtData.Id;
                        existingService.UpdatedDate = DateTime.Now;
                        db.BillingHeader.Update(existingService);
                        msg = "Billing header updated successfully!";
                    }
                    else
                    {
                        return new Ret { status = false, message = "Service not found." };
                    }
                }

                db.SaveChanges();
                return new Ret { status = true, message = msg };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Failed to save Service." };
            }
        }

    }
}
