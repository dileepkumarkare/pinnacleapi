using Microsoft.EntityFrameworkCore;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Serilog;
namespace Pinnacle.Models   
{
    public class SpecializationModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();


        public Ret SaveSpecialization(SpecializationEntity entity, JwtStatus jwtData)
        {
            try
            {
                entity.CreatedBy = jwtData.Id;
                entity.HospitalId = jwtData.HospitalId;
                string msg;

                if (entity.SpecializationId == 0)
                {
                    var lastSpecializationCode = db.Specializations.Where(x => x.SpecializationCode.StartsWith("SPE")).OrderByDescending(x => x.SpecializationCode).Select(x => x.SpecializationCode).FirstOrDefault();
                    string newSpecializationCode = "SPE001";

                    if (!string.IsNullOrEmpty(lastSpecializationCode) && lastSpecializationCode.Length > 3)
                    {
                        string numberPart = lastSpecializationCode.Substring(3); 
                        if (int.TryParse(numberPart, out int lastNumber))
                        {
                            newSpecializationCode = $"SPE{(lastNumber + 1):D3}"; 
                        }
                    }

                    entity.SpecializationCode = newSpecializationCode;
                    db.Specializations.Add(entity);
                    msg = "Specialization saved successfully!";
                }
                else
                {
                    var existingSpecialization = db.Specializations.AsNoTracking().FirstOrDefault(x => x.SpecializationId == entity.SpecializationId);
                    if (existingSpecialization != null)
                    {
                        db.Specializations.Update(entity);
                        msg = "Specialization updated successfully!";
                    }
                    else
                    {
                        return new Ret { status = false, message = "Specialization not found." };
                    }
                }

                db.SaveChanges();
                return new Ret { status = true, message = msg };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Failed to save Specialization." };
            }
        }

        public Ret GetAllSpecializations(Pagination entity)
        {

            try
            {
                var query = db.Specializations.Where(a => entity.Id == 0 || a.SpecializationId == entity.Id).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.SpecializationName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Specializations"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }

        public Ret GetAllSpecializationsLabel(int Id)
        {
            try
            {
                var res = db.Specializations.Where(a => Id == 0 || a.SpecializationId == Id).Select(a => new { value = a.SpecializationId, label = a.SpecializationName }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Specializations"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
    }
}
