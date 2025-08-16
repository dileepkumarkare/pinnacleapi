using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Serilog;

namespace Pinnacle.Models
{
    public class ServiceGroupModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();

        public Ret GetAllServiceGroup(Pagination entity, JwtStatus jwtData)
        {

            try
            {
                var query = (from a in db.ServiceGroup.Where(a => (entity.Id == 0 || a.ServiceGroupId == entity.Id))
                             join b in db.LabReportingSettings on a.ServiceGroupId equals b.ServiceGroupId
                             join c in db.Department on a.DepartmentId equals c.DepartmentId
                             select new
                             {
                                 a.ServiceGroupId,
                                 a.ServiceGroupCode,
                                 a.ServiceGroupName,
                                 a.DepartmentId,
                                 a.HospitalId,
                                 a.IncludeInvestigation,
                                 a.SampleReq,
                                 a.IsAccessoinNoReq,
                                 a.AutomationRadiologyPost,
                                 a.UpdBtnInVerification,
                                 a.UpdBtnInApproval,
                                 a.BarcodePrintReq,
                                 a.GrossingandSubmitting,
                                 a.ReqAutoReportDispatch,
                                 a.ReqVerification,
                                 a.ReqDispatching,
                                 a.ReqApproval,
                                 a.ReqDigitalSign,
                                 a.ResultEntry,
                                 a.Verification,
                                 a.Approval,
                                 a.IsActive,
                                 a.S1,
                                 a.S2,
                                 a.CreatedBy,
                                 a.CreatedDate,
                                 a.UpdatedBy,
                                 a.UpdatedDate,
                                 b.ReportTitle,
                                 b.DoctorSignCaption,
                                 b.Suggessions,
                                 b.Note1,
                                 b.Note2,
                                 b.ParameterCap,
                                 b.ResultValueCap,
                                 b.NormalValueCap,
                                 b.MethodCap,
                                 b.UOMCap,
                                 b.BarcodePrefix,
                                 c.DepartmentCode,
                                 c.DepartmentName
                             })
                            .AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.ServiceGroupName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Service group"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }

        public Ret GetAllServiceGroupLabel(int Id, JwtStatus jwtData)
        {
            try
            {
                var res = db.ServiceGroup.Where(a => (Id == 0 || a.ServiceGroupId == Id) && a.HospitalId == jwtData.HospitalId && a.IsActive == "Yes").
                                          Select(a => new { value = a.ServiceGroupId, label = a.ServiceGroupCode, a.ServiceGroupName }).
                                          AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Service group"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }


        public Ret SaveServiceGroup(ServiceGroupEntity entity, JwtStatus jwtData)
        {
            try
            {
                entity.CreatedBy = jwtData.Id;
                entity.HospitalId = jwtData.HospitalId;
                string msg;

                if (entity.ServiceGroupId == 0)
                {

                    db.ServiceGroup.Add(entity);
                    msg = "Service group saved successfully!";
                }
                else
                {
                    var existingServiceGroup = db.ServiceGroup.AsNoTracking().FirstOrDefault(x => x.ServiceGroupId == entity.ServiceGroupId);
                    if (existingServiceGroup != null)
                    {

                        existingServiceGroup.ServiceGroupCode = entity.ServiceGroupCode;
                        existingServiceGroup.ServiceGroupName = entity.ServiceGroupName;
                        existingServiceGroup.DepartmentId = entity.DepartmentId;
                        existingServiceGroup.HospitalId = entity.HospitalId;
                        existingServiceGroup.IncludeInvestigation = entity.IncludeInvestigation;
                        existingServiceGroup.SampleReq = entity.SampleReq;
                        existingServiceGroup.IsAccessoinNoReq = entity.IsAccessoinNoReq;
                        existingServiceGroup.AutomationRadiologyPost = entity.AutomationRadiologyPost;
                        existingServiceGroup.UpdBtnInVerification = entity.UpdBtnInVerification;
                        existingServiceGroup.UpdBtnInApproval = entity.UpdBtnInApproval;
                        existingServiceGroup.BarcodePrintReq = entity.BarcodePrintReq;
                        existingServiceGroup.GrossingandSubmitting = entity.GrossingandSubmitting;
                        existingServiceGroup.ReqAutoReportDispatch = entity.ReqAutoReportDispatch;
                        existingServiceGroup.ReqVerification = entity.ReqVerification;
                        existingServiceGroup.ReqDispatching = entity.ReqDispatching;
                        existingServiceGroup.ReqApproval = entity.ReqApproval;
                        existingServiceGroup.ReqDigitalSign = entity.ReqDigitalSign;
                        existingServiceGroup.ResultEntry = entity.ResultEntry;
                        existingServiceGroup.Verification = entity.Verification;
                        existingServiceGroup.Approval = entity.Approval;
                        existingServiceGroup.IsActive = entity.IsActive;
                        existingServiceGroup.S1 = entity.S1;
                        existingServiceGroup.S2 = entity.S2;
                        existingServiceGroup.UpdatedBy = jwtData.Id;
                        existingServiceGroup.UpdatedDate = DateTime.Now;
                        db.ServiceGroup.Update(existingServiceGroup);
                        msg = "Service group updated successfully!";
                    }
                    else
                    {
                        return new Ret { status = false, message = "Service not found." };
                    }
                }

                db.SaveChanges();
                SaveLabReportSettings(entity);
                return new Ret { status = true, message = msg };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Failed to save Service group." };
            }
        }

        private void SaveLabReportSettings(ServiceGroupEntity entity)
        {
            try
            {

                string msg;

                if (!db.LabReportingSettings.Any(labSet => labSet.ServiceGroupId == entity.ServiceGroupId))
                {
                    var _labSettings = new LabReportingSettings
                    {
                        ServiceGroupId = entity.ServiceGroupId,
                        ReportTitle = entity.ReportTitle,
                        DoctorSignCaption = entity.DoctorSignCaption,
                        Suggessions = entity.Suggessions,
                        Note1 = entity.Note1,
                        Note2 = entity.Note2,
                        ParameterCap = entity.ParameterCap,
                        ResultValueCap = entity.ResultValueCap,
                        NormalValueCap = entity.NormalValueCap,
                        MethodCap = entity.MethodCap,
                        UOMCap = entity.UOMCap,
                        BarcodePrefix = entity.BarcodePrefix
                    };
                    db.LabReportingSettings.Add(_labSettings);
                    msg = "Service saved successfully!";
                }
                else
                {
                    var _existingReportSettings = db.LabReportingSettings.AsNoTracking().FirstOrDefault(x => x.ServiceGroupId == entity.ServiceGroupId);
                    if (_existingReportSettings != null)
                    {
                        _existingReportSettings.ReportTitle = entity.ReportTitle;
                        _existingReportSettings.DoctorSignCaption = entity.DoctorSignCaption;
                        _existingReportSettings.Suggessions = entity.Suggessions;
                        _existingReportSettings.Note1 = entity.Note1;
                        _existingReportSettings.Note2 = entity.Note2;
                        _existingReportSettings.ParameterCap = entity.ParameterCap;
                        _existingReportSettings.ResultValueCap = entity.ResultValueCap;
                        _existingReportSettings.NormalValueCap = entity.NormalValueCap;
                        _existingReportSettings.MethodCap = entity.MethodCap;
                        _existingReportSettings.UOMCap = entity.UOMCap;
                        _existingReportSettings.BarcodePrefix = entity.BarcodePrefix;
                        db.LabReportingSettings.Update(_existingReportSettings);
                        msg = "Service updated successfully!";
                    }

                }

                db.SaveChanges();

            }
            catch (Exception ex)
            {

            }
        }
    }
}
