using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Serilog;

namespace Pinnacle.Models
{
    public class ServiceModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();

        public Ret GetAllServices(Pagination entity, JwtStatus jwtData)
        {

            try
            {
                var query = (from a in db.Services.Where(a => (entity.Id == 0 || a.Id == entity.Id) && a.HospitalId == jwtData.HospitalId)
                             join b in db.ServiceGroup on a.ServiceGroupId equals b.ServiceGroupId
                             select new
                             {
                                 a.Id,
                                 a.ServiceCode,
                                 a.ServiceType,
                                 a.ServiceName,
                                 a.ServiceGroupId,
                                 a.Charge,
                                 a.HospitalId,
                                 a.BillingHeadId,
                                 a.IsActive,
                                 a.IsPackage,
                                 a.IsProcedure,
                                 a.IsDiet,
                                 a.IsOutSide,
                                 a.IsSampleNeeded,
                                 a.IsConsentSlipNeeded,
                                 a.ApplicableFor,
                                 a.CreatedBy,
                                 a.CreatedDate,
                                 a.UpdatedBy,
                                 a.UpdatedDate,
                                 b.ServiceGroupName
                             }
                             ).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.ServiceName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Service"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }

        public Ret GetAllServicesLabel(int Id, JwtStatus jwtData)
        {
            try
            {
                var res = db.Services.Where(a => (Id == 0 || a.Id == Id) && a.HospitalId == jwtData.HospitalId && a.IsActive == "Yes").Select(a => new { value = a.Id, label = a.ServiceCode + " - " + a.ServiceName, a.Charge }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Service"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }


        public Ret SaveService(ServiceEntity entity, JwtStatus jwtData)
        {
            try
            {
                entity.CreatedBy = jwtData.Id;
                entity.HospitalId = jwtData.HospitalId;
                string msg;

                if (entity.Id == 0)
                {
                    var _serviceGroupCode = db.ServiceGroup.Where(grp => grp.ServiceGroupId == entity.ServiceGroupId).Select(grp => grp.ServiceGroupCode).FirstOrDefault();
                    var lastService = db.Services.Where(x => x.ServiceCode.StartsWith(_serviceGroupCode) && x.ServiceGroupId == entity.ServiceGroupId).OrderByDescending(x => x)
                        .Select(x => x.ServiceCode).FirstOrDefault();
                    string newServiceCode = _serviceGroupCode.ToString() + "001";

                    if (!string.IsNullOrEmpty(lastService) && lastService.Length > 2)
                    {
                        string numberPart = lastService.Substring(3);
                        if (int.TryParse(numberPart, out int lastNumber))
                        {
                            newServiceCode = _serviceGroupCode.ToString() + $"{(lastNumber + 1):D3}";
                        }
                    }

                    entity.ServiceCode = newServiceCode;
                    db.Services.Add(entity);
                    msg = "Service saved successfully!";
                }
                else
                {
                    var existingService = db.Services.AsNoTracking().FirstOrDefault(x => x.Id == entity.Id);
                    if (existingService != null)
                    {
                        existingService.ServiceName = entity.ServiceName;
                        existingService.ServiceType = entity.ServiceType;
                        existingService.BillingHeadId = entity.BillingHeadId;
                        existingService.ApplicableFor = entity.ApplicableFor;
                        existingService.Charge = entity.Charge;
                        existingService.IsPackage = entity.IsPackage;
                        existingService.IsProcedure = entity.IsProcedure;
                        existingService.IsDiet = entity.IsDiet;
                        existingService.IsOutSide = entity.IsOutSide;
                        existingService.IsSampleNeeded = entity.IsSampleNeeded;
                        existingService.IsConsentSlipNeeded = entity.IsConsentSlipNeeded;
                        existingService.ServiceGroupId = entity.ServiceGroupId;
                        existingService.UpdatedBy = jwtData.Id;
                        existingService.UpdatedDate = DateTime.Now;
                        db.Services.Update(existingService);
                        msg = "Service updated successfully!";
                    }
                    else
                    {
                        return new Ret { status = false, message = "Service not found." };
                    }
                }

                db.SaveChanges();
                return new Ret { status = true, message = msg };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Failed to save Service." };
            }
        }
        public Ret UpdateIsFavorite(DoctorFavMedicineServices entity, JwtStatus jwtData)
        {
            try
            {
                var doctorId = db.Doctors.Where(d => d.UserId == jwtData.Id).Select(d => d.DoctorId).FirstOrDefault();
                if (doctorId != null && !db.DoctorFavMedicineServices.Any(ser => ser.DoctorId == doctorId && ser.ServiceId == entity.ServiceId && ser.Type == 0))
                {
                    entity.Type = 0;
                    entity.DoctorId = Convert.ToInt32(doctorId);
                    db.DoctorFavMedicineServices.Add(entity);
                    db.SaveChanges();
                }
                else
                {
                    var existingService = db.DoctorFavMedicineServices.AsNoTracking().FirstOrDefault(ser => ser.DoctorId == doctorId && ser.ServiceId == entity.ServiceId && ser.Type == 0);
                    if (existingService != null)
                    {
                        existingService.IsFavorite = entity.IsFavorite;
                        db.DoctorFavMedicineServices.Remove(existingService);
                        db.SaveChanges();
                    }
                }


                return new Ret { status = true, message = entity.IsFavorite == "Yes" ? "Service successfully added to favorites" : "Service successfully removed from favorites" };

            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong" };
            }
        }
        public Ret GetAllInvestigations(Pagination entity, JwtStatus jwtData)
        {
            try
            {
                var query = from service in db.Services
                            join serviceGroup in db.ServiceGroup on service.ServiceGroupId equals serviceGroup.ServiceGroupId
                            join docFavService in db.DoctorFavMedicineServices.Where(s => s.Type == 0 && s.DoctorId == entity.Id) on service.Id equals docFavService.ServiceId into docFavServices
                            from _docService in docFavServices.DefaultIfEmpty()
                            where service.ServiceType == "I" && service.HospitalId == jwtData.HospitalId && service.IsActive == "Yes"
                            select new
                            {
                                value = service.Id,
                                label = service.ServiceCode + " - " + service.ServiceName,
                                service.Charge,
                                IsFavorite = string.IsNullOrEmpty(_docService.IsFavorite) ? "No" : _docService.IsFavorite,
                                serviceGroup.ServiceGroupName

                            };
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.label.Contains(entity.SearchKey) || c.ServiceGroupName.Contains(entity.SearchKey));
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
        public Ret GetAllFavInvestigations(JwtStatus jwtData)
        {
            try
            {
                var res = from service in db.Services
                          join serviceGroup in db.ServiceGroup on service.ServiceGroupId equals serviceGroup.ServiceGroupId
                          join docFavService in db.DoctorFavMedicineServices.Where(s => s.Type == 0) on service.Id equals docFavService.ServiceId
                          join doc in db.Doctors on docFavService.DoctorId equals doc.DoctorId
                          where service.ServiceType == "I" && service.HospitalId == jwtData.HospitalId && service.IsActive == "Yes"
                          && docFavService.IsFavorite == "Yes" && doc.UserId == jwtData.Id
                          select new
                          {
                              value = service.Id,
                              label = service.ServiceName,
                              service.Charge,
                              IsFavorite = string.IsNullOrEmpty(docFavService.IsFavorite) ? "No" : docFavService.IsFavorite,
                              serviceGroup.ServiceGroupName

                          };

                return new Ret { status = true, message = FetchMessage(res, "Service"), data = res, };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
    }
}
