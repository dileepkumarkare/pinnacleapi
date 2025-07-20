using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pinnacle.Entities;
using Pinnacle.Models;
using Serilog;
using System.Net;

namespace Pinnacle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalController : ControllerBase
    {
        HospitalModel model = new HospitalModel();
        MasterModel masterModel = new MasterModel();


        [HttpPost]
        [Route("SaveHospital")]
        public IActionResult SaveHospital(HospitalEntity entity)
        {

            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.SaveHospital(entity, tokenStatus.data) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("GetAllHospitalsLabel")]
        public IActionResult GetAllHospitalsLabel()
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetAllHospitalsLabel() : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("GetAllHospitals")]
        public IActionResult GetAllHospitals(Pagination entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetAllHospitals(entity) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data, totalCount = res.totalCount ?? 0 });
        }
        [HttpPost]
        [Route("SaveHospitalLogo")]

        public IActionResult SaveHospitalLogo([FromForm] HospitalProfile entity)
        {
            try
            {
                Ret Configs = model.SaveHospitalLogo(entity);
                if (Configs.status)
                {
                    return Ok(new { status = Configs.status, message = Configs.message, data = Configs.data });
                }
                else
                {
                    return Ok(new { status = Configs.status, message = Configs.message, data = Configs.data });
                }
            }
            catch (Exception ex)
            {
                Log.Information("GetTermsConditions Config at " + DateTime.Now.ToString() + " message " + (ex.Message));
                return Ok(new { status = "Error", message = "Failed to get data. Error: " + ex.Message });
            }
        }
    }
}
