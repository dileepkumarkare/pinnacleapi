using DevExpress.CodeParser;
using DevExpress.DataAccess.Native.Json;
using DevExpress.Export.Xl;
using DevExpress.Utils.Internal;
using DevExpress.XtraCharts;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraRichEdit.API.Native;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Pinnacle.Entities;
using Pinnacle.Helpers;
using Pinnacle.Helpers.JWT;
using Serilog;
using System.Numerics;
using System.Reflection.PortableExecutable;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace Pinnacle.Models
{
    public class OPBillingModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();
        CommonLogic CL = new CommonLogic();
        public Ret GetOSPandBillNumber(JwtStatus jwtData)
        {
            try
            {
                string prefix = jwtData.HospitalId == 1 ? "BOP" : "BOS";
                string NewOPBillNo = string.Concat(prefix, DateTime.Now.ToString("yyMMdd"), "001");
                string NewOSPNo = $"OSP{DateTime.UtcNow.ToString("yyMMdd")}001";
                string NewBRecNo = "BREC000001";
                string condition = string.Concat(prefix, DateTime.Now.ToString("yyMMdd"));
                var _lastOPBillNo = db.OPBillReceipt.Where(bill => bill.OpBillRecNo.StartsWith(condition)).Select(bill => bill.OpBillRecNo).OrderByDescending(bill => bill).FirstOrDefault();
                var _lastOSPNo = db.OSPatient.Where(osp => osp.OspNo.StartsWith("OSP")).Select(osp => osp.OspNo).OrderByDescending(osp => osp).FirstOrDefault();
                if (!string.IsNullOrEmpty(_lastOPBillNo))
                {
                    string _lastNumber = _lastOPBillNo.Substring(3);
                    NewOPBillNo = int.TryParse(_lastNumber, out int lastNumber) && _lastOPBillNo.Substring(3, 6) == DateTime.UtcNow.ToString("yyMMdd") ? $"{prefix}{(lastNumber + 1):D3}" : NewOPBillNo;
                }
                if (!string.IsNullOrEmpty(_lastOSPNo))
                {
                    string _lastNumber = _lastOSPNo.Substring(3);
                    NewOSPNo = int.TryParse(_lastNumber, out int lastNumber) ? $"OSP{(lastNumber + 1):D6}" : NewOSPNo;
                }
                return new Ret { status = true, data = new { billNo = NewOPBillNo, ospNo = NewOSPNo }, message = "Osp and bill numbers loaded successfully." };
            }
            catch (Exception ex)
            {
                Log.Information("OPBilling Model => GetOSP and BillNumber error at " + nameof(DateTime.Now) + " error message" + ex.Message);
                return new Ret { status = false, message = "Something went wrong" };
            }
        }
        public Ret SaveOPBilling(OPBillingCollection entity, JwtStatus jwtData)
        {
            try
            {

                if (entity.OpBillId == 0)
                {
                    var _opBilling = new OPBillingEntity
                    {
                        BillType = entity.BillType,
                        ConsultationId = entity.ConsultationId,
                        PatientId = SaveOSPatient(entity),
                        BillNo = entity.BillNo,
                        BillDate = entity.BillDate,
                        ConsultantId = entity.DoctorId,
                        IsFree = entity.IsFree,
                        VIPSource = entity.VIPSource,
                        VIPRemarks = entity.VIPRemarks,
                        FreeAuthorizedBy = entity.FreeAuthorizedBy,
                        OverAllConcPercentage = entity.OverAllConcPercentage,
                        OverAllConcAmount = entity.OverAllConcAmount,
                        GrossAmount = entity.GrossAmount,
                        NetAmount = entity.NetAmount,
                        IsCancelled = entity.IsCancelled,
                        CanAuthorizedBy = entity.CanAuthorizedBy,
                        CreatedBy = jwtData.Id,
                        CreatedDate = entity.CreatedDate,
                        Remarks = entity.Remarks,
                        HospitalId = jwtData.HospitalId,
                        OrganizationId = entity.OrganizationId,
                        SourceofRef = entity.Refferal,
                        ReferredBy = Convert.ToInt32(entity.ReferredBy),
                        RefNo = entity.RefNo

                    };
                    db.OPBilling.Add(_opBilling);
                    db.SaveChanges();
                    entity.OpBillId = _opBilling.OPBillId;
                    SaveOpServices(entity);
                    //SaveOpBillReceipt(entity);
                }
                return new Ret { status = true, data = new { OpBillId = entity.OpBillId }, message = "OP billing saved successfully" };
            }
            catch (Exception ex)
            {
                Log.Information("OPBilling Model => Save OP Billing error at " + nameof(DateTime.Now) + " error message" + ex.Message);
                return new Ret { status = false, message = "Something went wrong" };
            }
        }
        public int SaveOSPatient(OPBillingCollection entity)
        {
            try
            {
                if (entity.BillType == "OSP")
                {
                    var _ospatient = new OSPatientEntity
                    {
                        OspNo = entity.OspNo,
                        PatientName = entity.PatientName,
                        Email = entity.Email,
                        ContactNo = entity.ContactNo,
                        Relation = entity.Relation,
                        RelationName = entity.RelationName,
                        Dob = entity.Dob,
                        Age = entity.Age,
                        Gender = entity.Gender,
                        OccupationId = entity.OccupationId,
                        RefSource = entity.Refferal,
                        RefBy = entity.ReferredBy,
                        BloodGroup = entity.BloodGroup,
                        Nationality = entity.Nationality
                    };
                    db.OSPatient.Add(_ospatient);
                    db.SaveChanges();
                    return Convert.ToInt32(_ospatient.PatientId);
                }
                else
                {
                    return Convert.ToInt32(entity.PatientId);
                }
            }
            catch (Exception ex)
            {
                Log.Information("OPBilling Model => Save OP Billing error at " + nameof(DateTime.Now) + " error message" + ex.Message);
                return 0;

            }
        }
        public Ret SaveOpBillReceipt(OPBillingCollection entity, JwtStatus jwtData, string ipaddress)
        {
            try
            {

                if (!db.OPBillReceipt.Any(op => op.OpBillId == entity.OpBillId))
                {
                    var _opbillingReceipt = new OPBillReceiptEntity
                    {
                        OpBillRecNo = entity.OpBillRecNo,
                        OpBillId = entity.OpBillId,
                        RecDate = DateTime.UtcNow,
                        RecAmount = entity.RecAmount,
                        Concession = entity.Concession,
                        ConcAuthorizedBy = entity.ConcAuthorizedBy,
                        DueAmount = entity.DueAmount,
                        DueAuthorizedBy = entity.DueAuthorizedBy,
                        PaymentType = entity.PaymentType,
                        TxnNo = entity.TxnNo,
                        BankId = entity.BankId,
                        PaymentStatus = entity.PaymentStatus,
                        CreatedBy = jwtData.Id,
                        CreatedDate = DateTime.UtcNow
                    };
                    db.OPBillReceipt.Add(_opbillingReceipt);
                    db.SaveChanges();
                    var _existingBills = db.OPBilling.Where(b => b.OPBillId == entity.OpBillId).AsNoTracking().FirstOrDefault();
                    if (_existingBills is OPBillingEntity)
                    {
                        _existingBills.OverAllConcPercentage = entity.OverAllConcPercentage;
                        _existingBills.GrossAmount = entity.GrossAmount;
                        _existingBills.NetAmount = entity.NetAmount;
                        _existingBills.BillNo = entity.OpBillRecNo;
                        _existingBills.OverAllConcAmount = (Convert.ToDecimal(entity.GrossAmount) - Convert.ToDecimal(entity.NetAmount));
                        db.OPBilling.Update(_existingBills);
                        db.SaveChanges();
                    }

                    return new Ret { status = true, data = new { OpBillRecId = _opbillingReceipt.OpBillRecId }, message = "Receipt saved successfully." };
                }

                return new Ret { status = false, message = "This receipt already saved." };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong" };
            }
        }

        public Ret GetOpBillReceipt(int Id)
        {
            try
            {
                var _opbillingReceipt = from a in db.OPBillReceipt
                                        where a.OpBillId == Id
                                        select new
                                        {
                                            OpBillRecNo = a.OpBillRecNo,
                                            OpBillId = a.OpBillId,
                                            RecDate = a.RecDate,
                                            RecAmount = a.RecAmount,
                                            Concession = a.Concession,
                                            ConcAuthorizedBy = a.ConcAuthorizedBy,
                                            DueAmount = a.DueAmount,
                                            DueAuthorizedBy = a.DueAuthorizedBy,
                                            PaymentType = a.PaymentType,
                                            TxnNo = a.TxnNo,
                                            BankId = a.BankId,
                                            PaymentStatus = a.PaymentStatus
                                        };
                return new Ret { status = true, data = _opbillingReceipt, message = "Receipt saved successfully." };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong" };
            }
        }
        public void SaveOpServices(OPBillingCollection entity)
        {
            try
            {
                if (entity.Services is not null)
                {
                    foreach (var OPService in entity.Services)
                    {
                        var _opServices = new OPServiceBookingEntity
                        {
                            OpBillId = entity.OpBillId,
                            ServiceId = OPService.ServiceId,
                            Qty = OPService.Qty,
                            Rate = OPService.Rate,
                            TotalAmount = OPService.TotalAmount
                        };
                        db.OPServiceBooking.Add(_opServices);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }

        }
        public Ret GetServices(OPBillingFilter entity, JwtStatus jwtData)
        {
            try
            {
                if (!entity.PatientType.Equals("Corporate"))
                {
                    var _services = (from service in db.Services
                                     join tariffService in db.TariffServiceMapping on service.Id equals tariffService.ServiceId
                                     where (service.ServiceGroupId == entity.ServiceGroupId || entity.ServiceGroupId == 0) && tariffService.TariffId == 1 && service.IsActive == "Yes" &&
                                                                          new[] { "BF", "BV", "OP" }.Contains(service.ApplicableFor) && tariffService.HospitalId == jwtData.HospitalId
                                     select new { value = service.Id, label = service.ServiceName, service.ServiceCode, Rate = service.Charge }).AsNoTracking();
                    return new Ret { status = true, data = _services, message = "Services loaded successfully!" };
                }
                else
                {
                    var _services = (from service in db.Services
                                     where new[] { "BF", "BV", "OP" }.Contains(service.ApplicableFor) && service.IsActive == "Yes"
                                     select new { value = service.Id, label = service.ServiceName, service.ServiceCode, Rate = service.Charge }).AsNoTracking();
                    return new Ret { status = true, data = _services, message = "Services loaded successfully!" };
                }
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong!" };
                Log.Information("OPBilling Model => get OP services error at " + nameof(DateTime.Now) + " error message" + ex.Message);
            }
        }
        public Ret GetServiceCharge(OPBillingFilter entity, JwtStatus jwtData)
        {
            try
            {
                if (entity.OrganizationId > 0)
                {
                    var OpBillingService = db.OrganizationTariff.Where(org => org.OrganizationId == entity.OrganizationId).FirstOrDefault();
                    if (OpBillingService.OpPriorityDefault == null)
                    {
                        return new Ret { status = false, message = "Default tariff is not assigned to the selected organization, please assign it before proceeding" };
                    }

                    var priorities = new[]
                     {
                    OpBillingService.OpPriorityOne,
                    OpBillingService.OpPriorityTwo,
                    OpBillingService.OpPriorityThree,
                    OpBillingService.OpPriorityDefault

                };
                    foreach (var priority in priorities)
                    {
                        var _serviceAmount = (from a in db.TariffServiceMapping
                                              join b in db.Services on a.ServiceId equals b.Id
                                              where a.ServiceId == entity.Id && a.TariffId == priority
                                              select new { a.Charge, ServiceName = a.TariffServiceName, ServiceCode = a.TariffServiceCode, a.Id }).AsNoTracking().FirstOrDefault();
                        if (_serviceAmount is not null)
                        {
                            return new Ret { status = true, data = new { value = _serviceAmount.Id, label = _serviceAmount.ServiceName, _serviceAmount.ServiceCode, Rate = _serviceAmount.Charge, IsGeneral = priority == 1 ? "Yes" : "No" } };
                        }
                    }
                }
                else
                {
                    var _serviceAmount = (from a in db.TariffServiceMapping
                                          join b in db.Services on a.ServiceId equals b.Id
                                          where a.ServiceId == entity.Id && a.TariffId == 1 && a.HospitalId == jwtData.HospitalId
                                          select new { a.Charge, b.ServiceName, b.ServiceCode, a.Id }).AsNoTracking().FirstOrDefault();
                    if (_serviceAmount is not null)
                    {
                        return new Ret { status = true, data = new { value = _serviceAmount.Id, label = _serviceAmount.ServiceName, _serviceAmount.ServiceCode, Rate = _serviceAmount.Charge, IsGeneral = "Yes" } };
                    }
                }

                return new Ret { status = false, message = "This service is not assigned to any tariff. Please assign it before proceeding." };

            }
            catch (Exception ex)
            {
                Log.Information("OPBilling Model => get OP services error at " + nameof(DateTime.Now) + " error message" + ex.Message);
                return new Ret { status = false, message = "Something went wrong!" };

            }

        }
        public Ret GetOpBilling(Pagination entity, JwtStatus jwtData)
        {
            try
            {
                var query = (from a in db.OPBilling
                             join b in db.OPServiceBooking on a.OPBillId equals b.OpBillId into serviceGroup
                             join c in db.Patient on a.PatientId equals c.PatientId into patients
                             from h in patients.DefaultIfEmpty()
                             join d in db.Users on h.UserId equals d.Id into pUsers
                             from i in pUsers.DefaultIfEmpty()
                             join e in db.OSPatient on a.PatientId equals e.PatientId into OSpatients
                             from f in OSpatients.DefaultIfEmpty()
                             join g in db.Users on a.CreatedBy equals g.Id
                             join j in db.OPBillReceipt on a.OPBillId equals j.OpBillId into receipt
                             from k in receipt.DefaultIfEmpty()
                             join l in db.Users on k.DueAuthorizedBy equals l.Id into dueAuth
                             from m in dueAuth.DefaultIfEmpty()
                             join n in db.BankMaster on k.BankId equals n.Id into bank
                             from o in bank.DefaultIfEmpty()
                             join p in db.Doctors on a.ConsultantId equals p.DoctorId into doctors
                             from q in doctors.DefaultIfEmpty()
                             join r in db.Organization on a.OrganizationId equals r.OrganizationId into organizations
                             from org in organizations.DefaultIfEmpty()
                             join s in db.Users.AsEnumerable() on Convert.ToInt32(a.ReferredBy ?? .0) equals s.Id into refusers
                             from t in refusers.DefaultIfEmpty()
                             join u in db.TitleMaster on i.TitleId equals u.TitleId into ptitle
                             from v in ptitle.DefaultIfEmpty()
                             where a.HospitalId == jwtData.HospitalId && (entity.Id == 0 || a.OPBillId == entity.Id)

                             orderby a.OPBillId descending
                             select new
                             {
                                 a.OPBillId,
                                 a.BillType,
                                 a.ConsultationId,
                                 a.OrganizationId,
                                 org.OrganizationName,
                                 a.PatientId,
                                 a.BillNo,
                                 a.BillDate,
                                 DoctorId = a.ConsultantId,
                                 a.IsFree,
                                 a.VIPSource,
                                 a.VIPRemarks,
                                 a.FreeAuthorizedBy,
                                 a.OverAllConcPercentage,
                                 a.OverAllConcAmount,
                                 a.GrossAmount,
                                 a.NetAmount,
                                 a.IsCancelled,
                                 a.CanAuthorizedBy,
                                 a.CreatedBy,
                                 a.CreatedDate,
                                 a.CancelledBy,
                                 a.CancelledDate,
                                 a.Remarks,
                                 a.HospitalId,
                                 services = (from _serviceGroup in serviceGroup
                                             join trf in db.TariffServiceMapping on _serviceGroup.ServiceId equals trf.Id

                                             select new
                                             {
                                                 _serviceGroup.ServiceBookingId,
                                                 _serviceGroup.OpBillId,
                                                 Value = trf.TariffServiceCode,
                                                 Label = trf.TariffServiceName,
                                                 _serviceGroup.Qty,
                                                 _serviceGroup.TotalAmount,
                                                 _serviceGroup.Rate
                                             }).ToList(),
                                 umrNumber = a.BillType == "OSP" ? f.OspNo : i.UserName,
                                 PatientName = a.BillType == "OSP" ? f.PatientName : i.UserFullName,
                                 v.Title,
                                 CreatedUserName = g.UserFullName,
                                 OpBillRecNo = k.OpBillRecNo,
                                 RecDate = k.RecDate.HasValue ? Convert.ToDateTime(k.RecDate).ToString("yyyy-MM-dd") : null,
                                 RecAmount = k.RecAmount,
                                 ConcAuthorizedBy = k.ConcAuthorizedBy,
                                 DueAmount = k.DueAmount,
                                 DueAuthorizedBy = k.DueAuthorizedBy,
                                 PaymentType = k.PaymentType,
                                 TxnNo = k.TxnNo,
                                 BankId = k.BankId,
                                 PaymentStatus = k.PaymentStatus,
                                 DueAuthorizedId = m.UserName,
                                 BankName = o.BankName,
                                 Email = a.BillType == "OSP" ? f.Email : i.Email,
                                 ContactNo = a.BillType == "OSP" ? f.ContactNo : i.ContactNo,
                                 Relation = a.BillType == "OSP" ? f.Relation : h.Relation,
                                 RelationName = a.BillType == "OSP" ? f.RelationName : h.RelationName,
                                 Dob = a.BillType == "OSP" ? Convert.ToDateTime(f.Dob).ToString("yyyy-MM-dd") : Convert.ToDateTime(i.DOB).ToString("yyyy-MM-dd"),
                                 Age = a.BillType == "OSP" ? f.Age : h.Age,
                                 Gender = a.BillType == "OSP" ? f.Gender : i.Gender,
                                 OccupationId = a.BillType == "OSP" ? f.OccupationId : h.OccupationId,
                                 RefSource = a.SourceofRef,
                                 RefBy = a.ReferredBy,
                                 BloodGroup = a.BillType == "OSP" ? f.BloodGroup : h.BloodGroup,
                                 Nationality = a.BillType == "OSP" ? f.Nationality : h.Nationality,
                                 q.DoctorName,
                                 RefByName = t.UserFullName


                             }).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.BillNo.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "OpBilling"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                return new Ret();
            }
        }
        public Ret GetOpCorporateBilling(Pagination entity, JwtStatus jwtData)
        {
            try
            {
                var query = (from a in db.OPBilling
                             join c in db.Patient on a.PatientId equals c.PatientId into patients
                             from h in patients.DefaultIfEmpty()
                             join d in db.Users on h.UserId equals d.Id into pUsers
                             from i in pUsers.DefaultIfEmpty()
                             join g in db.Users on a.CreatedBy equals g.Id
                             join j in db.OPBillReceipt on a.OPBillId equals j.OpBillId into receipt
                             from k in receipt.DefaultIfEmpty()
                             join l in db.Users on k.DueAuthorizedBy equals l.Id into dueAuth
                             from m in dueAuth.DefaultIfEmpty()
                             join n in db.BankMaster on k.BankId equals n.Id into bank
                             from o in bank.DefaultIfEmpty()
                             join p in db.Doctors on a.ConsultantId equals p.DoctorId into doctors
                             from q in doctors.DefaultIfEmpty()
                             join r in db.Organization on a.OrganizationId equals r.OrganizationId into organizations
                             from org in organizations.DefaultIfEmpty()
                             join u in db.TitleMaster on i.TitleId equals u.TitleId into ptitle
                             from v in ptitle.DefaultIfEmpty()
                             where a.HospitalId == jwtData.HospitalId && (entity.Id == 0 || a.OPBillId == entity.Id)
                             && a.BillType == "Corporate"
                             orderby a.OPBillId descending
                             select new
                             {
                                 a.OPBillId,
                                 a.BillType,
                                 a.ConsultationId,
                                 a.OrganizationId,
                                 org.OrganizationName,
                                 a.PatientId,
                                 a.BillNo,
                                 a.BillDate,
                                 DoctorId = a.ConsultantId,
                                 a.IsFree,
                                 a.VIPSource,
                                 a.VIPRemarks,
                                 a.FreeAuthorizedBy,
                                 a.OverAllConcPercentage,
                                 a.OverAllConcAmount,
                                 a.GrossAmount,
                                 a.NetAmount,
                                 a.IsCancelled,
                                 a.CanAuthorizedBy,
                                 a.CreatedBy,
                                 a.CreatedDate,
                                 a.CancelledBy,
                                 a.CancelledDate,
                                 a.Remarks,
                                 a.HospitalId,
                                 services = (from b in db.OPServiceBooking
                                             join trf in db.TariffServiceMapping on b.ServiceId equals trf.Id
                                             where b.OpBillId == a.OPBillId
                                             select new
                                             {
                                                 b.ServiceBookingId,
                                                 b.OpBillId,
                                                 Value = trf.TariffServiceCode,
                                                 Label = trf.TariffServiceName,
                                                 b.Qty,
                                                 b.TotalAmount,
                                                 b.Rate
                                             }).ToList(),
                                 umrNumber = i.UserName,
                                 PatientName = i.UserFullName,
                                 v.Title,
                                 CreatedUserName = g.UserFullName,
                                 OpBillRecNo = k.OpBillRecNo,
                                 RecDate = Convert.ToDateTime(k.RecDate).ToString("yyyy-MM-dd"),
                                 RecAmount = k.RecAmount,
                                 ConcAuthorizedBy = k.ConcAuthorizedBy,
                                 DueAmount = k.DueAmount,
                                 DueAuthorizedBy = k.DueAuthorizedBy,
                                 PaymentType = k.PaymentType,
                                 TxnNo = k.TxnNo,
                                 BankId = k.BankId,
                                 PaymentStatus = k.PaymentStatus,
                                 DueAuthorizedId = m.UserName,
                                 BankName = o.BankName,
                                 Email = i.Email,
                                 ContactNo = i.ContactNo,
                                 Relation = h.Relation,
                                 RelationName = h.RelationName,
                                 Dob = Convert.ToDateTime(i.DOB).ToString("yyyy-MM-dd"),
                                 Age = h.Age,
                                 Gender = i.Gender,
                                 OccupationId = h.OccupationId,
                                 RefSource = h.Refferal,
                                 RefBy = h.ReferredBy.ToString(),
                                 BloodGroup = h.BloodGroup,
                                 Nationality = h.Nationality,
                                 q.DoctorName

                             }).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.BillNo.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "OpBilling"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                return new Ret();
            }
        }

        public Ret GetInvestigation(OPBillingFilter entity)
        {
            try
            {
                List<ServiceCharges> _lServiceCharges = new List<ServiceCharges>();
                var patientData = (from a in db.Patient
                                   join opcon in db.OpConsultation on a.PatientId equals opcon.PatientId
                                   join b in db.Users on a.UserId equals b.Id
                                   join cu in db.Users on a.CreatedBy equals cu.Id
                                   join c in db.PatientAddress on a.PatientId equals c.PatientId into _patientAddress
                                   from address in _patientAddress.DefaultIfEmpty()
                                   join d in db.PatientReceiptDetails.Where(rec => rec.RecType == "Registration") on a.PatientId equals d.PCId into _patientReceiptDetails
                                   from receipt in _patientReceiptDetails.DefaultIfEmpty()
                                   join pincodeData in db.PincodeData on address.AreaCode equals pincodeData.Id into _pincodeData
                                   from e in _pincodeData.DefaultIfEmpty()
                                   join district in db.District on e.DistrictId equals district.Id into _district
                                   from f in _district.DefaultIfEmpty()
                                   join state in db.States on f.StateId equals state.Id into _states
                                   from g in _states.DefaultIfEmpty()
                                   join h in db.Doctors on a.DoctorId equals h.DoctorId into doctors
                                   from i in doctors.DefaultIfEmpty()
                                   join j in db.Department on i.DepartmentId equals j.DepartmentId into departments
                                   from k in departments.DefaultIfEmpty()
                                   join l in db.TitleMaster on b.TitleId equals l.TitleId into titles
                                   from m in titles.DefaultIfEmpty()
                                   join n in db.CorporateRegistration on a.PatientId equals n.PatientId into corporatepatients
                                   from o in corporatepatients.DefaultIfEmpty()
                                   join p in db.Countries on a.PCountryId equals p.CountryId into pcountries
                                   from q in pcountries.DefaultIfEmpty()
                                   join r in db.Countries on a.VCountryId equals r.CountryId into vcountries
                                   from s in vcountries.DefaultIfEmpty()
                                   where (opcon.ConsultationNo == entity.ConsultationNo)
                                   select new
                                   {
                                       a.PatientId,
                                       a.PatientAdmissionType,
                                       a.Vip,
                                       a.NewBorn,
                                       a.PatientType,
                                       a.Age,
                                       a.MaritalStatus,
                                       a.Relation,
                                       a.RelationName,
                                       a.Nationality,
                                       a.BloodGroup,
                                       a.OccupationId,
                                       a.Refferal,
                                       a.VipSour,
                                       a.Remarks,
                                       a.CreatedBy,
                                       a.CreatedDate,
                                       a.UpdatedDate,
                                       a.ReferredBy,
                                       a.UserId,
                                       PatientName = b.UserFullName,
                                       b.Id,
                                       UMRNumber = b.UserName,
                                       b.ContactNo,
                                       b.Email,
                                       b.Password,
                                       b.Status,
                                       o.OrganizationId,
                                       b.UserProfileId,
                                       b.RoleId,
                                       b.HospitalId,
                                       b.TitleId,
                                       b.Gender,
                                       b.IsMedicalTestRequired,
                                       DOB = Convert.ToDateTime(b.DOB).ToString("yyyy-MM-dd"),
                                       a.PrimarycontactNo,
                                       ReceiptId = receipt.ReceiptId == null ? 0 : receipt.ReceiptId,
                                       receipt.PCId,
                                       receipt.ReceiptNumber,
                                       receipt.RegFee,
                                       receipt.RecType,
                                       receipt.BankId,
                                       Validity = Convert.ToDateTime(receipt.Validity).Date.ToString("yyyy-MM-dd"),
                                       receipt.RecDate,
                                       receipt.PaymentType,
                                       receipt.TxnNo,
                                       receipt.RecRemarks,
                                       PAddressId = address.PAddressId == null ? 0 : address.PAddressId,
                                       address.AreaCode,
                                       address.Address,
                                       address.IdProof,
                                       address.IdNumber,
                                       address.GovtId1,
                                       address.GovtIdNumber1,
                                       address.GovtId2,
                                       address.GovtIdNumber2,
                                       address.Country,
                                       address.Area,
                                       e.DistrictId,
                                       f.DistrictName,
                                       g.StateName,
                                       PinCode = e.Pincode,
                                       k.DepartmentName,
                                       a.DoctorId,
                                       DoctorName = (i.Title ?? "" + i.DoctorName).Trim(),
                                       CreatedUserName = cu.UserFullName,
                                       m.Title,
                                       a.PassportNo,
                                       a.PCountryId,
                                       PIssueDate = Convert.ToDateTime(a.PIssueDate).ToString("yyyy-MM-dd"),
                                       PExpiryDate = Convert.ToDateTime(a.PExpiryDate).ToString("yyyy-MM-dd"),
                                       a.VisaNo,
                                       a.VCountryId,
                                       VIssueDate = Convert.ToDateTime(a.VIssueDate).ToString("yyyy-MM-dd"),
                                       VExpiryDate = Convert.ToDateTime(a.VExpiryDate).ToString("yyyy-MM-dd"),
                                       a.PatientProfile,
                                       PCountryName = q.CountryName,
                                       VCountryName = s.CountryName
                                   }).AsNoTracking().FirstOrDefault();

                var _investigations = (from _docp in db.DoctorPrescription
                                       join opcon in db.OpConsultation on _docp.ConsultationId equals opcon.ConsultationId
                                       where opcon.ConsultationNo == entity.ConsultationNo
                                       select new { _docp.Investigations }).AsNoTracking().FirstOrDefault();

                if (_investigations != null && _investigations.Investigations != null)
                {
                    List<Investigations> _eInvestigations = JsonConvert.DeserializeObject<List<Investigations>>(_investigations.Investigations.ToString());

                    if (entity.OrganizationId > 0)
                    {
                        var OpBillingService = db.OrganizationTariff.Where(org => org.OrganizationId == entity.OrganizationId).FirstOrDefault();
                        if (OpBillingService.OpPriorityDefault == null)
                        {
                            return new Ret { status = false, message = "Default tariff is not assigned to the selected organization, please assign it before proceeding" };
                        }

                        var priorities = new[]
                         {
                    OpBillingService.OpPriorityOne,
                    OpBillingService.OpPriorityTwo,
                    OpBillingService.OpPriorityThree,
                    OpBillingService.OpPriorityDefault

                };
                        foreach (var priority in priorities)
                        {

                            foreach (var tests in _eInvestigations)
                            {
                                var _serviceAmount = (from a in db.TariffServiceMapping
                                                      where a.TariffId == priority && a.ServiceId == tests.LabTestId
                                                      select new ServiceCharges
                                                      {
                                                          Value = a.Id,
                                                          Label = a.TariffServiceName,
                                                          ServiceCode = a.TariffServiceCode,
                                                          Rate = Convert.ToDouble(a.Charge),
                                                          IsGeneral = priority == 1 ? "Yes" : "No"
                                                      }).AsNoTracking().FirstOrDefault();

                                if (_serviceAmount != null)
                                {
                                    _lServiceCharges.Add(_serviceAmount);
                                }
                            }

                        }

                    }
                    else
                    {
                        foreach (var tests in _eInvestigations)

                        {
                            var _serviceAmount = (from a in db.TariffServiceMapping
                                                  where a.TariffId == 1 && a.ServiceId == tests.LabTestId
                                                  select new ServiceCharges
                                                  {
                                                      Value = a.Id,
                                                      Label = a.TariffServiceName,
                                                      ServiceCode = a.TariffServiceCode,
                                                      Rate = Convert.ToDouble(a.Charge),
                                                      IsGeneral = "Yes"
                                                  }).AsNoTracking().FirstOrDefault();

                            if (_serviceAmount != null)
                            {
                                _lServiceCharges.Add(_serviceAmount);
                            }
                        }

                    }
                }
                var finalResult = new
                {
                    patientData.PatientId,
                    patientData.PatientAdmissionType,
                    patientData.Vip,
                    patientData.NewBorn,
                    patientData.PatientType,
                    patientData.Age,
                    patientData.MaritalStatus,
                    patientData.Relation,
                    patientData.RelationName,
                    patientData.Nationality,
                    patientData.BloodGroup,
                    patientData.OccupationId,
                    patientData.Refferal,
                    patientData.VipSour,
                    patientData.Remarks,
                    patientData.CreatedBy,
                    patientData.CreatedDate,
                    patientData.UpdatedDate,
                    patientData.ReferredBy,
                    patientData.UserId,
                    patientData.PatientName,
                    patientData.Id,
                    patientData.UMRNumber,
                    patientData.ContactNo,
                    patientData.Email,
                    patientData.Password,
                    patientData.Status,
                    patientData.OrganizationId,
                    patientData.UserProfileId,
                    patientData.RoleId,
                    patientData.HospitalId,
                    patientData.TitleId,
                    patientData.Gender,
                    patientData.IsMedicalTestRequired,
                    patientData.DOB,
                    patientData.PrimarycontactNo,
                    patientData.ReceiptId,
                    patientData.PCId,
                    patientData.ReceiptNumber,
                    patientData.RegFee,
                    patientData.RecType,
                    patientData.BankId,
                    patientData.Validity,
                    patientData.RecDate,
                    patientData.PaymentType,
                    patientData.TxnNo,
                    patientData.RecRemarks,
                    patientData.PAddressId,
                    patientData.AreaCode,
                    patientData.Address,
                    patientData.IdProof,
                    patientData.IdNumber,
                    patientData.GovtId1,
                    patientData.GovtIdNumber1,
                    patientData.GovtId2,
                    patientData.GovtIdNumber2,
                    patientData.Country,
                    patientData.Area,
                    patientData.DistrictId,
                    patientData.DistrictName,
                    patientData.StateName,
                    patientData.PinCode,
                    patientData.DepartmentName,
                    patientData.DoctorId,
                    patientData.DoctorName,
                    patientData.CreatedUserName,
                    patientData.Title,
                    patientData.PassportNo,
                    patientData.PCountryId,
                    patientData.PIssueDate,
                    patientData.PExpiryDate,
                    patientData.VisaNo,
                    patientData.VCountryId,
                    patientData.VIssueDate,
                    patientData.VExpiryDate,
                    patientData.PatientProfile,
                    patientData.PCountryName,
                    patientData.VCountryName,
                    Services = _lServiceCharges != null ? _lServiceCharges.Select(ls => ls) : []
                };
                return new Ret { status = true, data = finalResult };


            }
            catch (Exception ex)
            {
                return new Ret { status = false };
            }
        }
        public Ret GetOpBillingByRef(string RefNo, JwtStatus jwtData)
        {
            try
            {
                var _patientData = (from _col in db.CoLetterDetails
                                    join _cop in db.CorporateRegistration on _col.CoRegId equals _cop.CoRegId
                                    join _org in db.Organization on _cop.OrganizationId equals _org.OrganizationId
                                    join _patient in db.Patient on _cop.PatientId equals _patient.PatientId
                                    join _user in db.Users on _patient.UserId equals _user.Id
                                    where _col.RefNo == RefNo
                                    select new
                                    {
                                        RefNo,
                                        PatientName = _user.UserFullName,
                                        Age = _patient.Age,
                                        _user.Gender,
                                        _user.ContactNo,
                                        UMRNo = _user.UserName,
                                        _user.DOB,
                                        _patient.PatientType,
                                        _org.OrganizationName,
                                        _org.OrganizationType,
                                        OrgContactNo = _org.ContactNo,
                                        _org.ContactPerson,
                                        _org.GSTCode,
                                        _org.OpOrgPercentage,
                                        _org.OpEmpPercentage,
                                        _cop.EmpName,
                                        _cop.EmpNo,
                                        _cop.MedicalCardNo,
                                        _col.RefLetterNo,
                                        _col.LetterFileName
                                    }).FirstOrDefault();

                var _consultationData = (from opCon in db.OpConsultation
                                         where opCon.RefNo == RefNo && opCon.Status != "Cancelled"
                                         select new
                                         {
                                             BillAmount = opCon.ConsultantFee,
                                             BillDate = Convert.ToDateTime(opCon.ConsultationDate).ToString("dd-MM-yyyy"),
                                             OpBillRecNo = opCon.ConsultationNo,
                                             _patientData.EmpNo,
                                             _patientData.EmpName,
                                             BillName = "Consultation",
                                             opCon.ConsultationId

                                         }).ToList();

                var _billingDetails = (from _billing in db.OPBilling
                                       join _opbillR in db.OPBillReceipt on _billing.OPBillId equals _opbillR.OpBillId
                                       join _doctor in db.Doctors on _billing.ConsultantId equals _doctor.DoctorId into doctors
                                       from doctor in doctors.DefaultIfEmpty()
                                       where _billing.RefNo == RefNo
                                       select new
                                       {
                                           BillName = "Investigations",
                                           _billing.OPBillId,
                                           _billing.RefNo,
                                           _billing.OverAllConcAmount,
                                           _billing.GrossAmount,
                                           _billing.NetAmount,
                                           OpBillRecNo = _opbillR.OpBillRecNo,
                                           BillDate = Convert.ToDateTime(_opbillR.RecDate).ToString("dd-MM-yyyy"),
                                           BillNo = _opbillR.OpBillRecNo,
                                           BillAmount = _opbillR.RecAmount,
                                           DoctorName = doctor.DoctorName,
                                           _patientData.EmpNo,
                                           _patientData.EmpName,
                                           Services = (from a in db.OPServiceBooking
                                                       join s in db.TariffServiceMapping on a.ServiceId equals s.Id
                                                       where a.OpBillId == _billing.OPBillId
                                                       select new
                                                       {
                                                           s.TariffServiceCode,
                                                           s.TariffServiceName,
                                                           a.Rate,
                                                           a.Qty,
                                                           a.TotalAmount
                                                       }
                                           ).ToList()
                                       }).AsEnumerable()
                        .ToList();
                #region comment
                //var billingInfo = (from _billing in db.OPBilling
                //                   join _org in db.Organization on _billing.OrganizationId equals _org.OrganizationId
                //                   join _cop in db.CorporateRegistration on _billing.PatientId equals _cop.PatientId
                //                   join _col in db.CoLetterDetails on _billing.RefNo equals _col.RefNo
                //                   join _patient in db.Patient on _billing.PatientId equals _patient.PatientId
                //                   join _user in db.Users on _patient.UserId equals _user.Id
                //                   join _opcon in db.OpConsultation on _billing.RefNo equals _opcon.RefNo into opConsultations
                //                   from opcon in opConsultations.DefaultIfEmpty()
                //                   join _opr in db.PatientReceiptDetails.Where(a => a.RecType == "Consultation")
                //                       on opcon.ConsultationId equals _opr.PCId into Opr
                //                   from opr in Opr.DefaultIfEmpty()
                //                   join _doctor in db.Doctors on _billing.ConsultantId equals _doctor.DoctorId into doctors
                //                   from doctor in doctors.DefaultIfEmpty()
                //                   where _billing.RefNo == RefNo && _billing.IsCancelled == "No" && _billing.BillType == "Corporate"
                //                   && _billing.OrganizationId == _cop.OrganizationId
                //                   select new
                //                   {
                //                       _billing.OPBillId,
                //                       _billing.RefNo,
                //                       opcon.ConsultantFee,
                //                       opcon.ConsultationDate,
                //                       opcon.ConsultationNo,
                //                       PatientName = _user.UserFullName,
                //                       Age = _patient.Age,
                //                       _user.Gender,
                //                       _user.ContactNo,
                //                       UMRNo = _user.UserName,
                //                       _user.DOB,
                //                       _patient.PatientType,
                //                       DoctorName = doctor.DoctorName,
                //                       doctor.Qualification,
                //                       _org.OrganizationName,
                //                       _org.OrganizationType,
                //                       OrgContactNo = _org.ContactNo,
                //                       _org.ContactPerson,
                //                       _org.GSTCode,
                //                       _org.OpOrgPercentage,
                //                       _org.OpEmpPercentage,
                //                       _billing.OverAllConcAmount,
                //                       _billing.GrossAmount,
                //                       _billing.NetAmount,
                //                       _cop.EmpName,
                //                       _cop.EmpNo,
                //                       _cop.Department,
                //                       _cop.Designation,
                //                       _cop.MedicalCardNo,
                //                       _col.RefLetterNo
                //                   }).FirstOrDefault();

                #endregion

                var finalResult = new
                {
                    _patientData.RefNo,
                    _patientData.PatientName,
                    _patientData.Age,
                    _patientData.Gender,
                    _patientData.ContactNo,
                    _patientData.UMRNo,
                    _patientData.DOB,
                    _patientData.PatientType,
                    _patientData.OrganizationName,
                    _patientData.OrganizationType,
                    _patientData.OrgContactNo,
                    _patientData.ContactPerson,
                    _patientData.GSTCode,
                    _patientData.OpOrgPercentage,
                    _patientData.OpEmpPercentage,
                    _patientData.LetterFileName,
                    BillingDetails = _consultationData.Cast<object>().Concat(_billingDetails.Cast<object>()).ToList()

                };
                return new Ret { status = true, message = FetchMessage(finalResult, "OpBilling"), data = finalResult };
            }
            catch (Exception ex)
            {
                return new Ret();
            }
        }

    }
}
