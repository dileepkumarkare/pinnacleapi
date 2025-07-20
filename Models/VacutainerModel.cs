using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Serilog;
using System.Buffers;

namespace Pinnacle.Models
{
    public class VacutainerModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();

        public Ret GetAllVacutainer(Pagination entity)
        {

            try
            {
                var query = db.Vacutainer.Where(a => entity.Id == 0 || a.Id == entity.Id).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.VacutainerName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Vacutainer"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }

        public Ret GetAllVacutainerLabel(int Id, JwtStatus jwtData)
        {
            try
            {
                var res = db.Vacutainer.Where(a => Id == 0 || a.Id == Id && a.HospitalId == jwtData.HospitalId && a.IsActive == "Yes").
                                      Select(a => new { value = a.Id, label = a.VacutainerCode + " - " + a.VacutainerName }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Vacutainer"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }


        public Ret SaveVacutainer(VacutainerEntity entity, JwtStatus jwtData)
        {
            try
            {
                entity.CreatedBy = jwtData.Id;
                entity.HospitalId = jwtData.HospitalId;
                string msg;

                if (entity.Id == 0)
                {

                    var lastVacutainer = db.Vacutainer.Where(x => x.VacutainerCode.StartsWith("VCT")).OrderByDescending(x => x.VacutainerCode)
                        .Select(x => x.VacutainerCode).FirstOrDefault();
                    string newVacutainerCode = "VCT0001";

                    if (!string.IsNullOrEmpty(lastVacutainer) && lastVacutainer.Length > 2)
                    {
                        string numberPart = lastVacutainer.Substring(3);
                        if (int.TryParse(numberPart, out int lastNumber))
                        {
                            newVacutainerCode = $"VCT{(lastNumber + 1):D4}";
                        }
                    }

                    entity.VacutainerCode = newVacutainerCode;
                    db.Vacutainer.Add(entity);
                    msg = "Vacutainer saved successfully!";
                }
                else
                {
                    var existingVacutainer = db.Vacutainer.AsNoTracking().FirstOrDefault(x => x.Id == entity.Id);
                    var prevData = JsonConvert.SerializeObject(existingVacutainer);
                    if (existingVacutainer != null)
                    {
                        db.Vacutainer.Update(entity);
                        msg = "Vacutainer updated successfully!";

                    }
                    else
                    {
                        return new Ret { status = false, message = "Vacutainer not found." };
                    }
                }

                db.SaveChanges();
                return new Ret { status = true, message = msg };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Failed to save Vacutainer." };
            }
        }
    }
}
