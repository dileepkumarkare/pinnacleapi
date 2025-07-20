using Microsoft.EntityFrameworkCore;
using Pinnacle.Entities;
using Serilog;

namespace Pinnacle.Models
{
    public class PincodeModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();
        public Ret GetPincodeData(int Id)
        {
            try
            {
                var res = (from a in db.PincodeData.Where(pincode => pincode.Pincode == Id)
                          join b in db.District on a.DistrictId equals b.Id
                          join c in db.States on b.StateId equals c.Id
                          select new
                          {
                              a.Id,
                              a.Pincode,
                              b.DistrictName,
                              c.StateName
                          }).FirstOrDefault();
                var areas = db.PincodeData.Where(x => x.Pincode == Id).Select(x => new { value=x.Id,label = x.OfficeName }).AsNoTracking().ToList();
                return res is not null ? new Ret { status = true, message = "Pincode data loaded successfully!", data = new { picodeData=res, areas = areas } } : new Ret { status = false, message = "No data loaded!" };
            }
            catch (Exception ex)
            {
                Log.Information("GetPincodeData error at " + DateTime.Now + " Message " + ex.Message);
                return new Ret { status = false, message = "Something went wrong!" };

            }
        }
    }
}
