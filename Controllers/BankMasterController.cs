using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Pinnacle.Models;

namespace Pinnacle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankMasterController : ControllerBase
    {
        BankMasterModel model = new BankMasterModel();
        MasterModel masterModel = new MasterModel();
        JwtStatus jwtStatus = new JwtStatus();
        [HttpPost]
        [Route("GetAllBankMasterLabel")]
        public IActionResult GetAllDoctorLabel(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            if (tokenStatus.data != null)
            {
                jwtStatus = tokenStatus.data;
                jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
            }
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetAllBankLabel(obj.Id, jwtStatus) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("GetAllBanksList")]
        public IActionResult GetAllBanksList(Pagination entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            if (tokenStatus.data != null)
            {
                jwtStatus = tokenStatus.data;
                jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
            }
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetAllBanksList(entity, jwtStatus) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("SaveBankMaster")]
        public IActionResult SaveBankMaster(BankMaster entity)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                if (tokenStatus.data != null)
                {
                    jwtStatus = tokenStatus.data;
                    jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
                }
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : model.SaveBankMaster(entity, jwtStatus);
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }
        }

    }
}
