using Microsoft.AspNetCore.Mvc;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Pinnacle.Models;
using Pinnacle.Helpers;
using Serilog;




namespace Pinnacle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        AuthenticationModel model = new AuthenticationModel();
        MasterModel mastermodel = new MasterModel();
        PinnacleDbContext db = new PinnacleDbContext();
        private readonly JwtSettings jwtSettings;

        public AuthenticationController(JwtSettings jwtSettings)
        {
            this.jwtSettings = jwtSettings; 
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginEntity entity)
        {
            try
            {
                var Token = new TokenEntity();
                Ret res = model.Login(entity);
                if (res.status)
                {

                    Token = Helpers.JWT.JwtHelpers.GenTokenkey(new TokenEntity()
                    {
                       // Email = res.data.Email,                       
                        UserName = res.data.UserName,
                        UserId = res.data.UserId,
                        Id = Convert.ToInt16(res.data.Id)  ,
                        //OrganizationId = Convert.ToInt16(res.data.OrganizationId),
                        UserProfileId = Convert.ToInt16(res.data.UserProfileId),
                        RoleId = Convert.ToInt16(res.data.RoleId),
                        HospitalId= res.data.HospitalId
                    }, jwtSettings);
                }


                return Ok(new { status = res.status, IstokenExpired = false, message = res.message, data = res.status ? new { user = res.data, token = Token.Token } : null });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }
        }
        [HttpPost]
        [Route("Encrypt")]
        public IActionResult Encrypt(string Text)
        {
            string en = CommonLogic.Encrypt(Text);
            return Ok(en);
        }

        [HttpPost]
        [Route("UpdatePassword")]

        public IActionResult UpdatePassword(UpdatePasswordEntity entity)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                Ret tokenStatus = mastermodel.CheckToken(token);
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : model.UpdatePassword(entity, tokenStatus.data);
                return Ok(new { status = res.status, IstokenExpired = tokenStatus.IstokenExpired ?? false, message = res.message, data = res.data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }
        }

        [HttpPost]
        [Route("SendForgotPasswordMail")]
        public IActionResult SendForgotPasswordMail(ServerClass serverclass)
        {
            try
            {
                Ret userDetails = model.SendForgotPasswordMail(serverclass.Email, serverclass.ServerName);

                if (userDetails != null)
                    return Ok(new { status = userDetails.status, message = userDetails.message, data = userDetails.data });
                else
                    return Ok(new { status = userDetails.status, message = userDetails.message });

            }
            catch (Exception ex)
            {
                Log.Information("Send mail to set password at " + DateTime.Now.ToString() + " message " + (ex.Message));
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("SetPassword")]
        //POST : /api/Authentication/SetPassword 
        public IActionResult SetPassword(PasswordClass PCEntity)
        {
            try
            {
                Ret UserObj = model.SetPassword(PCEntity);
                if (UserObj.status)
                {
                    return Ok(new { status = UserObj.status, message = UserObj.message, data = UserObj.data });
                }
                else
                {
                    return Ok(new { status = UserObj.status, message = UserObj.message });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.InnerException });
            }

        }
    }
}
