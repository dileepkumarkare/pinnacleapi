using Microsoft.EntityFrameworkCore;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Serilog;

namespace Pinnacle.Models
{
    public class TariffAppModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();
        private readonly JwtStatus _jwtData;
        public TariffAppModel(JwtStatus jwtData)
        {
            _jwtData = jwtData;
        }

        public Ret GetAllTariffApps(Pagination entity,JwtStatus jwtData)
        {

            try
            {
                var query = db.TariffApp.Where(a => entity.Id == 0 || a.Id == entity.Id).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.TariffAppName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "TariffApp"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }

        public Ret GetAllTariffAppsLabel(int Id, JwtStatus jwtData)
        {
            try
            {
                var res = db.TariffApp.Where(a => (Id == 0 || a.Id == Id) && a.HospitalId == jwtData.HospitalId && a.IsActive == "Yes").
                                       Select(a => new { value = a.Id, label = a.TariffAppName + " - " + a.TariffAppCode }).
                                       AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "TariffApp"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }


        public Ret SaveTariffApp(TariffAppEntity entity, JwtStatus jwtData)
        {
            try
            {
                entity.AddedBy = jwtData.Id;
                entity.HospitalId = jwtData.HospitalId;
                string msg;

                if (entity.Id == 0)
                {

                    var lastTariffApp = db.TariffApp.Where(x => x.TariffAppCode.StartsWith("DE")).OrderByDescending(x => x.TariffAppCode)
                        .Select(x => x.TariffAppCode).FirstOrDefault();
                    string newTariffAppCode = "DE001";

                    if (!string.IsNullOrEmpty(lastTariffApp) && lastTariffApp.Length > 2)
                    {
                        string numberPart = lastTariffApp.Substring(2);
                        if (int.TryParse(numberPart, out int lastNumber))
                        {
                            newTariffAppCode = $"DE{(lastNumber + 1):D3}";
                        }
                    }

                    entity.TariffAppCode = newTariffAppCode;
                    db.TariffApp.Add(entity);
                    msg = "TariffApp saved successfully!";
                }
                else
                {
                    var existingTariffApp = db.TariffApp.AsNoTracking().FirstOrDefault(x => x.Id == entity.Id);
                    if (existingTariffApp != null)
                    {
                        db.TariffApp.Update(entity);
                        msg = "TariffApp updated successfully!";
                    }
                    else
                    {
                        return new Ret { status = false, message = "TariffApp not found." };
                    }
                }

                db.SaveChanges();
                return new Ret { status = true, message = msg };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Failed to save TariffApp." };
            }
        }
    }
}
