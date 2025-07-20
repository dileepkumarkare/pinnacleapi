using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Serilog;

namespace Pinnacle.Models
{
    public class ReferralModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();

        public Ret SaveReferral(ReferralEntity referralEntity, JwtStatus jwtData)
        {
            try
            {
                referralEntity.CreatedBy = jwtData.Id;
                referralEntity.HospitalId = jwtData.HospitalId;
                string msg;

                if (referralEntity.Id == 0)
                {
                    // Generate Reference Code
                    var lastReferral = db.ReferralMaster
                        .Where(x => x.ReferenceCode.StartsWith("REF"))
                        .OrderByDescending(x => x.ReferenceCode)
                        .Select(x => x.ReferenceCode)
                        .FirstOrDefault();

                    string newReferenceCode = "REF001";
                    if (!string.IsNullOrEmpty(lastReferral) && lastReferral.Length > 3)
                    {
                        if (int.TryParse(lastReferral.Substring(3), out int lastNumber))
                        {
                            newReferenceCode = $"REF{(lastNumber + 1):D3}";
                        }
                    }

                    referralEntity.ReferenceCode = newReferenceCode;

                    db.ReferralMaster.Add(referralEntity);
                    db.SaveChanges();
                    msg = "Referral saved successfully!";
                }
                else
                {
                    var existingReferral = db.ReferralMaster
                        .FirstOrDefault(x => x.Id == referralEntity.Id);

                    if (existingReferral != null)
                    {
                        existingReferral.ReferenceName = referralEntity.ReferenceName;
                        existingReferral.AliasName = referralEntity.AliasName;
                        existingReferral.Specialization = referralEntity.Specialization;
                        existingReferral.Designation = referralEntity.Designation;
                        existingReferral.DOB = referralEntity.DOB;
                        existingReferral.MarriageDate = referralEntity.MarriageDate;
                        existingReferral.PROCode = referralEntity.PROCode;
                        existingReferral.AreaCode = referralEntity.AreaCode;
                        existingReferral.ModifyBy = jwtData.Id;
                        existingReferral.ModifyDate = DateTime.Now;
                        existingReferral.ReferralType = referralEntity.ReferralType;
                        db.ReferralMaster.Update(existingReferral);
                        db.SaveChanges();

                        msg = "Referral updated successfully!";
                    }
                    else
                    {
                        return new Ret { status = false, message = "Referral not found." };
                    }
                }

                return new Ret { status = true, message = msg, data = new { id = referralEntity.Id } };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Failed to save Referral. Error: " + ex.Message };
            }
        }

        public Ret GetAllReferrals(Pagination entity)
        {

            try
            {
                var query = (from a in db.ReferralMaster.Where(a => (entity.Id == 0 || a.Id == entity.Id))
                             join b in db.Users.AsEnumerable() on a.CreatedBy equals b.Id into ReferralUsers
                             from c in ReferralUsers.DefaultIfEmpty()
                             select new
                             {
                                 a.Id,
                                 a.ReferralType,
                                 a.ReferenceCode,
                                 a.ReferenceName,
                                 a.AliasName,
                                 a.HospitalId,
                                 a.Specialization,
                                 a.Designation,
                                 a.DOB,
                                 a.MarriageDate,
                                 a.PROCode,
                                 a.AreaCode,
                                 a.ActiveDate,
                                 a.DeactiveDate,
                                 a.IsPaymentRequired,
                                 a.IsActive,
                                 a.CreatedBy,
                                 a.CreatedDate,
                                 a.ModifyBy,
                                 a.ModifyDate,
                                 a.ActivateBy,
                                 a.ActivateDate,
                                 a.DeactivateBy,
                                 a.DeactivatedDate,
                                 CreatedByName = c.UserId,
                                 ModifyByName = string.Empty,
                                 VerifyByName = string.Empty,
                                 CancelledByName = string.Empty

                             }).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.ReferenceName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Referral"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }

        public Ret SaveReferralPercentage(ReferralPercentage referralPercentage, JwtStatus jwtData)
        {
            try
            {
                string msg;

                if (referralPercentage.Id == 0)
                {
                    db.ReferralPercentage.Add(referralPercentage);
                    db.SaveChanges();
                    msg = "Referral percentage saved successfully!";
                }
                else
                {
                    var existingReferral = db.ReferralPercentage
                        .FirstOrDefault(x => x.Id == referralPercentage.Id);

                    if (existingReferral != null)
                    {
                        if (referralPercentage != null)
                        {
                            existingReferral.InPatient = referralPercentage.InPatient;
                            existingReferral.Investigations = referralPercentage.Investigations;
                            existingReferral.OpConsultations = referralPercentage.OpConsultations;
                            existingReferral.PAN = referralPercentage.PAN;
                            existingReferral.ACNo = referralPercentage.ACNo;
                            db.ReferralPercentage.Update(existingReferral);
                            db.SaveChanges();
                        }

                        msg = "Referral percentage updated successfully!";
                    }
                    else
                    {
                        return new Ret { status = false, message = "Referral not found." };
                    }
                }

                return new Ret { status = true, message = msg };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Failed to save Referral. Error: " + ex.Message };
            }
        }
        public Ret GetReferralPercentage(OnlyId objId)
        {
            try
            {
                var referralPercentage = db.ReferralPercentage.Where(x => x.ReferralId == objId.Id).AsNoTracking().ToList();
                if (referralPercentage != null)
                {
                    return new Ret { status = true, message = FetchMessage(referralPercentage, "Referral percentage"), data = referralPercentage };
                }
                else
                {
                    return new Ret { status = false, message = NoDataMessage() };
                }

            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = NoDataMessage() };
            }


        }

        public Ret GetAllReferralLabel(int Id)
        {
            try
            {
                var res = db.ReferralMaster.Where(a => Id == 0 || a.Id == Id).Select(a => new { value = a.Id, label = a.ReferenceName + " - " + a.ReferenceCode, a.ReferralType }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Department"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
    }
}

