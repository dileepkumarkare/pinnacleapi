using Pinnacle.Entities;
using Serilog;
using Pinnacle.Helpers.JWT;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace Pinnacle.Models
{
    public class AddressModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();
        public Ret SaveCountry(CountryEntity entity, JwtStatus jwData)
        {
            try
            {
                entity.CreatedBy = jwData.Id;
                var res = entity.CountryId == 0 ? db.Countries.Add(entity) : db.Countries.Update(entity);
                db.SaveChanges();
                return new Ret { status = true, message = AddSaveMessage(entity.CountryId, "Country") };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }
        public Ret SaveState(StateEntity entity, JwtStatus jwData)
        {
            try
            {
                entity.AddedBy = jwData.Id;
                var res = entity.Id == 0 ? db.States.Add(entity) : db.States.Update(entity);
                db.SaveChanges();
                return new Ret { status = true, message = AddSaveMessage(entity.Id, "State") };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }
        public Ret SaveCity(CityEntity entity, JwtStatus jwData)
        {
            try
            {
                entity.AddedBy = jwData.Id;
                var res = entity.CityId == 0 ? db.Cities.Add(entity) : db.Cities.Update(entity);
                db.SaveChanges();
                return new Ret { status = true, message = AddSaveMessage(entity.CityId, "City") };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }
        public Ret GetAllCountry(Pagination entity)
        {
            try
            {
                var query = db.Countries.OrderBy(c => c.Priority).ThenBy(c=>c.CountryName).AsQueryable().AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.CountryName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Country"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret GetAllStates(Pagination entity)
        {
            try
            {
                var query = db.States.Where(a => entity.Id == 0 || a.CountryId == entity.Id).AsNoTracking();

                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.StateName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "State"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret GetAllCity(Pagination entity)
        {
            try
            {
                var query = db.Cities.Where(a => entity.Id == 0 || a.StateId == entity.Id).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.CityName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "City"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret GetAllCountryLabel()
        {
            try
            {
                var res = db.Countries.Select(a => new { value = a.CountryId, label = a.CountryName }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Country"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret GetAllStateLabel(int Id)
        {
            try
            {
                var res = db.States.Where(a => Id == 0 || a.CountryId == Id).Select(a => new { value = a.Id, label = a.StateName }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "City"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret GetAllCityLabel(int Id)
        {
            try
            {
                var res = db.Cities.Where(a => Id == 0 || a.StateId == Id).Select(a => new { value = a.CityId, label = a.CityName }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "City"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret GetCountryLabel()
        {
            try
            {
                var countryList = db.Countries.Where(c => c.IsActive == "Yes").Select(c => new { value = c.CountryId, c.CountryCode, label = c.CountryName, c.Priority }).OrderBy(c => c.Priority).AsNoTracking().ToList();
                if (countryList is not null)
                {
                    return new Ret { status = true, data = countryList, message = "Country list loaded successfully." };
                }
                else
                {
                    return new Ret { status = false, message = "Failed to load the country list" };
                }
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong" };
                Log.Information("Country model => Get Country List method error at " + DateTime.Now.ToString() + " message " + ex.Message);
            }
        }
    }
}
