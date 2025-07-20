using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pinnacle.Entities;
using Pinnacle.Models;
using System.Diagnostics.Contracts;

namespace Pinnacle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorPrescriptionController : ControllerBase
    {
        DoctorPrescriptionModel model = new DoctorPrescriptionModel();
        MasterModel masterModel = new MasterModel();
        [HttpPost]
        [Route("SavePrescription")]
        public IActionResult SavePrescription(DoctorPrescription entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : model.SaveDoctorPrescription(entity, tokenStatus.data);
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("GetDoctorPrescription")]
        public IActionResult GetDoctorPrescription(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetDoctorPrescription(obj.Id) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }


        [HttpPost]
        [Route("GetPatientEHR")]
        public IActionResult GetPatientEHR(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetPatientEHR(obj.Id) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
    }
}

