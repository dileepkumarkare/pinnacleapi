using Microsoft.EntityFrameworkCore;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Serilog;

namespace Pinnacle.Models
{
    public class LabTestParameterModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();

        public Ret SaveLabTestParameters(LabTestParametersEntity entity, JwtStatus jwt)
        {
            Ret ret = new Ret();
            try
            {
                var existingParam = db.LabTestParameters.FirstOrDefault(x => x.Id == entity.Id);

                if (existingParam != null)
                {
                    // Update existing record
                    existingParam.ParamCode = entity.ParamCode;
                    existingParam.ShortName = entity.ShortName;
                    existingParam.HospitalId = jwt.HospitalId;
                    existingParam.ParamDesc = entity.ParamDesc;
                    existingParam.Method = entity.Method;
                    existingParam.IsIncAntiBiotics = entity.IsIncAntiBiotics;
                    existingParam.Units = entity.Units;
                    existingParam.TextSize = entity.TextSize;
                    existingParam.ParamDisplay = entity.ParamDisplay;
                    existingParam.IsAccreditationNeed = entity.IsAccreditationNeed;
                    existingParam.IsMultipleValues = entity.IsMultipleValues;
                    existingParam.IsNewIOrganism = entity.IsNewIOrganism;
                    existingParam.IsNormalRange = entity.IsNormalRange;
                    existingParam.IsCriticalRange = entity.IsCriticalRange;
                    existingParam.IsDefaultDRange = entity.IsDefaultDRange;
                    existingParam.Remarks = entity.Remarks;
                    existingParam.Notes = entity.Notes;
                    existingParam.IsActive = entity.IsActive;
                    existingParam.ModifyBy = entity.ModifyBy;
                    existingParam.ModifyDate = DateTime.UtcNow;
                    existingParam.LabGroupId = entity.LabGroupId;

                    db.SaveChanges();

                    // Remove old ParameterRanges
                    var oldRanges = db.ParameterRange.Where(x => x.ParamId == entity.Id).ToList();
                    if (oldRanges.Count > 0)
                    {
                        db.ParameterRange.RemoveRange(oldRanges);
                        db.SaveChanges();
                    }
                }
                else
                {
                    // Insert new record
                    entity.HospitalId = jwt.HospitalId;
                    entity.CreatedDate = DateTime.UtcNow;
                    db.LabTestParameters.Add(entity);
                    db.SaveChanges();
                }

                int paramId = existingParam != null ? existingParam.Id : entity.Id;

                // Save new ParameterRanges
                if (entity.ParameterRanges != null && entity.ParameterRanges.Count > 0)
                {
                    foreach (var range in entity.ParameterRanges)
                    {
                        range.ParamId = paramId;
                        db.ParameterRange.Add(range);
                    }
                    db.SaveChanges();
                }

                return new Ret { status = true, message = "Saved Successfully." };
            }
            catch (Exception ex)
            {
                Log.Information("Error while saving Lab Test Parameters at " + DateTime.Now + " - " + ex.Message);
                return new Ret { status = false, message = "Error while saving." };
            }
        }

        public Ret GetAllLabTestParameters(Pagination entity)
        {
            try
            {
                var allRanges = db.ParameterRange.AsNoTracking().ToList()
                                .GroupBy(r => r.ParamId ?? 0)
                                .ToDictionary(g => g.Key, g => g.ToList());

                var result = db.LabTestParameters.AsNoTracking()
                    .Select(param => new LabTestParametersEntity
                    {
                        Id = param.Id,
                        ParamCode = param.ParamCode,
                        ShortName = param.ShortName,
                        HospitalId = param.HospitalId,
                        ParamDesc = param.ParamDesc,
                        Method = param.Method,
                        IsIncAntiBiotics = param.IsIncAntiBiotics,
                        Units = param.Units,
                        TextSize = param.TextSize,
                        ParamDisplay = param.ParamDisplay,
                        IsAccreditationNeed = param.IsAccreditationNeed,
                        IsMultipleValues = param.IsMultipleValues,
                        IsNewIOrganism = param.IsNewIOrganism,
                        IsNormalRange = param.IsNormalRange,
                        IsCriticalRange = param.IsCriticalRange,
                        IsDefaultDRange = param.IsDefaultDRange,
                        Remarks = param.Remarks,
                        Notes = param.Notes,
                        IsActive = param.IsActive,
                        CreatedBy = param.CreatedBy,
                        CreatedDate = param.CreatedDate,
                        ModifyBy = param.ModifyBy,
                        ModifyDate = param.ModifyDate,
                        LabGroupId = param.LabGroupId,
                        ParameterRanges = allRanges.ContainsKey(param.Id) ? allRanges[param.Id] : new List<ParameterRange>()
                    })
                    .AsNoTracking();
                var res = PaginatedValues(result, entity);

                return new Ret
                {
                    status = true,
                    message = res.Count > 0 ? $"{res.Count} Lab Test Parameters fetched successfully." : "No Lab Test Parameters found.",
                    data = res,
                    totalCount = res.Count
                };
            }
            catch (Exception ex)
            {
                Log.Information("Error while fetching Lab Test Parameters at " + DateTime.Now + " - " + ex.Message);
                return new Ret
                {
                    status = false,
                    message = "Failed to fetch Lab Test Parameters."
                };
            }
        }
        public Ret GetByIdLabTestParameter(int id)
        {
            try
            {
                var allRanges = db.ParameterRange.AsNoTracking().Where(r => r.ParamId == id).ToList();

                var param = db.LabTestParameters.AsNoTracking()
                    .Where(p => p.Id == id)
                    .Select(p => new LabTestParametersEntity
                    {
                        Id = p.Id,
                        ParamCode = p.ParamCode,
                        ShortName = p.ShortName,
                        HospitalId = p.HospitalId,
                        ParamDesc = p.ParamDesc,
                        Method = p.Method,
                        IsIncAntiBiotics = p.IsIncAntiBiotics,
                        Units = p.Units,
                        TextSize = p.TextSize,
                        ParamDisplay = p.ParamDisplay,
                        IsAccreditationNeed = p.IsAccreditationNeed,
                        IsMultipleValues = p.IsMultipleValues,
                        IsNewIOrganism = p.IsNewIOrganism,
                        IsNormalRange = p.IsNormalRange,
                        IsCriticalRange = p.IsCriticalRange,
                        IsDefaultDRange = p.IsDefaultDRange,
                        Remarks = p.Remarks,
                        Notes = p.Notes,
                        IsActive = p.IsActive,
                        CreatedBy = p.CreatedBy,
                        CreatedDate = p.CreatedDate,
                        ModifyBy = p.ModifyBy,
                        ModifyDate = p.ModifyDate,
                        LabGroupId = p.LabGroupId,
                        ParameterRanges = allRanges
                    })
                    .FirstOrDefault();

                if (param == null)
                {
                    return new Ret
                    {
                        status = false,
                        message = "Lab Test Parameter not found."
                    };
                }

                return new Ret
                {
                    status = true,
                    message = "Lab Test Parameter fetched successfully.",
                    data = param
                };
            }
            catch (Exception ex)
            {
                Log.Information("Error while fetching Lab Test Parameter by ID at " + DateTime.Now + " - " + ex.Message);
                return new Ret
                {
                    status = false,
                    message = "Failed to fetch Lab Test Parameter."
                };
            }
        }
        public Ret GetAllLabParamLabel(int Id)
        {
            try
            {
                var res = db.LabTestParameters.Where(a => Id == 0 || a.Id == Id).Select(a => new { value = a.ParamCode, label = a.ShortName }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Labparams"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }

    }
}
