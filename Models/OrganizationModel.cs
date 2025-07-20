using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Serilog;
using System.Drawing;
using System.Reflection.Emit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Pinnacle.Models
{
    public class OrganizationModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();

        public Ret SaveOrganization(OrganizationEntity entity, JwtStatus jwtData, string IpAddress)
        {
            try
            {

                if (entity.OrganizationId == 0)
                {
                    entity.AddedBy = jwtData.Id;
                    entity.HospitalId = jwtData.HospitalId;
                    var latestOrgCode = db.Organization.Where(org => org.OrganizationCode.StartsWith("ORG")).OrderByDescending(org => org.OrganizationCode).Select(Org => Org.OrganizationCode).FirstOrDefault();
                    entity.OrganizationCode = "ORG0001";
                    if (!string.IsNullOrEmpty(latestOrgCode) && latestOrgCode.Length > 3)
                    {
                        string previousNumber = (latestOrgCode.Substring(3));
                        if (int.TryParse(previousNumber, out int latestNumber))
                        {
                            entity.OrganizationCode = $"ORG{(latestNumber + 1):D4}";
                        }

                    }
                    db.Organization.Add(entity);
                    db.SaveChanges();
                    SaveOrganizationTariff(entity);
                    SaveOrganizationAddress(entity);
                    return new Ret { status = true, message = "Organization saved successfully!", data = new { organizationId = entity.OrganizationId } };
                }
                else if (entity.OrganizationId > 0)
                {
                    var _existingOrganization = db.Organization.Where(org => org.OrganizationId == entity.OrganizationId).AsNoTracking().FirstOrDefault();
                    var prevData = JsonConvert.SerializeObject(_existingOrganization);
                    if (_existingOrganization is not null)
                    {
                        _existingOrganization.OrganizationName = entity.OrganizationName;
                        _existingOrganization.OrganizationType = entity.OrganizationType;
                        _existingOrganization.ContactNo = entity.ContactNo;
                        _existingOrganization.ContactDate = entity.ContactDate;
                        _existingOrganization.ContactPerson = entity.ContactPerson;
                        _existingOrganization.Color = entity.Color;
                        _existingOrganization.EffectFrom = entity.EffectFrom;
                        _existingOrganization.EffectTo = entity.EffectTo;
                        _existingOrganization.AuthorizedPerson = entity.AuthorizedPerson;
                        _existingOrganization.PanNumber = entity.PanNumber;
                        _existingOrganization.TanNumber = entity.TanNumber;
                        _existingOrganization.GSTCode = entity.GSTCode;
                        _existingOrganization.OpOrgPercentage = entity.OpOrgPercentage;
                        _existingOrganization.OpEmpPercentage = entity.OpEmpPercentage;
                        _existingOrganization.IpOrgPercentage = entity.IpOrgPercentage;
                        _existingOrganization.IpEmpPercentage = entity.IpEmpPercentage;
                        _existingOrganization.OpNos = entity.OpNos;
                        _existingOrganization.OpDays = entity.OpDays;
                        _existingOrganization.OpDisc = entity.OpDisc;
                        _existingOrganization.OpDaysForVisit = entity.OpDaysForVisit;
                        _existingOrganization.IpNos = entity.IpNos;
                        _existingOrganization.IpDays = entity.IpDays;
                        _existingOrganization.IpDisc = entity.IpDisc;
                        _existingOrganization.IpDaysForVisit = entity.IpDaysForVisit;
                        _existingOrganization.Pharmacy = entity.Pharmacy;
                        _existingOrganization.PharmacyDisc = entity.PharmacyDisc;
                        _existingOrganization.TariffPriorityFor = entity.TariffPriorityFor;
                        _existingOrganization.CorpConsultation = entity.CorpConsultation;
                        _existingOrganization.IsActive = entity.IsActive;
                        _existingOrganization.UpdatedBy = jwtData.Id;
                        _existingOrganization.UpdatedDate = DateTime.Now;
                        _existingOrganization.Category = entity.Category;
                    }
                    db.Organization.Update(_existingOrganization);
                    SaveOrganizationTariff(entity);
                    SaveOrganizationAddress(entity);
                    //AuditLog auditLog = new AuditLog();
                    //auditLog.Action = "Update";
                    //auditLog.ActionBy = jwtData.Id;
                    //auditLog.ActionDate = DateTime.Now;
                    //auditLog.Module = "Organization";
                    //auditLog.IPAddress = IpAddress;
                    //auditLog.PreviousData = prevData;
                    //db.AuditLog.Add(auditLog);
                }
                return new Ret { status = true, message = "Organization updated successfully!", data = new { organizationId = entity.OrganizationId } };
            }
            catch (Exception ex)
            {
                Log.Information("Organization Model=>Save Organization error at " + DateTime.Now.ToString() + " Message: " + ex.Message);
                return new Ret { status = false, message = "Something went wrong!, Please try again" };
            }
        }

        public Ret GetAllOrganization(Pagination entity, JwtStatus jwtData)
        {
            try
            {
                var query = (from a in db.Organization
                             join b in db.OrganizationTariff on a.OrganizationId equals b.OrganizationId
                             join c in db.OrganizationAddress on a.OrganizationId equals c.OrganizationId into organizationAddress
                             from orgAddr in organizationAddress.DefaultIfEmpty()
                             join pin in db.PincodeData on orgAddr.AreaCode equals pin.Id into pinCode
                             from pincode in pinCode.DefaultIfEmpty()
                             join dist in db.District on pincode.DistrictId equals dist.Id into districts
                             from district in districts.DefaultIfEmpty()
                             join st in db.States on district.StateId equals st.Id into states
                             from state in states.DefaultIfEmpty()
                             where (entity.Id == 0 || a.OrganizationId == entity.Id) && a.HospitalId == jwtData.HospitalId
                             orderby a.OrganizationId descending
                             select new
                             {
                                 a.OrganizationId,
                                 OrganizationName = a.OrganizationName ?? "",
                                 OrganizationCode = a.OrganizationCode ?? "",
                                 OrganizationType = a.OrganizationType ?? "",
                                 HospitalId = a.HospitalId ?? 0,
                                 ContactNo = a.ContactNo ?? "",
                                 ContactDate = a.ContactDate.HasValue ? a.ContactDate.Value.ToString("yyyy-MM-dd") : "",
                                 ContactPerson = a.ContactPerson ?? "",
                                 Color = a.Color ?? "",
                                 EffectFrom = a.EffectFrom.HasValue ? a.EffectFrom.Value.ToString("yyyy-MM-dd") : "",
                                 EffectTo = a.EffectTo.HasValue ? a.EffectTo.Value.ToString("yyyy-MM-dd") : "",
                                 AuthorizedPerson = a.AuthorizedPerson ?? "",
                                 PanNumber = a.PanNumber ?? "",
                                 TanNumber = a.TanNumber ?? "",
                                 GSTCode = a.GSTCode ?? "",
                                 OpOrgPercentage = a.OpOrgPercentage ?? 0,
                                 OpEmpPercentage = a.OpEmpPercentage ?? 0,
                                 IpOrgPercentage = a.IpOrgPercentage ?? 0,
                                 IpEmpPercentage = a.IpEmpPercentage ?? 0,
                                 OpNos = a.OpNos ?? 0,
                                 OpDays = a.OpDays ?? 0,
                                 OpDisc = a.OpDisc ?? 0,
                                 OpDaysForVisit = a.OpDaysForVisit ?? 0,
                                 IpNos = a.IpNos ?? 0,
                                 IpDays = a.IpDays ?? 0,
                                 IpDisc = a.IpDisc ?? 0,
                                 IpDaysForVisit = a.IpDaysForVisit != null && a.IpDaysForVisit != null ? a.IpDaysForVisit : 0,
                                 Pharmacy = a.Pharmacy ?? "",
                                 PharmacyDisc = a.PharmacyDisc ?? 0,
                                 TariffPriorityFor = a.TariffPriorityFor ?? "",     // ❗Fixed: added property name
                                 CorpConsultation = a.CorpConsultation ?? "",
                                 IsActive = a.IsActive ?? "",
                                 AddedBy = a.AddedBy != null ? a.AddedBy : 0,
                                 Category = a.Category ?? "",
                                 CreatedDate = a.CreatedDate != null ? a.CreatedDate.ToString("yyyy-MM-dd") : null,
                                 UpdatedBy = a.UpdatedBy ?? 0,
                                 UpdatedDate = a.UpdatedDate.HasValue ? a.UpdatedDate.Value.ToString("yyyy-MM-dd") : "",

                                 Id = b != null ? b.Id : 0,
                                 IpPriorityOne = b != null ? b.IpPriorityOne ?? 0 : 0,
                                 IpPriorityOneDisc = b != null ? b.IpPriorityOneDisc ?? 0 : 0,
                                 IpPriorityTwo = b != null ? b.IpPriorityTwo ?? 0 : 0,
                                 IpPriorityTwoDisc = b != null ? b.IpPriorityTwoDisc ?? 0 : 0,
                                 IpPriorityThree = b != null ? b.IpPriorityThree ?? 0 : 0,
                                 IpPriorityThreeDisc = b != null ? b.IpPriorityThreeDisc ?? 0 : 0,
                                 IpPriorityDefault = b != null ? b.IpPriorityDefault ?? 0 : 0,
                                 IpPriorityDefaultDisc = b != null ? b.IpPriorityDefaultDisc ?? 0 : 0,
                                 OpPriorityOne = b != null ? b.OpPriorityOne ?? 0 : 0,
                                 OpPriorityOneDisc = b != null ? b.OpPriorityOneDisc ?? 0 : 0,
                                 OpPriorityTwo = b != null ? b.OpPriorityTwo ?? 0 : 0,
                                 OpPriorityTwoDisc = b != null ? b.OpPriorityTwoDisc ?? 0 : 0,
                                 OpPriorityThree = b != null ? b.OpPriorityThree ?? 0 : 0,
                                 OpPriorityThreeDisc = b != null ? b.OpPriorityThreeDisc ?? 0 : 0,
                                 OpPriorityDefault = b != null ? b.OpPriorityDefault ?? 0 : 0,
                                 OpPriorityDefaultDisc = b != null ? b.OpPriorityDefaultDisc ?? 0 : 0,
                                 CorporateBillDoneIn = b != null ? b.CorporateBillDoneIn ?? 0 : 0,
                                 SubmitToMktgDeptIn = b != null ? b.SubmitToMktgDeptIn ?? 0 : 0,
                                 SubmitToOrganizationIn = b != null ? b.SubmitToOrganizationIn ?? 0 : 0,
                                 SummaryApprovalDays = b != null ? b.SummaryApprovalDays ?? 0 : 0,
                                 BillClearanceTime = b != null ? b.BillClearanceTime ?? 0 : 0,

                                 Area = orgAddr != null && orgAddr.Area != null ? orgAddr.Area : "",
                                 Address = orgAddr != null && orgAddr.Address != null ? orgAddr.Address : "",
                                 AreaCode = orgAddr != null && orgAddr.AreaCode.HasValue ? orgAddr.AreaCode.Value : 0,
                                 DistrictName = district != null && district.DistrictName != null ? district.DistrictName : "",
                                 StateName = state != null && state.StateName != null ? state.StateName : "",
                                 Country = orgAddr != null && orgAddr.Country != null ? orgAddr.Country : "",
                                 Pincode = pincode != null && pincode.Pincode.HasValue ? pincode.Pincode.Value : 0
                             }
                             ).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.OrganizationName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Organization"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }

        public Ret GetAllOrganizationLabel(int Id, JwtStatus jwtData)
        {
            try
            {
                var res = db.Organization.Where(a => (Id == 0 || a.OrganizationId == Id) && a.IsActive == "Yes" && a.HospitalId == jwtData.HospitalId).Select(a => new { value = a.OrganizationId, label = a.OrganizationName + " - " + a.OrganizationCode, isActive = a.IsActive ?? "No" }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Organization"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }

        private void SaveOrganizationTariff(OrganizationEntity entity)
        {
            try
            {
                var _organizationTariff = new OrganizationTariff
                {
                    IpPriorityOne = entity.IpPriorityOne,
                    IpPriorityOneDisc = entity.IpPriorityOneDisc,
                    IpPriorityTwo = entity.IpPriorityTwo,
                    IpPriorityTwoDisc = entity.IpPriorityTwoDisc,
                    IpPriorityThree = entity.IpPriorityThree,
                    IpPriorityThreeDisc = entity.IpPriorityThreeDisc,
                    IpPriorityDefault = entity.IpPriorityDefault,
                    IpPriorityDefaultDisc = entity.IpPriorityDefaultDisc,
                    OpPriorityOne = entity.OpPriorityOne,
                    OpPriorityOneDisc = entity.OpPriorityOneDisc,
                    OpPriorityTwo = entity.OpPriorityTwo,
                    OpPriorityTwoDisc = entity.OpPriorityTwoDisc,
                    OpPriorityThree = entity.OpPriorityThree,
                    OpPriorityThreeDisc = entity.OpPriorityThreeDisc,
                    OpPriorityDefault = entity.OpPriorityDefault,
                    OpPriorityDefaultDisc = entity.OpPriorityDefaultDisc,
                    CorporateBillDoneIn = entity.CorporateBillDoneIn,
                    SubmitToMktgDeptIn = entity.SubmitToMktgDeptIn,
                    SubmitToOrganizationIn = entity.SubmitToOrganizationIn,
                    SummaryApprovalDays = entity.SummaryApprovalDays,
                    BillClearanceTime = entity.BillClearanceTime
                };


                if (!db.OrganizationTariff.Any(org => org.OrganizationId == entity.OrganizationId))
                {
                    _organizationTariff.OrganizationId = entity.OrganizationId;
                    db.OrganizationTariff.Add(_organizationTariff);
                }
                else
                {
                    var _existingOrganization = db.OrganizationTariff.Where(org => org.OrganizationId == entity.OrganizationId).FirstOrDefault();
                    _existingOrganization.IpPriorityOne = entity.IpPriorityOne;
                    _existingOrganization.IpPriorityOneDisc = entity.IpPriorityOneDisc;
                    _existingOrganization.IpPriorityTwo = entity.IpPriorityTwo;
                    _existingOrganization.IpPriorityTwoDisc = entity.IpPriorityTwoDisc;
                    _existingOrganization.IpPriorityThree = entity.IpPriorityThree;
                    _existingOrganization.IpPriorityThreeDisc = entity.IpPriorityThreeDisc;
                    _existingOrganization.IpPriorityDefault = entity.IpPriorityDefault;
                    _existingOrganization.IpPriorityDefaultDisc = entity.IpPriorityDefaultDisc;
                    _existingOrganization.OpPriorityOne = entity.OpPriorityOne;
                    _existingOrganization.OpPriorityOneDisc = entity.OpPriorityOneDisc;
                    _existingOrganization.OpPriorityTwo = entity.OpPriorityTwo;
                    _existingOrganization.OpPriorityTwoDisc = entity.OpPriorityTwoDisc;
                    _existingOrganization.OpPriorityThree = entity.OpPriorityThree;
                    _existingOrganization.OpPriorityThreeDisc = entity.OpPriorityThreeDisc;
                    _existingOrganization.OpPriorityDefault = entity.OpPriorityDefault;
                    _existingOrganization.OpPriorityDefaultDisc = entity.OpPriorityDefaultDisc;
                    _existingOrganization.CorporateBillDoneIn = entity.CorporateBillDoneIn;
                    _existingOrganization.SubmitToMktgDeptIn = entity.SubmitToMktgDeptIn;
                    _existingOrganization.SubmitToOrganizationIn = entity.SubmitToOrganizationIn;
                    _existingOrganization.SummaryApprovalDays = entity.SummaryApprovalDays;
                    _existingOrganization.BillClearanceTime = entity.BillClearanceTime;
                    db.OrganizationTariff.Update(_existingOrganization);
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Information("Organization Tariff error at " + DateTime.Now + "message " + ex.Message);
            }
        }
        public Ret SaveCharges(OrganizationChargeEntity entity, JwtStatus jwtData)
        {
            try
            {
                foreach (var doctor in entity.DoctorIds)
                {
                    var _existingCharge = db.OrganizationCharges.FirstOrDefault(charge => charge.OrganizationId == entity.OrganizationId && charge.DoctorId == doctor.Value);
                    if (_existingCharge != null)
                    {
                        _existingCharge.OpCharge = entity.OpCharge is null ? _existingCharge.OpCharge : entity.OpCharge;
                        _existingCharge.IpCharge = entity.IpCharge is null ? _existingCharge.IpCharge : entity.IpCharge;
                        _existingCharge.UpdateBy = jwtData.Id;
                        _existingCharge.UpdatedDate = DateTime.Now;
                        db.OrganizationCharges.Update(_existingCharge);
                    }
                    else
                    {
                        var _OrgCharges = new OrganizationChargeEntity
                        {
                            OrganizationId = entity.OrganizationId,
                            DoctorId = doctor.Value,
                            OpCharge = entity.OpCharge,
                            IpCharge = entity.IpCharge,
                            CreatedBy = jwtData.Id,
                            CreatedDate = DateTime.Now,
                        };
                        db.OrganizationCharges.Add(_OrgCharges);
                    }

                    db.SaveChanges();
                }


                return new Ret { status = true, message = "Organization charge updated successfully!", data = new { organizationChargeId = entity.Id } };
            }
            catch (Exception ex)
            {
                Log.Information("Organization Model=>Save Organization Charge error at " + DateTime.Now.ToString() + " Message: " + ex.Message);
                return new Ret { status = false, message = "Something went wrong!, Please try again" };
            }
        }
        public Ret UpdateDoctorCharge(OrganizationChargeEntity entity, JwtStatus jwtData)
        {
            try
            {

                var _existingCharge = db.OrganizationCharges.FirstOrDefault(charge => charge.OrganizationId == entity.OrganizationId && charge.DoctorId == entity.DoctorId);
                if (_existingCharge != null)
                {
                    _existingCharge.OpCharge = entity.OpCharge is null ? _existingCharge.OpCharge : entity.OpCharge;
                    _existingCharge.IpCharge = entity.IpCharge is null ? _existingCharge.IpCharge : entity.IpCharge;
                    _existingCharge.IsActive = entity.IsActive is null ? _existingCharge.IsActive : entity.IsActive;
                    _existingCharge.UpdateBy = jwtData.Id;
                    _existingCharge.UpdatedDate = DateTime.Now;
                    db.OrganizationCharges.Update(_existingCharge);
                }
                db.SaveChanges();
                return new Ret { status = true, message = "Organization charge updated successfully!", data = new { organizationChargeId = entity.Id } };
            }
            catch (Exception ex)
            {
                Log.Information("Organization Model=>Save Organization Charge error at " + DateTime.Now.ToString() + " Message: " + ex.Message);
                return new Ret { status = false, message = "Something went wrong!, Please try again" };
            }
        }
        public Ret GetCharges(Pagination entity)
        {
            try
            {
                var query = (from a in db.OrganizationCharges
                             join b in db.Doctors on a.DoctorId equals b.DoctorId
                             join c in db.Department on b.DepartmentId equals c.DepartmentId
                             join d in db.Designation on b.DesignationId equals d.DesignationId
                             join e in db.Users on b.UserId equals e.Id
                             where a.OrganizationId == entity.Id
                             select new
                             {
                                 a.OrganizationId,
                                 DoctorName = e.UserName + " - " + b.DoctorName,
                                 DepartmentName = c.DepartmentCode + " - " + c.DepartmentName,
                                 DesignationName = d.DesignationCode + " - " + d.DesignationName,
                                 b.DoctorId,
                                 a.OpCharge,
                                 a.IpCharge,
                                 a.CreatedDate,
                                 a.CreatedBy,
                                 a.IsActive
                             }).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.DoctorName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Organization"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = NoDataMessage() };
            }
        }

        public Ret UpdateStatus(OrganizationChargeEntity entity, JwtStatus jwtData)
        {
            try
            {

                var _existingCharge = db.OrganizationCharges.FirstOrDefault(charge => charge.OrganizationId == entity.OrganizationId && charge.DoctorId == entity.DoctorId);
                if (_existingCharge != null)
                {
                    _existingCharge.IsActive = entity.IsActive;
                    db.OrganizationCharges.Update(_existingCharge);
                }
                db.SaveChanges();
                return new Ret { status = true, message = "Status updated successfully!", data = new { organizationChargeId = entity.Id } };
            }
            catch (Exception ex)
            {
                Log.Information("Organization Model=>Save Organization Charge error at " + DateTime.Now.ToString() + " Message: " + ex.Message);
                return new Ret { status = false, message = "Something went wrong!, Please try again" };
            }
        }

        public Ret GetDoctors(OnlyId entity, int Visit = 0, int Days = 0)
        {
            try
            {
                var res = (from a in db.OrganizationCharges
                           join b in db.Doctors on a.DoctorId equals b.DoctorId
                           join c in db.Department on b.DepartmentId equals c.DepartmentId
                           join e in db.Users on b.UserId equals e.Id
                           join f in db.Organization on a.OrganizationId equals f.OrganizationId
                           where a.OrganizationId == entity.Id && a.IsActive == "Yes"
                           select new
                           {
                               value = a.DoctorId,
                               Label = e.UserName + " - " + b.DoctorName,
                               a.OrganizationId,
                               DepartmentName = c.DepartmentCode + " - " + c.DepartmentName,
                               consultationFee = Visit <= Convert.ToInt32(f.OpNos) && Visit > 1 ? 0 : a.OpCharge,
                               IpConsultationCharge = a.IpCharge
                           }).AsNoTracking();

                return new Ret { status = true, message = FetchMessage(res, "Organization"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = NoDataMessage() };
            }
        }
        public Ret GetPatientList(OnlyId entity)
        {
            try
            {
                var _patientList = (from org in db.Organization
                                    join cons in db.CorporateRegistration on org.OrganizationId equals cons.OrganizationId
                                    join patient in db.Patient on cons.PatientId equals patient.PatientId
                                    join user in db.Users on patient.UserId equals user.Id
                                    where (org.OrganizationId == entity.Id)
                                    select new
                                    {
                                        value = patient.PatientId,
                                        label = user.UserName + " - " + user.UserFullName,
                                        umrNumber = user.UserName
                                    }).Distinct().ToList();

                return new Ret { status = true, data = _patientList, message = "Patient list loaded successfully" };
            }
            catch (Exception ex)
            {
                Log.Information("Organization Model=>get patient list error at " + DateTime.Now.ToString() + " Message: " + ex.Message);
                return new Ret { status = false, message = "Something went wrong" };
            }
        }
        public Ret SaveOrganizationAddress(OrganizationEntity organization)
        {
            try
            {
                if (!db.OrganizationAddress.Any(org => org.OrganizationId == organization.OrganizationId))
                {
                    OrganizationAddress organizationAddress = new OrganizationAddress
                    {
                        OrganizationId = organization.OrganizationId,
                        Address = organization.Address,
                        Area = organization.Area,
                        AreaCode = organization.AreaCode,
                        Country = organization.Country
                    };
                    db.OrganizationAddress.Add(organizationAddress);
                    db.SaveChanges();
                    return new Ret { status = true };
                }
                else
                {
                    var _existingAddress = db.OrganizationAddress.Where(org => org.OrganizationId == organization.OrganizationId).FirstOrDefault();
                    if (_existingAddress != null)
                    {
                        _existingAddress.OrganizationId = organization.OrganizationId;
                        _existingAddress.Address = organization.Address;
                        _existingAddress.Area = organization.Area;
                        _existingAddress.AreaCode = organization.AreaCode;
                        _existingAddress.Country = organization.Country;
                        db.OrganizationAddress.Update(_existingAddress);
                        db.SaveChanges();
                        return new Ret { status = true };
                    }
                    return new Ret { status = false };
                }
            }
            catch (Exception ex)
            {
                Log.Information("Organization Model=> Organization Address error at " + DateTime.Now.ToString() + " error :" + ex.Message);
                return new Ret { status = false, message = "Something went wrong" };
            }
        }

    }
}