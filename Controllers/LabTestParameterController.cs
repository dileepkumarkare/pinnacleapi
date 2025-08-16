using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Pinnacle.Models;

namespace Pinnacle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabTestParameterController : ControllerBase
    {
        LabTestParameterModel model = new LabTestParameterModel();
        MasterModel masterModel = new MasterModel();
        JwtStatus jwtStatus = new JwtStatus();

        [HttpPost]
        [Route("SaveLabTestParameters")]
        public IActionResult SaveLabTestParameters(LabTestParametersEntity entity)
        {

            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            jwtStatus = tokenStatus.data;
            jwtStatus.HospitalId = Convert.ToInt32(Request.Headers["X-Hospital-Id"].ToString());
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.SaveLabTestParameters(entity, jwtStatus) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("GetAllLabTestParameters")]
        public IActionResult GetAllServices(Pagination entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            jwtStatus = tokenStatus.data;
            jwtStatus.HospitalId = Convert.ToInt32(Request.Headers["X-Hospital-Id"].ToString());
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetAllLabTestParameters(entity) : accessStatus;
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
        [Route("GetByIdLabTestParameter")]
        public IActionResult GetByIdLabTestParameter(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetByIdLabTestParameter(obj.Id) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("GetAllLabParamLabel")]
        public IActionResult GetAllLabParamLabel(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetAllLabParamLabel(obj.Id) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
    }
}
