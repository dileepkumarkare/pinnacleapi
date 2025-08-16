using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Pinnacle.Models;
using Serilog;
namespace Pinnacle.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class SpecializationController : ControllerBase
    {

        SpecializationModel model = new SpecializationModel();
        MasterModel masterModel = new MasterModel();
        JwtStatus jwtStatus = new JwtStatus();

        [HttpPost]
        [Route("SaveSpecialization")]
        public IActionResult SaveSpecialization(SpecializationEntity entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);

            if (tokenStatus.data != null)
            {
                jwtStatus = tokenStatus.data;
                jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
            }
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.SaveSpecialization(entity, jwtStatus) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }


        [HttpPost]
        [Route("GetAllSpecializations")]
        public IActionResult GetAllSpecializations(Pagination entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetAllSpecializations(entity) : accessStatus;
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
        [Route("GetAllSpecializationsLabel")]
        public IActionResult GetAllSpecializationsLabel(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetAllSpecializationsLabel(obj.Id) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
    }
}
