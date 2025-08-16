using Microsoft.AspNetCore.Http.HttpResults;
using Pinnacle.Entities;
using Pinnacle.Helpers;
using Pinnacle.Helpers.JWT;
using Serilog;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json.Nodes;

namespace Pinnacle.Models
{
    public class SampleCollectionModel : MasterModel
    {
        PinnacleDbContext db = new PinnacleDbContext();

        public Ret SaveSampleCollection(SampleCollectionEntity entity, JwtStatus jwtData)
        {
            try
            {

                if (!db.SampleCollection.Any(sc => sc.BillNo == entity.BillNo))
                {
                    var _sc = new SampleCollectionEntity
                    {
                        PatientType = entity.PatientType,
                        BillNo = entity.BillNo,
                        NoOfBarcodes = entity.NoOfBarcodes,
                        IsUrgent = entity.IsUrgent,
                        TransNo = entity.TransNo,
                        TransDate = DateTime.UtcNow,
                        LabNo = entity.LabNo,
                        PhlebotomistId = entity.PhlebotomistId,
                        ClinicalHistory = entity.ClinicalHistory,
                        Remarks = entity.Remarks,
                        CreatedBy = jwtData.Id,
                        CreatedDate = DateTime.UtcNow,
                        BillId = entity.BillId
                    };
                    db.SampleCollection.Add(_sc);
                    db.SaveChanges();
                    entity.Id = _sc.Id;
                    SaveSampleCollectionTests(entity, jwtData);
                    return new Ret { status = true, message = "Sample collected and saved successfully." };
                }
                else
                {
                    return new Ret { status = true, message = "Sample collected and saved successfully." };
                }
            }
            catch (Exception ex)
            {
                Log.Information("Sample Collection Model => Save Sample Collection  exception at " + DateTime.UtcNow.ToString() + " message is : " + ex.Message);
                return new Ret { status = false, message = "Something went wrong" };
            }
        }
        public Ret SaveSampleCollectionTests(SampleCollectionEntity entity, JwtStatus jwtData)
        {
            try
            {
                foreach (var test in entity.SampleCollectionTests)
                {
                    if (!db.SampleCollectionTests.Any(sct => sct.SampleCollId == entity.Id && sct.TestId == test.TestId))
                    {
                        var _sct = new SampleCollectionTest
                        {

                            SampleCollId = entity.Id,
                            TestId = test.TestId,
                            IsCollected = test.IsCollected,
                            IsRejected = test.IsRejected,
                            IsRecollected = test.IsRecollected,
                            IsUrgent = test.IsUrgent,
                            IsPriority = test.IsPriority,
                            CreatedBy = jwtData.Id,
                            CreatedDate = DateTime.UtcNow,
                            Remarks = test.Remarks
                        };
                        db.SampleCollectionTests.Add(_sct);
                        db.SaveChanges();
                    }
                    else
                    {
                        SampleCollectionTest _existsSampleTest = db.SampleCollectionTests.Where(sct => sct.TestId == test.TestId && sct.SampleCollId == test.SampleCollId).FirstOrDefault();
                        if (_existsSampleTest is SampleCollectionTest)
                        {
                            _existsSampleTest.IsCollected = test.IsCollected;
                            _existsSampleTest.IsUrgent = test.IsUrgent;
                            _existsSampleTest.IsPriority = test.IsPriority;
                            _existsSampleTest.Remarks = test.Remarks;
                            db.SampleCollectionTests.Update(_existsSampleTest);
                            db.SaveChanges();
                        }

                    }
                }

                return new Ret { status = true, message = "Test data saved successfully." };
            }
            catch (Exception ex)
            {
                Log.Information("Sample Collection Model => Save Sample Collection Test  exception at " + DateTime.UtcNow.ToString() + " message is : " + ex.Message);
                return new Ret { status = false, message = "Something went wrong" };
            }
        }

