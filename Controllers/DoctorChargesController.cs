using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Pinnacle.Models;

namespace Pinnacle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorChargesController : ControllerBase
    {
        MasterModel masterModel = new MasterModel();
        DoctorChargesModel doctorModel = new DoctorChargesModel();
        JwtStatus jwtStatus = new JwtStatus();

        [HttpPost]
        [Route("SaveDoctorCharges")]
        public IActionResult SaveDoctor(DoctorCharges entity)
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
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : doctorModel.SaveDoctorCharges(entity, jwtStatus);
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }
        }
        [HttpPost]
        [Route("GetDoctorCharges")]
        public IActionResult GetDoctorCharges(Pagination entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            if (tokenStatus.data != null)
            {
                jwtStatus = tokenStatus.data;
                jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
            }
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? doctorModel.GetDoctorCharges(entity) : accessStatus;
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
        [Route("GetConsultationFee")]
        public IActionResult GetConsultationFee(DoctorCharges entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            if (tokenStatus.data != null)
            {
                jwtStatus = tokenStatus.data;
                jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
            }
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? doctorModel.GetConsultationFee(entity, jwtStatus) : accessStatus;
            return Ok(new
            {
                status = res.status,
                IstokenExpired = tokenStatus.IstokenExpired ?? false,
                message = res.message,
                data = res.data,
                totalCount = res.totalCount ?? 0
            });
        }
    }
}
