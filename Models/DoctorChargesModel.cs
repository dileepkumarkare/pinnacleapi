using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Serilog;
using System.Diagnostics.Eventing.Reader;
using System.Numerics;

namespace Pinnacle.Models
{
    public class DoctorChargesModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();
        OrganizationModel orgModel = new OrganizationModel();

        public Ret GetDoctorCharges(Pagination entity)
        {

            try
            {
                var query = db.DoctorCharges.Where(a => a.DoctorId == entity.Id).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.VisitType.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Doctor charges"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }

        public Ret GetConsultationFee(DoctorCharges entity, JwtStatus jwtData)
        {

            try
            {
                var _visitDays = db.Hospital.Where(h => h.HospitalId == jwtData.HospitalId).Select(h => h.Visits).FirstOrDefault();
                if (entity.PaymentBy == "Self")
                {
                    var res = db.DoctorCharges.Where(a => a.DoctorId == entity.DoctorId && (a.EffectFrom <= entity.ConsultationDate && a.EffectTo >= entity.ConsultationDate || a.EffectFrom <= entity.ConsultationDate && string.IsNullOrEmpty(a.EffectTo.ToString())))
                                              .Select(charge => charge.Charge).FirstOrDefault();
                    return new Ret { status = true, message = FetchMessage(res, "Doctor charges"), data = new { doctorList = new[] { new { consultationFee = entity.Visit <= Convert.ToInt32(_visitDays) && entity.Visit > 1 ? 0 : res } } } };
                }
                else
                {
                    var onlyId = new OnlyId { Id = Convert.ToInt32(entity.OrganizationId) };
                    Ret _doctorList = orgModel.GetDoctors(onlyId, Convert.ToInt32(entity.Visit), Convert.ToInt32(_visitDays));
                    return new Ret { status = true, message = FetchMessage(_doctorList, "Doctor charges"), data = new { doctorList = _doctorList.data } };
                }

            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }

        public Ret SaveDoctorCharges(DoctorCharges entity, JwtStatus jwtData)
        {
            try
            {
                entity.CreatedBy = jwtData.Id;

                string msg;
                var _existing = db.DoctorCharges.Any(charge => charge.DoctorId == entity.DoctorId &&
                                                               charge.EffectFrom >= entity.EffectFrom &&
                                                               charge.EffectTo <= entity.EffectTo &&
                                                               charge.VisitType == entity.VisitType);

                if (!_existing && entity.ChargeId == 0)
                {
                    db.DoctorCharges.Add(entity);
                    msg = "Doctor charges saved successfully!";
                }
                else if (entity.ChargeId > 0)
                {

                    var existingCharges = db.DoctorCharges.AsNoTracking().FirstOrDefault(x => x.ChargeId == entity.ChargeId);
                    if (existingCharges != null)
                    {
                        existingCharges.Charge = entity.Charge;
                        existingCharges.EffectFrom = entity.EffectFrom;
                        existingCharges.EffectTo = entity.EffectTo;
                        db.DoctorCharges.Update(existingCharges);
                        msg = "Doctor charges updated successfully!";
                    }
                    else
                    {
                        msg = "Failed to save doctor charges!";
                    }
                }
                else
                {
                    msg = "Charges already exist for the selected visit type and dates!";
                }
                db.SaveChanges();
                return new Ret { status = true, message = msg };
            }

            catch (Exception ex)
            {
                return new Ret { status = false, message = "Failed to save doctor charges." };
            }
        }
    }
}
