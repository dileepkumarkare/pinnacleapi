using Pinnacle.Entities;
using Serilog;
using Pinnacle.Helpers.JWT;
using Microsoft.EntityFrameworkCore;
using System.Data;
using ExcelDataReader;
using System.IO;
using System.Reflection.PortableExecutable;
namespace Pinnacle.Models
{
    public class DesignationModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();


        public Ret SaveDesignation(DesignationEntity entity, JwtStatus jwtData)
        {
            try
            {
                entity.AddedBy = jwtData.Id;
                entity.HospitalId = jwtData.HospitalId;
                string msg;

                if (entity.DesignationId == 0)
                {
                    var lastDesignation = db.Designation.Where(x => x.DesignationCode.StartsWith("DES")).OrderByDescending(x => x.DesignationCode)
                        .Select(x => x.DesignationCode).FirstOrDefault();

                    string newDesignationCode = "DES001";

                    if (!string.IsNullOrEmpty(lastDesignation) && lastDesignation.Length > 3)
                    {
                        string numberPart = lastDesignation.Substring(3);
                        if (int.TryParse(numberPart, out int lastNumber))
                        {
                            newDesignationCode = $"DES{(lastNumber + 1):D3}";
                        }
                    }
                    entity.DesignationCode = newDesignationCode;
                    db.Designation.Add(entity);
                    msg = "Designation saved successfully!";
                }
                else
                {
                    var existingDesignation = db.Designation.FirstOrDefault(x => x.DesignationId == entity.DesignationId);
                    if (existingDesignation != null)
                    {
                        existingDesignation.UpdatedDate = DateTime.Now;
                        existingDesignation.UpdatedBy = jwtData.Id;
                        existingDesignation.DesignationName = entity.DesignationName;
                        msg = "Designation updated successfully!";
                    }
                    else
                    {
                        return new Ret { status = false, message = "Designation not found." };
                    }
                }
                db.SaveChanges();
                return new Ret { status = true, message = msg };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Failed to save designation." };
            }
        }

        public Ret DesignationFileImport(ImportFile file, JwtStatus jwt)
        {
            try
            {
                DataTable dt = new DataTable();
                using (var streamReader = new StreamReader(file.File.OpenReadStream()))
                {

                    string[] headers = streamReader.ReadLine()?.Split(',') ?? new string[0];
                    foreach (string header in headers)
                    {
                        dt.Columns.Add(header);
                    }
                    var lastCode = db.Designation.OrderByDescending(x => x.DesignationCode).Select(x => x.DesignationCode).FirstOrDefault();
                    int nextCode = (lastCode != null && lastCode.StartsWith("DES") && int.TryParse(lastCode.Substring(3), out int parsedCode))
                        ? parsedCode + 1
                        : 1;

                    List<DesignationEntity> validEntities = new List<DesignationEntity>();

                    while (!streamReader.EndOfStream)
                    {
                        string[] rows = streamReader.ReadLine()?.Split(',');

                        if (rows != null && rows.Length >= 1 && !string.IsNullOrEmpty(rows[0]?.Trim()))
                        {
                            string designationName = rows[0].Trim();

                            if (db.Designation.Any(x => x.DesignationName == designationName))
                            {
                                return new Ret { status = false, message = $"Designation Name '{designationName}' Already Exists." };
                            }

                            string formattedCode = $"DES{nextCode:D3}";
                            nextCode++;

                            DesignationEntity entity = new DesignationEntity
                            {
                                DesignationName = designationName,
                                DesignationCode = formattedCode,
                                AddedBy = jwt.Id,
                                HospitalId = jwt.HospitalId,
                                Status = "Active",
                                CreatedDate = DateTime.Now
                            };

                            validEntities.Add(entity);
                        }
                    }

                    if (validEntities.Any())
                    {
                        db.Designation.AddRange(validEntities);
                        db.SaveChanges();
                        return new Ret { status = true, message = "Upload successful!" };
                    }
                    else
                    {
                        return new Ret { status = false, message = "No valid data to import." };
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error at {DateTime.Now}: {ex.Message}");
                return new Ret { status = false, message = "Error occurred during upload. Please try again later." };
            }
        }

        public Ret SaveTitle(TitleEntity entity)
        {
            try
            {
                if (entity.TitleId == 0)
                {
                    db.TitleMaster.Add(entity);
                    db.SaveChanges();
                    return new Ret { status = true, message = "Title saved successfully!" };
                }
                else
                {
                    var existingEntity = db.TitleMaster.AsNoTracking().FirstOrDefault(h => h.TitleId == entity.TitleId);
                    if (existingEntity != null)
                    {
                        db.TitleMaster.Update(entity);
                        db.SaveChanges();
                        return new Ret { status = true, message = "Title updated successfully!" };
                    }
                    else
                    {
                        return new Ret { status = false, message = "Title record not found for update." };
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }

        public Ret GetAllTitles(Pagination entity)
        {

            try
            {
                var query = db.TitleMaster.Where(a => entity.Id == 0 || a.TitleId == entity.Id).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.Title.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "TitleMaster"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }
        public Ret GetAllTitle()
        {
            try
            {
                var res = db.TitleMaster.Select(a => new { value = a.TitleId, label = a.Title }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Title"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
        public Ret GetAllDesignations(Pagination entity)
        {

            try
            {
                var query = db.Designation.Where(a => entity.Id == 0 || a.DesignationId == entity.Id).AsNoTracking();
                if (!string.IsNullOrEmpty(entity.SearchKey)) query = query.Where(c => c.DesignationName.Contains(entity.SearchKey));
                var totalCount = query.Count();
                var res = PaginatedValues(query, entity);
                return new Ret { status = true, message = FetchMessage(res, "Designation"), data = res, totalCount = totalCount };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }

        }
        public Ret GetAllDesignationsLabel(int Id)
        {
            try
            {
                var res = db.Designation.Where(a => Id == 0 || a.DesignationId == Id).Select(a => new { value = a.DesignationId, label = a.DesignationName + " - " + a.DesignationCode }).AsNoTracking().ToList();
                return new Ret { status = true, message = FetchMessage(res, "Designation"), data = res };
            }
            catch (Exception ex)
            {
                Log.Information(" Error " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = FailedSaveMessage() };
            }
        }
    }


}
