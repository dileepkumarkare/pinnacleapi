using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pinnacle.Entities;
using Pinnacle.Models;
using Serilog;
namespace Pinnacle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationController : ControllerBase
    {
        DesignationModel model = new DesignationModel();
        MasterModel masterModel = new MasterModel();


        [HttpPost]
        [Route("SaveDesignation")]
        public IActionResult SaveDesignation(DesignationEntity entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.SaveDesignation(entity, tokenStatus.data) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("DesignationFileImport")] 
        public IActionResult DesignationFileImport([FromForm]ImportFile File)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = masterModel.CheckToken(token);
                Ret accessStatus = masterModel.CheckAceess(true);
                Ret Configs = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.DesignationFileImport(File,tokenStatus.data) : accessStatus;
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
        [Route("SaveTitle")]
        public IActionResult SaveTitle(TitleEntity entity)
        { 
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.SaveTitle(entity) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("GetAllTitles")]
        public IActionResult GetAllTitles(Pagination entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetAllTitles(entity) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("GetAllTitle")]
        public IActionResult GetAllTitle()
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetAllTitle() : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }

        [HttpPost]
        [Route("GetAllDesignations")]
        public IActionResult GetAllDesignations(Pagination entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetAllDesignations(entity) : accessStatus;
            return Ok(new
            { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data, totalCount = res.totalCount ?? 0
            });
        }


        [HttpPost]
        [Route("GetAllDesignationsLabel")]
        public IActionResult GetAllDesignationsLabel(OnlyId obj)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? model.GetAllDesignationsLabel(obj.Id) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
    }
}
