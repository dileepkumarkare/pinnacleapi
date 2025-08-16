using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pinnacle.Entities;
using Pinnacle.Helpers;
using Pinnacle.Helpers.JWT;
using Pinnacle.Models;

namespace Pinnacle.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class OPBillingController : ControllerBase
    {
        OPBillingModel model = new OPBillingModel();
        MasterModel masterModel = new MasterModel();
        JwtStatus jwtStatus = new JwtStatus();
        private readonly string _ipAddress;
        public OPBillingController(IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext;

            _ipAddress = httpContext?.Request?.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(_ipAddress))
            {
                _ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString();
            }
        }

        [HttpPost]
        [Route("GetOSPandBillNumber")]
        public IActionResult GetOSPandBilNumber()
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);

            if (tokenStatus.data != null)
            {
                jwtStatus = tokenStatus.data;
                jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
            }
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetOSPandBillNumber(jwtStatus) : accessStatus;
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
        [Route("SaveOpBilling")]
        public IActionResult SaveOpBilling(OPBillingCollection entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);

            if (tokenStatus.data != null)
            {
                jwtStatus = tokenStatus.data;
                jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
            }
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.SaveOPBilling(entity, jwtStatus) : accessStatus;
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
        [Route("SaveOpBillingReceipt")]
        public IActionResult SaveOpBillingReceipt(OPBillingCollection entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);

            if (tokenStatus.data != null)
            {
                jwtStatus = tokenStatus.data;
                jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
            }
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.SaveOpBillReceipt(entity, jwtStatus, _ipAddress) : accessStatus;
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
        [Route("GetServices")]
        public IActionResult GetServices(OPBillingFilter entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            if (tokenStatus.data != null)
            {
                jwtStatus = tokenStatus.data;
                jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
            }
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetServices(entity,jwtStatus) : accessStatus;
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
        [Route("GetServiceCharge")]
        public IActionResult GetServiceCharge(OPBillingFilter entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            if (tokenStatus.data != null)
            {
                jwtStatus = tokenStatus.data;
                jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
            }
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetServiceCharge(entity,jwtStatus) : accessStatus;
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
        [Route("GetOpBilling")]
        public IActionResult GetOpBilling(Pagination entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);

            if (tokenStatus.data != null)
            {
                jwtStatus = tokenStatus.data;
                jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
            }
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetOpBilling(entity, jwtStatus) : accessStatus;
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
        [Route("GetOpBillReceipt")]
        public IActionResult GetOpBillReceipt(OnlyId entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetOpBillReceipt(entity.Id) : accessStatus;
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
        [Route("GetInvestigations")]
        public IActionResult GetInvestigations(OPBillingFilter entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetInvestigation(entity) : accessStatus;
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
        [Route("GetBillingDetailsByRefNo")]
        public IActionResult GetBillingDetailsByRefNo(OPBillingFilter entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);

            if (tokenStatus.data != null)
            {
                jwtStatus = tokenStatus.data;
                jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
            }
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetOpBillingByRef(entity.RefNo, jwtStatus) : accessStatus;
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
