using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pinnacle.Entities;
using Pinnacle.Helpers;
using Pinnacle.Helpers.JWT;
using Serilog;
using System.Data;

namespace Pinnacle.Models
{
    public class ItemModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();
        // OracleHelper oracleHelper = new OracleHelper();
        public Ret GetAllMedicines(int Id, JwtStatus jwtData)
        {
            try
            {
                //DataSet dsInventory = oracleHelper.GetLiveInventory();
                //string json = JsonConvert.SerializeObject(dsInventory.Tables[0]);
                //List<LiveInventory> inventoryList = JsonConvert.DeserializeObject<List<LiveInventory>>(json);

                //var itemData = (from a in db.ItemMaster
                //                join b in db.ItemGenericNames on a.GenericCodeId equals b.Id
                //                join c in db.ItemFormCodes on a.FormCodeId equals c.Id
                //                where a.HospitalId == jwtData.HospitalId && a.IsActive == "Y"
                //                select new
                //                {
                //                    a.ItemId,
                //                    a.ItemDesc,
                //                    a.ItemCode,
                //                    b.GenericDesc,
                //                    c.FormCodeDesc
                //                }).AsNoTracking().ToList(); // Execute SQL 
                //var res = (from a in itemData
                //           join inv in inventoryList on a.ItemCode equals inv.ItemCd into _Inventory
                //           from _inv in _Inventory.DefaultIfEmpty()
                //           select new
                //           {
                //               value = a.ItemId,
                //               label = a.ItemDesc + " [" + a.GenericDesc + " ]",
                //               a.ItemCode,
                //               a.FormCodeDesc,
                //               OnHandQty = _inv?.OnHandQty ?? 0.00
                //           }).ToList();
                var res = (from a in db.ItemMaster
                           join b in db.ItemGenericNames on a.GenericCodeId equals b.Id
                           join c in db.ItemFormCodes on a.FormCodeId equals c.Id
                           join d in db.MedicineOnHandStock on a.ItemCode equals d.ITEMCD into _Inventory
                           from e in _Inventory.DefaultIfEmpty()
                               //join inv in inventoryList.AsEnumerable() on a.ItemCode equals inv.ItemCd into _Inventory
                               //from _inv in _Inventory.DefaultIfEmpty()
                           where a.HospitalId == jwtData.HospitalId && a.IsActive == "Y"
                           select new { value = a.ItemId, label = a.ItemDesc + " [" + b.GenericDesc + " ]" + "[" + Convert.ToString(e.ONHANDQTY ?? "0") + "]", a.ItemCode, c.FormCodeDesc }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Item"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }

        public Ret GetAllMedicineList(Pagination entity, JwtStatus jwtData)
        {
            try
            {
                var query = from item in db.ItemMaster
                            join docFavService in db.DoctorFavMedicineServices.Where(s => s.Type == 1) on item.ItemId equals docFavService.ServiceId into docFavServices
                            from _docService in docFavServices.DefaultIfEmpty()
                            join doc in db.Doctors on _docService.DoctorId equals doc.DoctorId into _docServices
                            from ds in _docServices.Where(d => d.UserId == jwtData.Id).DefaultIfEmpty()
                            join d in db.MedicineOnHandStock on item.ItemCode equals d.ITEMCD into _Inventory
                            from e in _Inventory.DefaultIfEmpty()
                            join b in db.ItemGenericNames on item.GenericCodeId equals b.Id
                            join c in db.ItemFormCodes on item.FormCodeId equals c.Id
                            where item.ItemType == "M" && item.HospitalId == jwtData.HospitalId && item.IsActive == "Y"

                            select new
                            {
                                value = item.ItemId,
                                label = item.ItemDesc + " [" + b.GenericDesc + " ]" + "[" + Convert.ToString(e.ONHANDQTY ?? "0") + "]",
                                IsFavorite = string.IsNullOrEmpty(_docService.IsFavorite) ? "No" : _docService.IsFavorite,
                                item.ItemCode,
                                c.FormCodeDesc
                            };
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.label.Contains(entity.SearchKey) || c.FormCodeDesc.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(query, "Service"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }

        public Ret GetAllFavMedicines(Pagination entity, JwtStatus jwtData)
        {
            try
            {

                var query = from item in db.ItemMaster
                            join docFavService in db.DoctorFavMedicineServices.Where(s => s.Type == 1) on item.ItemId equals docFavService.ServiceId into docFavServices
                            from _docService in docFavServices.DefaultIfEmpty()
                            join doc in db.Doctors on _docService.DoctorId equals doc.DoctorId
                            join d in db.MedicineOnHandStock on item.ItemCode equals d.ITEMCD into _Inventory
                            from e in _Inventory.DefaultIfEmpty()
                            join b in db.ItemGenericNames on item.GenericCodeId equals b.Id
                            join c in db.ItemFormCodes on item.FormCodeId equals c.Id
                            where item.ItemType == "M" && item.HospitalId == jwtData.HospitalId && item.IsActive == "Y"
                            && doc.UserId == jwtData.Id
                            select new
                            {
                                value = item.ItemId,
                                label = item.ItemDesc + " [" + b.GenericDesc + " ]" + "[" + Convert.ToString(e.ONHANDQTY ?? "0") + "]",
                                IsFavorite = string.IsNullOrEmpty(_docService.IsFavorite) ? "No" : _docService.IsFavorite,
                                item.ItemCode,
                                c.FormCodeDesc

                            };
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.label.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(query, "Service"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret UpdateIsFavorite(DoctorFavMedicineServices entity, JwtStatus jwtData)
        {
            try
            {
                var doctorId = db.Doctors.Where(d => d.UserId == jwtData.Id).Select(d => d.DoctorId).FirstOrDefault();
                if (doctorId != null && !db.DoctorFavMedicineServices.Any(ser => ser.DoctorId == doctorId && ser.ServiceId == entity.ServiceId && ser.Type == 1))
                {
                    entity.Type = 1;
                    entity.DoctorId = Convert.ToInt32(doctorId);
                    db.DoctorFavMedicineServices.Add(entity);
                    db.SaveChanges();
                }
                else
                {
                    var existingService = db.DoctorFavMedicineServices.AsNoTracking().FirstOrDefault(ser => ser.DoctorId == doctorId && ser.ServiceId == entity.ServiceId && ser.Type == 1);
                    if (existingService != null)
                    {
                        existingService.IsFavorite = entity.IsFavorite;
                        db.DoctorFavMedicineServices.Update(existingService);
                        db.SaveChanges();
                    }
                }
                return new Ret { status = true, message = entity.IsFavorite == "Yes" ? "Medicine successfully added to favorites" : "Service successfully removed from favorites" };

            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong" };
            }
        }

    }
}
