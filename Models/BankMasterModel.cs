using DevExpress.XtraRichEdit.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Serilog;

namespace Pinnacle.Models
{
    public class BankMasterModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();

        public Ret GetAllBankLabel(int Id, JwtStatus jwtData)
        {
            try
            {
                var _bankList = (from _bank in db.BankMaster
                                 where _bank.HospitalId == jwtData.HospitalId &&
                                 (Id == 0 || _bank.Id == Id) && _bank.IsActive == "Yes"
                                 orderby _bank.BankName
                                 select new
                                 {
                                     value = _bank.Id,
                                     label = _bank.BankCode + " - " + _bank.BankName,
                                     _bank.BranchName,
                                     _bank.IFSCCode,
                                     _bank.MICRCode,
                                     _bank.Address,
                                     _bank.BSRCode,
                                     _bank.IsRequiredTrans
                                 }).AsNoTracking().ToList();

                return new Ret { status = true, message = FetchMessage(_bankList, "Bank List"), data = _bankList };
            }
            catch (Exception ex)
            {
                Log.Information("Error at " + DateTime.Now.ToString() + " message " + ex.Message);
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret GetAllBanksList(Pagination entity, JwtStatus jwtData)
        {
            try
            {
                var query = (from _bank in db.BankMaster
                             join _user in db.Users on _bank.CreatedBy equals _user.Id
                             where _bank.HospitalId == jwtData.HospitalId &&
                             (entity.Id == 0 || _bank.Id == entity.Id) && _bank.IsActive == "Yes"
                             orderby _bank.Id descending
                             select new
                             {
                                 _bank.Id,
                                 _bank.BankCode,
                                 _bank.BankName,
                                 _bank.BranchName,
                                 _bank.HospitalId,
                                 _bank.IFSCCode,
                                 _bank.MICRCode,
                                 _bank.BSRCode,
                                 _bank.Address,
                                 _bank.IsRequiredTrans,
                                 _bank.IsActive,
                                 _bank.CreatedBy,
                                 CreatedDate = Convert.ToDateTime(_bank.CreatedDate).ToString("yyyy-MM-dd"),
                                 _bank.ModifyBy,
                                 ModifyDate = Convert.ToDateTime(_bank.ModifyDate).ToString("yyyy-MM-dd"),
                                 CreatedById = _user.UserName
                             }).AsNoTracking();

                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(a => a.BankCode.Contains(entity.SearchKey) || a.BankName.Contains(entity.SearchKey) || a.BranchName.Contains(entity.SearchKey));

                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Bank Master"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information("Error at " + DateTime.Now.ToString() + " message " + ex.Message);
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret SaveBankMaster(BankMaster entity, JwtStatus jwtData)
        {
            try
            {
                if (entity.Id == 0)
                {
                    string bankCode = "BNK1";
                    var _lastBnkCode = db.BankMaster.Where(bnk => bnk.BankCode.StartsWith("BNK")).OrderByDescending(bnk => bnk).Select(bnk => bnk.BankCode).FirstOrDefault();
                    if (!string.IsNullOrEmpty(_lastBnkCode))
                    {
                        var newNumber = _lastBnkCode.Substring(3);
                        bankCode = int.TryParse(newNumber, out int lastNumber) ? $"BNK{lastNumber + 1}" : bankCode;
                    }
                    entity.CreatedBy = jwtData.Id;
                    entity.HospitalId = jwtData.HospitalId;
                    entity.BankCode = bankCode;
                    db.BankMaster.Add(entity);
                    db.SaveChanges();
                    return new Ret { status = true, message = "Bank details saved successfully." };
                }
                else
                {
                    var _existingDetails = db.BankMaster.Where(bnk => bnk.Id == entity.Id).FirstOrDefault();
                    if (_existingDetails != null)
                    {
                        _existingDetails.BankName = entity.BankName;
                        _existingDetails.BranchName = entity.BranchName;
                        _existingDetails.IFSCCode = entity.IFSCCode;
                        _existingDetails.MICRCode = entity.MICRCode;
                        _existingDetails.BSRCode = entity.BSRCode;
                        _existingDetails.Address = entity.Address;
                        _existingDetails.IsRequiredTrans = entity.IsRequiredTrans;
                        _existingDetails.IsActive = entity.IsActive;
                        _existingDetails.ModifyBy = jwtData.Id;
                        _existingDetails.ModifyDate = DateTime.UtcNow;
                        db.BankMaster.Update(_existingDetails);
                        db.SaveChanges();
                        return new Ret { status = true, message = "Bank details updated successfully." };
                    }
                    return new Ret { status = false, message = "Failed update the bank details." };
                }

            }
            catch (Exception ex)
            {
                Log.Information("Bank Master Controller=> Save Bank Master error at " + DateTime.Now.ToString() + " message " + ex.Message);
                return new Ret { status = false, message = "Something went wrong." };
            }
        }
    }
}
