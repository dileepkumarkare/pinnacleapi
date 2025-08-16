using Microsoft.EntityFrameworkCore.Metadata;
using Pinnacle.Entities;
using Pinnacle.Helpers;
using Pinnacle.Helpers.JWT;
using Pinnacle.IServices;
using System.Data;
using System.Data.SqlClient;

namespace Pinnacle.Services
{
    public class ItemMasterService : IItemMasterService
    {
        PinnacleDbContext db = new PinnacleDbContext();
        public async Task<Ret> GetAllMedicines(int Id, JwtStatus jwtData)
        {
            DBHelper dBHelper = new DBHelper(db.GetConnectionString());
            SqlParameter[] objParams = new SqlParameter[1];
            objParams[0] = new SqlParameter("@HospitalId", jwtData.HospitalId);
            DataSet ds = dBHelper.ExecuteDataSetSP("PKG_GET_ITEM_LIST", objParams);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return new Ret
                {
                    status = true,
                    data = CommonLogic.GetJsonObject(ds),
                    message = "Medicine list loaded successfully."
                };
            }
            else
            {
                return new Ret { status = true, message = "No data found." };
            }

        }
        public async Task<Ret> GetAllMedicineList(Pagination entity, JwtStatus jwtData)
        {
            DBHelper dBHelper = new DBHelper(db.GetConnectionString());
            SqlParameter[] objParams = new SqlParameter[6];
            objParams[0] = new SqlParameter("@HospitalId", jwtData.HospitalId);
            objParams[1] = new SqlParameter("@SearchKey", entity.SearchKey);
            objParams[2] = new SqlParameter("@PageNumber", entity.PageNumber);
            objParams[3] = new SqlParameter("@PageSize", entity.PageSize);
            objParams[4] = new SqlParameter("@AllKeys", entity.AllKeys);
            objParams[5] = new SqlParameter("@Id", jwtData.Id);
            DataSet ds = dBHelper.ExecuteDataSetSP("PKG_GET_ALL_MEDICINES", objParams);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return new Ret
                {
                    status = true,
                    data = CommonLogic.GetJsonObject(ds),
                    message = "Medicine list loaded successfully.",
                    totalCount = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalCount"].ToString())
                };
            }
            else
            {
                return new Ret { status = true, message = "No data found." };
            }
        }
        public async Task<Ret> GetAllFavMedicines(Pagination entity, JwtStatus jwtData)
        {
            return new Ret();
        }
    }
}
