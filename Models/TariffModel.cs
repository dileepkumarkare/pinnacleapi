using Pinnacle.Entities;
using Microsoft.EntityFrameworkCore;
using Pinnacle.Helpers.JWT;
using Serilog;
namespace Pinnacle.Models
{
    public class TariffModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();
        public Ret SaveTariff(TariffEntity entity, JwtStatus jwtData)
        {
            try
            {
                entity.CreatedBy = jwtData.Id;
                entity.HospitalId = jwtData.HospitalId;
                string msg;

                if (entity.TariffId == 0)
                {

                    var lastTariff = db.Tariff.Where(x => x.TariffCode.StartsWith("TR")).OrderByDescending(x => x)
                        .Select(x => x.TariffCode).FirstOrDefault();
                    string newTariffCode = "TR01";

                    if (!string.IsNullOrEmpty(lastTariff) && lastTariff.Length > 2)
                    {
                        string numberPart = lastTariff.Substring(2);
                        if (int.TryParse(numberPart, out int lastNumber))
                        {
                            newTariffCode = $"TR{(lastNumber + 1):D2}";
                        }
                    }

                    entity.TariffCode = newTariffCode;
                    db.Tariff.Add(entity);
                    msg = "Tariff saved successfully!";
                }
                else
                {
                    var _existingTariff = db.Tariff.AsNoTracking().FirstOrDefault(x => x.TariffId == entity.TariffId);
                    if (_existingTariff != null)
                    {
                        db.Tariff.Update(entity);
                        msg = "Tariff updated successfully!";
                    }
                    else
                    {
                        return new Ret { status = false, message = "Tariff not found." };
                    }
                }

                db.SaveChanges();
                return new Ret { status = true, message = msg };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Failed to save Tariff." };
            }
        }
        public Ret GetAllTariff(Pagination entity)
        {

            try
            {
                var query = db.Tariff.Where(a => entity.Id == 0 || a.TariffId == entity.Id).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.TariffName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Tariff"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }


        public Ret GetAlltariffLabel(int Id)
        {
            try
            {
                var res = db.Tariff.Where(a => Id == 0 || a.TariffId == Id).Select(a => new { value = a.TariffId, label = a.TariffCode + " - " + a.TariffName }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Tariff"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }



    }
}
