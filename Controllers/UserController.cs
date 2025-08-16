using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Pinnacle.Models;

namespace Pinnacle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        MasterModel masterModel = new MasterModel();
        UserModel UserModel = new UserModel();
        JwtStatus jwtStatus = new JwtStatus();

        [HttpPost]
        [Route("SaveUser")]
        public IActionResult SaveUser(UserEntity entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);

            if (tokenStatus.data != null)
            {
                jwtStatus = tokenStatus.data;
                jwtStatus.HospitalId = Request.Headers["X-Hospital-Id"].FirstOrDefault() != null ? Convert.ToInt32(Request.Headers["X-Hospital-Id"].FirstOrDefault()) : 0;
            }
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? UserModel.SaveUser(entity, jwtStatus) : accessStatus;
            return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
        }
        [HttpPost]
        [Route("GetAllUsers")]
        public IActionResult GetAllUsers(Pagination entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? UserModel.GetAllUsers(entity) : accessStatus;
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
        [Route("UserGetById")]
        public IActionResult UserGetById(Pagination entity)
        {
            string token = Request.Headers["Authorization"];
            Ret tokenStatus = masterModel.CheckToken(token);
            Ret accessStatus = masterModel.CheckAceess(true);
            Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : accessStatus.status ? UserModel.UserGetById(entity) : accessStatus;
            return Ok(new
            {
                status = res.status,
                IstokenExpired = tokenStatus.IstokenExpired ?? false,
                message = res.message,
                data = res.data,
                totalCount = res.totalCount ?? 0
            });
        }
        [HttpGet("download-bak")]
        public IActionResult DownloadBakFile()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Reports", "PinnacleTestDbBackup.bak");

            if (!System.IO.File.Exists(filePath))
                return NotFound("Backup file not found.");

            var mimeType = "application/octet-stream"; // Recommended for binary files like .bak
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, mimeType, "PinnacleTest_20250717.bak");
        }
    }
}
