using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Serilog;

namespace Pinnacle.Models
{
    public class SpecimenModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();
        public Ret GetAllSpecimen(Pagination entity)
        {

            try
            {
                var query = db.Specimen.Where(a => entity.Id == 0 || a.Id == entity.Id).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.SpecimenName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Specimen"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }

        public Ret GetAllSpecimenLabel(int Id, JwtStatus jwtData)
        {
            try
            {
                var res = db.Specimen.Where(a => Id == 0 || a.Id == Id && a.HospitalId == jwtData.HospitalId && a.IsActive == "Yes").
                                      Select(a => new { value = a.Id, label = a.SpecimenCode + " - " + a.SpecimenName }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Specimen"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }


        public Ret SaveSpecimen(SpecimenEntity entity, JwtStatus jwtData)
        {
            try
            {
                entity.CreatedBy = jwtData.Id;
                entity.HospitalId = jwtData.HospitalId;
                string msg;

                if (entity.Id == 0)
                {

                    var lastSpecimen = db.Specimen.Where(x => x.SpecimenCode.StartsWith("SPC")).OrderByDescending(x => x.SpecimenCode)
                        .Select(x => x.SpecimenCode).FirstOrDefault();
                    string newSpecimenCode = "SPC1";

                    if (!string.IsNullOrEmpty(lastSpecimen) && lastSpecimen.Length > 2)
                    {
                        string numberPart = lastSpecimen.Substring(3);
                        if (int.TryParse(numberPart, out int lastNumber))
                        {
                            newSpecimenCode = $"SPC{(lastNumber + 1)}";
                        }
                    }

                    entity.SpecimenCode = newSpecimenCode;
                    db.Specimen.Add(entity);
                    msg = "Specimen saved successfully!";
                }
                else
                {
                    var existingSpecimen = db.Specimen.AsNoTracking().FirstOrDefault(x => x.Id == entity.Id);
                    var prevData = JsonConvert.SerializeObject(existingSpecimen);
                    if (existingSpecimen != null)
                    {
                        db.Specimen.Update(entity);
                        msg = "Specimen updated successfully!";

                    }
                    else
                    {
                        return new Ret { status = false, message = "Specimen not found." };
                    }
                }

                db.SaveChanges();
                return new Ret { status = true, message = msg };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Failed to save Specimen." };
            }
        }
    }
}
