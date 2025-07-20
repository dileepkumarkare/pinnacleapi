using Microsoft.EntityFrameworkCore;
using Pinnacle.Entities;
using Serilog;

namespace Pinnacle.Models
{
    public class CountryModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();

        public Ret GetCountryList(Pagination entity)
        {
            try
            {
                var query = db.Countries.Where(c => c.CountryId == entity.Id || entity.Id == 0).AsNoTracking();
                if (query is not null)
                {
                    if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(a => a.CountryName.Contains(entity.SearchKey) || a.CountryCode.Contains(entity.SearchKey));

                    var totalCount = query.Count();
                    var res = PaginatedValues(query, entity);
                    return new Ret { status = true, data = res, message = "Country list loaded successfully.", totalCount = totalCount };
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
