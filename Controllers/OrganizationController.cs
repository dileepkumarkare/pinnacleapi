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
    public class OrganizationController : ControllerBase
    {
        OrganizationModel orgModel = new OrganizationModel();
        JwtStatus jwtStatus = new JwtStatus();
        MasterModel masterModel = new MasterModel();
        private readonly string _ipAddress;
        public OrganizationController(IHttpContextAccessor httpContextAccessor)
        {
            _ipAddress = httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (_ipAddress == null)
            {
                _ipAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            }
        }

        [HttpPost]
        [Route("SaveOrganization")]
        public IActionResult SaveOrganization(OrganizationEntity entity)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret accessStatus = masterModel.CheckAceess(true);

                if (tokenStatus.data != null)
                {
                    jwtStatus = tokenStatus.data;
                    jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
                }
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? orgModel.SaveOrganization(entity, jwtStatus, _ipAddress) : accessStatus;
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok();

            }
        }

        [HttpPost]
        [Route("GetAllOrganization")]
        public IActionResult GetAllOrganization(Pagination entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);

            if (tokenStatus.data != null)
            {
                jwtStatus = tokenStatus.data;
                jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
            }
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? orgModel.GetAllOrganization(entity, jwtStatus) : accessStatus;
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
        [Route("GetAllOrganizationLabel")]
        public IActionResult GetAllOrganizationLabel(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);

            if (tokenStatus.data != null)
            {
                jwtStatus = tokenStatus.data;
                jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
            }
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? orgModel.GetAllOrganizationLabel(obj.Id, jwtStatus) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("SaveCharges")]
        public IActionResult SaveCharges(OrganizationChargeEntity entity)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret accessStatus = masterModel.CheckAceess(true);

                if (tokenStatus.data != null)
                {
                    jwtStatus = tokenStatus.data;
                    jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
                }
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? orgModel.SaveCharges(entity, jwtStatus) : accessStatus;
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok();

            }
        }
        [HttpPost]
        [Route("GetCharges")]
        public IActionResult GetCharges(Pagination entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? orgModel.GetCharges(entity) : accessStatus;
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
        [Route("UpdateDoctorCharge")]
        public IActionResult UpdateDoctorCharge(OrganizationChargeEntity entity)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret accessStatus = masterModel.CheckAceess(true);

                if (tokenStatus.data != null)
                {
                    jwtStatus = tokenStatus.data;
                    jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
                }
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? orgModel.UpdateDoctorCharge(entity, jwtStatus) : accessStatus;
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok();

            }
        }
        [HttpPost]
        [Route("UpdateStatus")]
        public IActionResult UpdateStatus(OrganizationChargeEntity entity)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret accessStatus = masterModel.CheckAceess(true);

                if (tokenStatus.data != null)
                {
                    jwtStatus = tokenStatus.data;
                    jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
                }
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? orgModel.UpdateStatus(entity, jwtStatus) : accessStatus;
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok();

            }
        }
        [HttpPost]
        [Route("GetDoctors")]
        public IActionResult GetDoctors(OnlyId entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? orgModel.GetDoctors(entity) : accessStatus;
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
        [Route("GetPatientList")]
        public IActionResult GetPatientList(OnlyId entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? orgModel.GetPatientList(entity) : accessStatus;
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