        public Ret GetBillDetails(SampleCollectionSearch entity)
        {
            try
            {
                DBHelper _dbHelper = new DBHelper(db.GetConnectionString());
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@P_BILLNO", entity.BillNo);
                DataSet ds = _dbHelper.ExecuteDataSetSP("PKG_GET_BILLING_DETAILS_FOR_SAMPLECOLLECTION", parameters);
                if (ds is not null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return new Ret { status = true, data = CommonLogic.GetSQLJsonObject(ds), message = "Billing details loaded successfully." };
                }
                else
                {
                    return new Ret { status = false };
                }
                #region 
                //string LabNo = DateTime.Now.ToString("yyMM") + "0001";
                //string TransNo = "S" + DateTime.Now.ToString("yyMMdd") + "001";
                //string _condition = DateTime.Now.ToString("yyMMdd");
                //SampleCollectionEntity _existsSc = db.SampleCollection.Where(sc => sc.LabNo.StartsWith(_condition)).OrderByDescending(sc => sc).FirstOrDefault();
                //if (_existsSc is SampleCollectionEntity && _existsSc.LabNo != null)
                //{
                //    string lastNumber = _existsSc.LabNo.Substring(6);
                //    LabNo = int.TryParse(lastNumber, out int _lastNumber) && DateTime.Now.ToString("yyMMdd") == _existsSc.LabNo.Substring(1, 6) ? $"{DateTime.UtcNow.ToString("yyMM") + _lastNumber + 1:D8}" : LabNo;
                //    string LasttxnNo = _existsSc.TransNo.Substring(7);
                //    TransNo = int.TryParse(LasttxnNo, out int _lastTxnNumber) && DateTime.Now.ToString("yyMMdd") == _existsSc.TransNo.Substring(1, 6) ? $"S{DateTime.UtcNow.ToString("yyMMdd") + _lastTxnNumber + 1:D10}" : TransNo;
                //}
                //var sampleCollections = from _billing in db.OPBilling
                //                            // join _opr in db.OPBillReceipt on _billing.OPBillId equals _opr.OpBillId
                //                        join _patient in db.Patient on _billing.PatientId equals _patient.PatientId into patients
                //                        from _patients in patients.DefaultIfEmpty()
                //                        join _usr in db.Users on _patients.UserId equals _usr.Id into pUsers
                //                        from users in pUsers.DefaultIfEmpty()
                //                        join _osp in db.OSPatient on _billing.PatientId equals _osp.PatientId into OSpatients
                //                        from osp in OSpatients.DefaultIfEmpty()
                //                        join _doctor in db.Doctors on _billing.ConsultantId equals _doctor.DoctorId into doctors
                //                        from doctor in doctors.DefaultIfEmpty()
                //                        join _sc in db.SampleCollection on _billing.BillNo equals _sc.BillNo into _sampleCollections
                //                        from sc in _sampleCollections.DefaultIfEmpty()
                //                        where _billing.BillNo == entity.BillNo
                //                        select new
                //                        {
                //                            BillNo = entity.BillNo ?? "",
                //                            BillId = _billing.OPBillId,
                //                            UMRNumber = _billing.BillType == "OSP" ? osp.OspNo : users.UserName ?? "",
                //                            PatientName = _billing.BillType == "OSP" ? osp.PatientName : users.UserFullName ?? "",
                //                            Gender = _billing.BillType == "OSP" ? osp.Gender : users.Gender ?? "",
                //                            Age = _billing.BillType == "OSP" ? osp.Age : _patients.Age ?? "",
                //                            DoctorCode = doctor.DoctorCode ?? "",
                //                            DoctorName = doctor.DoctorName ?? "",
                //                            TransNo = TransNo ?? "",
                //                            LabNo = LabNo ?? "",
                //                            VIPSource = _billing.VIPSource ?? "",
                //                            VIPRemarks = _billing.VIPRemarks ?? "",
                //                            BillCreatedDate = _billing.CreatedDate.HasValue ? Convert.ToDateTime(_billing.CreatedDate).ToString("dd-MM-yyyy") : "",
                //                            SampleCollectionTests = (from _opServices in db.OPServiceBooking
                //                                                     join _tariffServices in db.TariffServiceMapping on _opServices.ServiceId equals _tariffServices.Id
                //                                                     join _services in db.Services on _tariffServices.ServiceId equals _services.Id
                //                                                     join _sct in db.SampleCollectionTests on _tariffServices.Id equals _sct.TestId into _sampleCollectionTests
                //                                                     from sct in _sampleCollectionTests.DefaultIfEmpty()
                //                                                     where _opServices.OpBillId == _billing.OPBillId && _tariffServices.TariffId == 1
                //                                                     && _services.IsSampleNeeded == "Yes" && _services.ServiceType == "I"
                //                                                     select new
                //                                                     {
                //                                                         TestId = _opServices.ServiceId,
                //                                                         ServiceCode = _services.ServiceCode ?? "",
                //                                                         ServiceName = _services.ServiceName ?? "",
                //                                                         IsOutSide = _services.IsOutSide ?? "No",
                //                                                         IsPackage = _services.IsPackage ?? "No",
                //                                                         Specimen = "",
                //                                                         Dosage = "",
                //                                                         Vacutainer = "",
                //                                                         Precautions = ""
                //                                                     }
                //                         ).ToList()
                //                        };
                #endregion

                //return new Ret { status = true, data = sampleCollections, message = "Billing details loaded successfully." };
            }
            catch (Exception ex)
            {
                Log.Information("Sample Collection Model => Get Billing Details exception at " + DateTime.UtcNow.ToString() + " message is : " + ex.Message);
                return new Ret { status = false, message = "Something went wrong" };
            }
        }

        public Ret GetBarcode(SampleCollectionSearch entity)
        {
            try
            {
                DBHelper _dbHelper = new DBHelper(db.GetConnectionString());
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@P_BILLNO", entity.BillNo);
                DataSet ds = _dbHelper.ExecuteDataSetSP("PKG_GET_BARCODE", parameters);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return new Ret { status = true, data = CommonLogic.GetJsonObject(ds), message = "Barcode generated successfully" };
                }
                else
                {
                    return new Ret { status = false, message = "Failed to generate barcode" };
                }
            }
            catch (Exception ex)
            {
                Log.Information("Sample Collection Model => Get Barcode exception at " + DateTime.UtcNow.ToString() + " message is : " + ex.Message);
                return new Ret { status = false, message = "Something went wrong" };
            }
        }


    }
}
