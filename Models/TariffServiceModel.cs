using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Serilog;

namespace Pinnacle.Models
{
    public class TariffServiceModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();

        public Ret GetAllTariffServices(Pagination entity, JwtStatus jwtData)
        {

            try
            {
                var query = (from a in db.TariffServiceMapping.Where(a => (entity.Id == 0 || a.Id == entity.Id))
                             join b in db.Tariff on a.TariffId equals b.TariffId
                             join c in db.Services on a.ServiceId equals c.Id
                             select new
                             {
                                 a.Id,
                                 a.TariffId,
                                 a.ServiceId,
                                 a.TariffServiceName,
                                 a.TariffServiceCode,
                                 a.Charge,
                                 a.CreatedBy,
                                 a.CreatedDate,
                                 a.UpdatedBy,
                                 a.UpdatedDate,
                                 tariffName = b.TariffCode + " - " + b.TariffName,
                                 ServiceName = c.ServiceCode + " - " + c.ServiceName
                             }
                             ).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.tariffName.Contains(entity.SearchKey) || c.ServiceName.Contains(entity.SearchKey) ||
                 c.TariffServiceCode.Contains(entity.SearchKey) || c.TariffServiceName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Tariff service mapping"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }

        public Ret GetAllTariffServicesLabel(int Id, JwtStatus jwtData)
        {
            try
            {
                var res = db.TariffServiceMapping.Where(a => (Id == 0 || a.Id == Id)).Select(a => new { value = a.Id, label = a.TariffServiceCode + " - " + a.TariffServiceName, a.Charge }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Tariff service mapping"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret SaveTariffService(TariffServiceMapping entity, JwtStatus jwtData)
        {
            try
            {
                entity.CreatedBy = jwtData.Id;
                entity.HospitalId = jwtData.HospitalId;
                string msg;

                if (entity.Id == 0)
                {


                    db.TariffServiceMapping.Add(entity);
                    msg = "Tariff service mapping saved successfully!";
                }
                else
                {
                    var _existingTariffService = db.TariffServiceMapping.AsNoTracking().FirstOrDefault(x => x.Id == entity.Id);
                    if (_existingTariffService != null)
                    {
                        _existingTariffService.TariffServiceName = entity.TariffServiceName;
                        _existingTariffService.TariffServiceCode = entity.TariffServiceCode;
                        // _existingTariffService.ServiceGroupId = entity.ServiceGroupId;
                        _existingTariffService.Charge = entity.Charge;
                        _existingTariffService.UpdatedBy = jwtData.Id;
                        _existingTariffService.UpdatedDate = DateTime.Now;
                        db.TariffServiceMapping.Update(_existingTariffService);
                        msg = "Tariff service mapping updated successfully!";
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
                return new Ret { status = false, message = "Failed to map tariff and service." };
            }
        }
    }
}
