using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;

namespace Pinnacle.IServices
{
    public interface IItemMasterService
    {
        Task<Ret> GetAllMedicines(int Id, JwtStatus jwtData);
        Task<Ret> GetAllMedicineList(Pagination entity, JwtStatus jwtData);
        Task<Ret> GetAllFavMedicines(Pagination entity, JwtStatus jwtData);
    }
}
