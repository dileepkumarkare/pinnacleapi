using Pinnacle.Entities;
using Pinnacle.Helpers;
using Pinnacle.Helpers.JWT;
using Pinnacle.Reports;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Pinnacle.Models
{
    public class ReportModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();
        public Ret GenerateOpConsultationReceipt(int ConsultationId)
        {
            try
            {
                OpConsultationReceipt opConsultationReceipt = new OpConsultationReceipt();
                string base64String = string.Empty;
                if (opConsultationReceipt.Parameters.Count > 0)
                {
                    opConsultationReceipt.Parameters["P_ConsultationId"].Value = ConsultationId;
                    using (var ms = new MemoryStream())
                    {
                        opConsultationReceipt.ExportToPdf(ms);
                        ms.Position = 0;
                        byte[] pdfBytes = ms.ToArray();
                        base64String = Convert.ToBase64String(pdfBytes);
                    }
                }
                return new Ret { status = true, message = "Receipt generated successfully!", data = base64String };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong" };

            }
        }
        public Ret GeneratePrescription(int ConsultationId)
        {
            try
            {
                Reports.DoctorPrescription dp = new Reports.DoctorPrescription();
                string base64String = string.Empty;
                string rootFolder = Path.GetFullPath("Uploads/Reports/Prescription_" + ConsultationId + ".pdf");
                if (File.Exists(rootFolder))
                {
                    File.Delete(rootFolder);
                }
                if (dp.Parameters.Count > 0)
                {
                    dp.Parameters["PConsultationId"].Value = ConsultationId;
                    using (var ms = new MemoryStream())
                    {
                        dp.ExportToPdf(ms);
                        ms.Position = 0;
                        byte[] pdfBytes = ms.ToArray();
                        string folderPath = "Uploads/Reports";
                        string filePath = Path.Combine(folderPath, "Prescription_" + ConsultationId + ".pdf");
                        System.IO.File.WriteAllBytes(filePath, ms.ToArray());
                        // base64String = Convert.ToBase64String(pdfBytes);
                    }
                }
                return new Ret { status = true, message = "Receipt generated successfully!", data = null };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong" };

            }
        }
        public Ret GenerateOpConsultationBillingReport(OPConsultationBillingFilter entity, JwtStatus jwtData)
        {
            try
            {
                DBHelper dbHelper = new DBHelper(db.GetConnectionString());
                SqlParameter[] objParams = new SqlParameter[3];
                objParams[0] = new SqlParameter("@HospitalId", jwtData.HospitalId);
                objParams[1] = new SqlParameter("@FromDate", entity.FromDate);
                objParams[2] = new SqlParameter("@ToDate", entity.ToDate);
                DataSet ds = dbHelper.ExecuteDataSetSP("PKG_GET_OP_CONSULTATION_REPORT", objParams);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return new Ret { status = true, data = CommonLogic.GetJsonObject(ds), message = "Data loaded successfully" };
                }
                return new Ret { status = true, message = "Something went wrong" };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong" };
            }
        }
        public Ret GetDashboardOpConsCount(DashboardEntity entity, JwtStatus jwtData)
        {
            try
            {
                DBHelper dbHelper = new DBHelper(db.GetConnectionString());
                SqlParameter[] objParams = new SqlParameter[5];
                objParams[0] = new SqlParameter("@HOSPITALID", jwtData.HospitalId);
                objParams[1] = new SqlParameter("@DATE", entity.Date);
                objParams[2] = new SqlParameter("@MONTH", entity.Month);
                objParams[3] = new SqlParameter("@YEAR", entity.Year);
                objParams[4] = new SqlParameter("@FILTERTYPE", entity.FilterType);
                DataSet ds = dbHelper.ExecuteDataSetSP("PKG_GET_OPCONS_COUNT", objParams);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return new Ret { status = true, data = CommonLogic.GetJsonArray(ds), message = "Data loaded successfully" };
                }
                return new Ret { status = true, message = "Something went wrong" };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong" };
            }
        }
        public Ret OpBillingReport(OPConsultationBillingFilter entity, JwtStatus jwtData)
        {
            try
            {
                DBHelper dbHelper = new DBHelper(db.GetConnectionString());
                SqlParameter[] objParams = new SqlParameter[3];
                objParams[0] = new SqlParameter("@HospitalId", jwtData.HospitalId);
                objParams[1] = new SqlParameter("@FromDate", entity.FromDate);
                objParams[2] = new SqlParameter("@ToDate", entity.ToDate);
                DataSet ds = dbHelper.ExecuteDataSetSP("PKG_GET_OPBILLING_REPORT", objParams);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return new Ret { status = true, data = CommonLogic.GetJsonObject(ds), message = "Data loaded successfully" };
                }
                return new Ret { status = true, message = "Something went wrong" };
            }
            catch (Exception ex)
            {
                return new Ret { status = false, message = "Something went wrong" };
            }
        }

    }
}
