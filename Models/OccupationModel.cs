using Pinnacle.Entities;
using Microsoft.EntityFrameworkCore;
using Pinnacle.Helpers.JWT;
using Serilog;
namespace Pinnacle.Models
{
    public class OccupationModel : MasterModel
    {

        PinnacleDbContext db = new PinnacleDbContext();
        public Ret SaveOccupation(OccupationEntity entity, JwtStatus jwtData)
        {
            try
            {
                entity.AddedBy = jwtData.Id;
                entity.HospitalId = jwtData.HospitalId;
                string msg;

                if (entity.OccupationId == 0)
                {

                    var lastOccupation = db.Occupation.Where(x => x.OccupationCode.StartsWith("OCC")).OrderByDescending(x => x.OccupationCode)
                        .Select(x => x.OccupationCode).FirstOrDefault();
                    string newOccupationCode = "OCC01";

                    if (!string.IsNullOrEmpty(lastOccupation) && lastOccupation.Length > 2)
                    {
                        string numberPart = lastOccupation.Substring(2);
                        if (int.TryParse(numberPart, out int lastNumber))
                        {
                            newOccupationCode = $"OCC{(lastNumber + 1):D2}";
                        }
                    }

                    entity.OccupationCode = newOccupationCode;
                    db.Occupation.Add(entity);
                    msg = "Occupation saved successfully!";
                }
                else
                {
                    var existingOccupation = db.Occupation.AsNoTracking().FirstOrDefault(x => x.OccupationId == entity.OccupationId);
                    if (existingOccupation != null)
                    {
                        db.Occupation.Update(entity);
                        msg = "Occupation updated successfully!";
                    }
                    else
                    {
                        return new Ret { status = false, message = "Occupation not found." };
                    }
                }

                db.SaveChanges();
                return new Ret { status = true, message = msg };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Failed to save Occupation." };
            }
        }



        public Ret GetAllOccupation(Pagination entity)
        {

            try
            {
                var query = db.Occupation.Where(a => entity.Id == 0 || a.OccupationId == entity.Id).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.OccupationName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Occupation"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }

        public Ret GetAllOccupationLabel(int Id)
        {
            try
            {
                var res = db.Occupation.Where(a => Id == 0 || a.OccupationId == Id).Select(a => new { value = a.OccupationId, label = a.OccupationName + " - " + a.OccupationCode }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Occupation"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }


    }
}
