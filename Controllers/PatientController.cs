using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pinnacle.Entities;
using Pinnacle.IServices;
using Pinnacle.Models;

namespace Pinnacle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        MasterModel masterModel = new MasterModel();
        private WhatsappModel _whatsappmodel;
        // PatientModel Model = new PatientModel();
        private readonly PatientModel Model;
        public PatientController(IWhatsappService whatsappService)
        {
            Model = new PatientModel(whatsappService);
            _whatsappmodel = new WhatsappModel(whatsappService);
        }

        [HttpPost]
        [Route("SavePatientBasicDetails")]
        public IActionResult SavePatientBasicDetails(PatientEntity entity)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : Model.SavePatientBasicDetails(entity, tokenStatus.data);
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }
        }
        [HttpPost]
        [Route("GetAllPatient")]
        public IActionResult GetAllPatient(Pagination entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? Model.GetAllPatient(entity, tokenStatus.data) : accessStatus;
            return Ok(new
            {
                status = res.status,
                IstokenExpired = tokenStatus.IstokenExpired ?? false,
                message = res.message,
                data = res.data,
                totalCount = res.totalCount ?? 0
            });
        }
        [HttpPost]
        [Route("PatientGetById")]
        public IActionResult PatientGetById(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? Model.PatientGetById(obj.Id) : accessStatus;
            return Ok(new
            {
                status = res.status,
                IstokenExpired = tokenStatus.IstokenExpired ?? false,
                message = res.message,
                data = res.data,
                totalCount = res.totalCount ?? 0
            });
        }
        [HttpPost]
        [Route("GetAllReligionLabel")]
        public IActionResult GetAllReligionLabel()
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? Model.GetAllReligionLabel() : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("PatientUMRGetByNumber")]
        public IActionResult PatientUMRGetByNumber(Search umr)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? Model.PatientUMRGetByNumber(umr) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("GetUMRNumber")]
        public IActionResult GetUMRNumber()
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? Model.GetUMRNumber() : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        //[HttpPost]
        //[Route("SavePatientAddress")]
        //public IActionResult SavePatientAddress(PatientAddress entity)
        //{
        //    try
        //    {
        //        string token = Request.Headers["Authorization"];
        //        Ret tokenStatus = masterModel.CheckToken(token);
        //        Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : Model.SavePatientAddress(entity);
        //        return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
        //    }
        //}
        [HttpPost]
        [Route("GetPatientAddress")]
        public IActionResult GetPatientAddress(OnlyId entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? Model.GetPatientAddress(entity.Id) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }

        [HttpPost]
        [Route("GetRecNumber")]
        public IActionResult GetRecNumber()
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? Model.GetPatientReceiptNumber() : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("GetPatientUmrNumbersList")]
        public IActionResult GetPatientUmrNumbersList()
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? Model.GetPatientUmrNumbersList(tokenStatus.data) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("SavePatientProfile")]
        public IActionResult SavePatientProfile(PatientEntity entity)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : Model.SavePatientProfile(entity);
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }
        }
        [HttpPost]
        [Route("GetOpConsultations")]
        public IActionResult GetOpConsultations(Pagination entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? Model.GetOpConsultations(entity, tokenStatus.data) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }

        [HttpPost]
        [Route("SendWhatsappMessage")]
        public IActionResult SendWhatsappMessage(string PhoneNumber, string PatientName, string UHID, string Nationality, string AreaCode)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : _whatsappmodel.SendUHIDNumberToWhatsapp(PhoneNumber, PatientName, UHID, Nationality, AreaCode, tokenStatus.data);
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }
        }
        [HttpPost]
        [Route("Registration")]
        public IActionResult Registration(PatientEntity entity)
        {
            try
            {
                Ret res = Model.Registration(entity);
                return Ok(new { status = res.status, IstokenExpired = false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }
        }
    }
}
