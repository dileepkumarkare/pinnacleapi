using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pinnacle.Entities;
using Pinnacle.Models;
using Serilog;

namespace Pinnacle.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        MasterModel masterModel = new MasterModel();
        DoctorModal doctorModel = new DoctorModal();
        private readonly string _ipAddress;


        public DoctorController(IHttpContextAccessor httpContextAccessor)
        {
            _ipAddress = httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(_ipAddress))
            {
                _ipAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            }
        }

        [HttpPost]
        [Route("SaveDoctor")]
        public IActionResult SaveDoctor(DoctorEntity entity)
        {
            try
            {
                var ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                }
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : doctorModel.SaveDoctor(entity, tokenStatus.data, ipAddress);
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }
        }

        [HttpPost]
        [Route("SaveDoctorDetails")]
        public IActionResult SaveDoctorDetails(DoctorDetailsEntity entity)
        {
            try
            {

                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : doctorModel.SaveDoctorDetails(entity, tokenStatus.data, _ipAddress);
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }
        }

        [HttpPost]
        [Route("GetAllDoctors")]
        public IActionResult GetAllDoctors(Pagination entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? doctorModel.GetAllDoctors(entity, tokenStatus.data) : accessStatus;
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
        [Route("GetDoctorsList")]
        public IActionResult GetDoctorsList(Pagination entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? doctorModel.GetDoctorsList(entity, tokenStatus.data) : accessStatus;
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
        [Route("DoctorGetById")]
        public IActionResult DoctorGetById(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? doctorModel.DoctorGetById(obj.Id) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("RemoveEducationDetails")]
        public IActionResult RemoveEducationDetails(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? doctorModel.RemoveEducationDetails(obj.Id, _ipAddress, tokenStatus.data) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }



        [HttpPost]
        [Route("DoctorPersonalGetById")]
        public IActionResult GetPersonalDetails(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? doctorModel.GetPersonalDetails(obj.Id) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }

        [HttpPost]
        [Route("SaveDoctorEducation")]
        public IActionResult SaveDoctorEducation(DoctorEducationUpload entity)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : doctorModel.SaveDoctorEducation(entity, tokenStatus.data);
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }
        }


        [HttpPost]
        [Route("SaveDoctorExperience")]
        public IActionResult SaveDoctorExperience(DoctorExperienceUpload entity)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : doctorModel.SaveDoctorExperience(entity, tokenStatus.data);
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }
        }


        [HttpPost]
        [Route("GetAllDoctorEducation")]
        public IActionResult GetAllDoctorEducation(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? doctorModel.GetAllDoctorEducation(obj.Id) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("GetAllDoctorExperience")]
        public IActionResult GetAllDoctorExperience(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? doctorModel.GetAllDoctorExperience(obj.Id) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("SaveDoctorProfile")]

        public IActionResult SaveDoctorProfile([FromForm] DoctorProfileUpload entity)
        {
            try
            {
                Ret Configs = doctorModel.SaveDoctorProfile(entity);
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
        [Route("RemoveExperienceDetails")]
        public IActionResult RemoveExperienceDetails(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? doctorModel.RemoveExperienceDetails(obj.Id, _ipAddress, tokenStatus.data) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("GetAllDoctorLabel")]
        public IActionResult GetAllDoctorLabel(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? doctorModel.GetAllDoctorLabel(obj.Id, tokenStatus.data) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }

        [HttpPost]
        [Route("GetOpConsultationByDoctorId")]
        public IActionResult GetOpConsultationByDoctorId(Pagination entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? doctorModel.GetOpConsultationByDoctorId(entity, tokenStatus.data) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data, totalCount = res.totalCount });
        }

        [HttpPost]
        [Route("GetOpConsultationByNurseId")]
        public IActionResult GetOpConsultationByNurseId(PrescriptionFilter entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? doctorModel.GetOpConsultationByNurseId(entity, tokenStatus.data) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data, totalCount = res.totalCount });
        }

        [HttpPost]
        [Route("ScheduleAvailability")]
        public IActionResult ScheduleAvailability(JsonEntity je)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret accessStatus = masterModel.CheckAceess(true);
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? doctorModel.ScheduleAvailability(je, tokenStatus.data) : accessStatus;
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data, totalCount = res.totalCount });

            }
            catch (Exception ex)
            {
                Log.Information("Clinician Controller=> ScheduleAvailability Method Error at =>" + DateTime.Now.ToString() + " Error Message =>" + ex.Message);
                return Ok(new { status = false, message = "Something went wrong,Please try again!!" });
            }
        } //END

        [HttpPost]
        [Route("GetAvailability")]
        public IActionResult GetAvailability(JsonEntity je)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret accessStatus = masterModel.CheckAceess(true);
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? doctorModel.GetAvailability(je, tokenStatus.data) : accessStatus;
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data, totalCount = res.totalCount });

            }
            catch (Exception ex)
            {
                Log.Information("Clinician Controller=> GetAvailability Method Error at =>" + DateTime.Now.ToString() + " Error Message =>" + ex.Message);
                return Ok(new { status = false, message = "Something went wrong,Please try again!!" });
            }

        }

        [HttpPost]
        [Route("GetAvailabilityWithAppointment")]
        public IActionResult GetAvailabilityWithAppointment(JsonEntity je)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret accessStatus = masterModel.CheckAceess(true);
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? doctorModel.GetAvailabilityWithAppointment(je) : accessStatus;
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data, totalCount = res.totalCount });

            }
            catch (Exception ex)
            {
                Log.Information("Clinician Controller=> GetAvailabilityWithAppointment Method Error at =>" + DateTime.Now.ToString() + " Error Message =>" + ex.Message);
                return Ok(new { status = false, message = "Something went wrong,Please try again!!" });
            }

        }


        [HttpPost]
        [Route("ScheduleStatusUpdate")]
        public IActionResult ScheduleStatusUpdate(JsonEntity je)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret accessStatus = masterModel.CheckAceess(true);
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? doctorModel.ScheduleStatusUpdate(je, tokenStatus.data) : accessStatus;
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data, totalCount = res.totalCount });

            }
            catch (Exception ex)
            {
                Log.Information("Clinician Controller=> ScheduleStatusUpdate Method Error at =>" + DateTime.Now.ToString() + " Error Message =>" + ex.Message);
                return Ok(new { status = false, message = "Something went wrong,Please try again!!" });
            }
        } //END

        [HttpPost]
        [Route("GetAvailabilityWithAppointmentMobile")]
        public IActionResult GetAvailabilityWithAppointmentMobile(JsonEntity je)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret accessStatus = masterModel.CheckAceess(true);
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? doctorModel.GetAvailabilityWithAppointmentMobile(je, tokenStatus.data) : accessStatus;
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data, totalCount = res.totalCount });

            }
            catch (Exception ex)
            {
                Log.Information("Clinician Controller=> GetAvailabilityWithAppointment Method Error at =>" + DateTime.Now.ToString() + " Error Message =>" + ex.Message);
                return Ok(new { status = false, message = "Something went wrong,Please try again!!" });
            }
        }
        [HttpPost]
        [Route("GetAllHospitalDoctors")]
        public IActionResult GetAllHospitalDoctorsList()
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? doctorModel.GetAllHospitalDoctorsList() : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data, totalCount = res.totalCount });
        }

    }
}
