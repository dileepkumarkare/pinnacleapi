using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pinnacle.Entities;
using Pinnacle.Helpers;
using Pinnacle.Helpers.JWT;
using Serilog;
namespace Pinnacle.Models



{
    public class DepartmentModal : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();
        CommonLogic cl = new CommonLogic();

        public Ret GetAllDepartments(Pagination entity)
        {

            try
            {
                var query = db.Department.Where(a => entity.Id == 0 || a.DepartmentId == entity.Id).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.DepartmentName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Department"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }

        public Ret GetAllDepartmentsLabel(int Id)
        {
            try
            {
                var res = db.Department.Where(a => Id == 0 || a.DepartmentId == Id).Select(a => new { value = a.DepartmentId, label = a.DepartmentCode + " - " + a.DepartmentName, IsMedical = a.IsMedical ?? "No" }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Department"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }


        public Ret SaveDepartment(DepartmentEntity entity, JwtStatus jwtData)
        {
            try
            {
                entity.CreatedBy = jwtData.Id;
                entity.HospitalId = jwtData.HospitalId;
                string msg;

                if (entity.DepartmentId == 0)
                {

                    var lastDepartment = db.Department.Where(x => x.DepartmentCode.StartsWith("DE")).OrderByDescending(x => x.DepartmentCode)
                        .Select(x => x.DepartmentCode).FirstOrDefault();
                    string newDepartmentCode = "DE001";

                    if (!string.IsNullOrEmpty(lastDepartment) && lastDepartment.Length > 2)
                    {
                        string numberPart = lastDepartment.Substring(2);
                        if (int.TryParse(numberPart, out int lastNumber))
                        {
                            newDepartmentCode = $"DE{(lastNumber + 1):D3}";
                        }
                    }

                    entity.DepartmentCode = newDepartmentCode;
                    db.Department.Add(entity);
                    msg = "Department saved successfully!";
                }
                else
                {
                    var existingDepartment = db.Department.AsNoTracking().FirstOrDefault(x => x.DepartmentId == entity.DepartmentId);
                    var prevData = JsonConvert.SerializeObject(existingDepartment);
                    if (existingDepartment != null)
                    {
                        db.Department.Update(entity);
                        msg = "Department updated successfully!";
                        //var auditLog = new AuditLog
                        //{
                        //    Module = "Department",
                        //    Action = "Update",
                        //    PreviousData = prevData,
                        //    ActionBy = jwtData.Id,
                        //    ActionDate = DateTime.Now
                        //};
                        //db.AuditLog.Add(auditLog);
                        //db.SaveChanges();
                    }
                    else
                    {
                        return new Ret { status = false, message = "Department not found." };
                    }
                }

                db.SaveChanges();
                return new Ret { status = true, message = msg };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Failed to save Department." };
            }
        }

    }

}

