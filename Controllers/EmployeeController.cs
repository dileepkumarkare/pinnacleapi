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
    public class EmployeeController : ControllerBase
    {
        MasterModel masterModel = new MasterModel();
        EmployeeModel employeeModel = new EmployeeModel();
        JwtStatus jwtStatus = new JwtStatus();

        [HttpPost]
        [Route("GenerateEmployeeId")]
        public IActionResult GenerateEmployeeId(EmployementType entity)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : employeeModel.GenerateEmployeeId(entity.MedicalTestRequired);
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }
        }
        [HttpPost]
        [Route("SaveEmployeeBasicDetails")]
        public IActionResult SaveEmployeeBasicDetails(UserEntity entity)
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
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : employeeModel.SaveEmployeeBasicDetails(entity, jwtStatus);
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }
        }
        [HttpPost]
        [Route("EmployeeGetById")]
        public IActionResult EmployeeGetById(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? employeeModel.EmployeeGetById(obj.Id) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("StatusUpdate")]
        public IActionResult StatusUpdate(StatusUpdate entity)
        {
            try
            {
                string token = Request.Headers["authorization"];
                Ret tokenstatus = masterModel.CheckToken(token);
                Ret res = tokenstatus.IstokenExpired == true || tokenstatus.status == false ? tokenstatus : employeeModel.StatusUpdate(entity);
                return Ok(new { status = res.status, IstokenExpired = tokenstatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }
        }
        [HttpPost]
        [Route("SaveEmployeeEducation")]
        public IActionResult SaveEmployeeEducation(EmployeeEducation entity)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : employeeModel.SaveEmployeeEducation(entity);
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }
        }
        [HttpPost]
        [Route("SaveEmployeeExperience")]
        public IActionResult SaveEmployeeExperience(EmployeeExperience entity)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : employeeModel.SaveEmployeeExperience(entity);
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }
        }
        [HttpPost]
        [Route("GetAllEducation")]
        public IActionResult GetAllEducation(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? employeeModel.GetAllEducation(obj.Id) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("GetAllExperience")]
        public IActionResult GetAllExperience(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? employeeModel.GetAllExperience(obj.Id) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }


        [HttpPost]
        [Route("SaveEmployeeAddress")]
        public IActionResult SaveEmployeeAddress(EmployeeAddress entity)
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
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : employeeModel.SaveEmployeeAddress(entity, jwtStatus);
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }
        }
        [HttpPost]
        [Route("GetAllEmployee")]
        public IActionResult GetAllEmployee(Pagination entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);

            if (tokenStatus.data != null)
            {
                jwtStatus = tokenStatus.data;
                jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
            }
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? employeeModel.GetAllEmployee(entity, jwtStatus) : accessStatus;
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
        [Route("AddressGetById")]
        public IActionResult AddressGetById(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? employeeModel.AddressGetById(obj.Id) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("GetAllAddress")]
        public IActionResult GetAllAddress(Pagination entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? employeeModel.GetAllAddress(entity) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data, totalCount = res.totalCount ?? 0 });
        }
        [HttpPost]
        [Route("SaveDocuments")]
        public IActionResult SaveDocuments([FromForm] FileUploadEntity entity)
        {
            try
            {
                Ret Configs = employeeModel.SaveDocuments(entity);
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
        [HttpPost]
        [Route("GetEmpDocuments")]
        public IActionResult GetEmpDocuments(OnlyId onlyid)
        {
            try
            {
                Ret res = employeeModel.GetEmpDocuments(onlyid.Id);
                if (res.status)
                {
                    return Ok(new { status = res.status, message = res.message, data = res.data });
                }
                else
                {
                    return Ok(new { status = res.status, message = res.message, data = res.data });
                }
            }
            catch (Exception ex)
            {
                Log.Information("GetTermsConditions Config at " + DateTime.Now.ToString() + " message " + (ex.Message));
                return Ok(new { status = "Error", message = "Failed to get data. Error: " + ex.Message });
            }
        }
        [HttpPost]
        [Route("DeleteEmpDoc")]
        public IActionResult DeleteEmpDoc(OnlyId onlyid)
        {
            try
            {
                Ret res = employeeModel.DeleteEmpDoc(onlyid.Id);
                if (res.status)
                {
                    return Ok(new { status = res.status, message = res.message, data = res.data });
                }
                else
                {
                    return Ok(new { status = res.status, message = res.message, data = res.data });
                }
            }
            catch (Exception ex)
            {
                Log.Information("GetTermsConditions Config at " + DateTime.Now.ToString() + " message " + (ex.Message));
                return Ok(new { status = "Error", message = "Failed to get data. Error: " + ex.Message });
            }
        }
        [HttpPost]
        [Route("SaveProfilePhoto")]

        public IActionResult SaveProfilePhoto([FromForm] ProfilePhoto entity)
        {
            try
            {
                Ret Configs = employeeModel.SaveProfilePhoto(entity);
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
        [HttpPost]
        [Route("UpdateMedicalCheckUpStatus")]
        public IActionResult UpdateMedicalCheckUpStatus(StatusUpdate entity)
        {
            try
            {
                string token = Request.Headers["authorization"];
                Ret tokenstatus = masterModel.CheckToken(token);
                Ret res = tokenstatus.IstokenExpired == true || tokenstatus.status == false ? tokenstatus : employeeModel.UpdateMedicalCheckUpStatus(entity);
                return Ok(new { status = res.status, IstokenExpired = tokenstatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }
        }
        [HttpPost]
        [Route("GetAllEmployeeLabel")]
        public IActionResult GetAllEmployeeLabel(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);

            if (tokenStatus.data != null)
            {
                jwtStatus = tokenStatus.data;
                jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
            }
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? employeeModel.GetAllEmployeeLabel(obj.Id, jwtStatus) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
    }
}
