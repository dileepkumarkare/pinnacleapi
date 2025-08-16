using Microsoft.AspNetCore.Mvc;
using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Pinnacle.Models;
using Pinnacle.Helpers;
using Serilog;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using IAuthenticationService = Pinnacle.IServices.IAuthenticationService;



namespace Pinnacle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        AuthenticationModel model = new AuthenticationModel();
        MasterModel mastermodel = new MasterModel();
        JwtStatus jwtStatus = new JwtStatus();
        PinnacleDbContext db = new PinnacleDbContext();
        private readonly IAuthenticationService _service;
        private readonly JwtSettings jwtSettings;

        public AuthenticationController(JwtSettings jwtSettings, IAuthenticationService service)
        {
            this.jwtSettings = jwtSettings;
            _service = service;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginEntity entity)
        {
            try
            {
                var res = await _service.Login(entity);
                if (!res.status || res.data == null)
                    return Ok(new { status = false, IstokenExpired = false, message = res.message });

                var Token = Helpers.JWT.JwtHelpers.GenTokenkey(new TokenEntity
                {
                    UserName = res.data.UserName,
                    UserId = res.data.UserId,
                    Id = Convert.ToInt16(res.data.Id),
                    UserProfileId = Convert.ToInt16(res.data.UserProfileId),
                    RoleId = Convert.ToInt16(res.data.RoleId),
                    HospitalId = 0 // update this if needed
                }, jwtSettings);

                Response.Cookies.Append("refreshToken", res.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(1)
                });

                return Ok(new
                {
                    status = true,
                    IstokenExpired = false,
                    message = res.message,
                    data = new
                    {
                        user = res.data,
                        token = Token.Token
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok(new { status = "Error", message = "Failed to load data. Error: " + ex.Message });
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
                if (tokenStatus.data != null)
                {
                    jwtStatus = tokenStatus.data;
                    jwtStatus.HospitalId = Convert.ToInt32(Request.Headers["X-Hospital-Id"].ToString());
                }
                Ret res = tokenStatus.IstokenExpired == true ? tokenStatus : model.UpdatePassword(entity, jwtStatus);
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
        [HttpPost]
        [Route("refresh-token")]
        public IActionResult Refresh()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                    return Unauthorized();
                Ret res = model.RefreshToken(refreshToken);
                if (!res.status)
                    return Unauthorized();

                var newAccessToken = Helpers.JWT.JwtHelpers.GenTokenkey(new TokenEntity
                {
                    UserName = res.data.UserName,
                    UserId = res.data.UserId,
                    Id = Convert.ToInt16(res.data.Id),
                    UserProfileId = Convert.ToInt16(res.data.UserProfileId),
                    RoleId = Convert.ToInt16(res.data.RoleId),
                    HospitalId = 0 // update this if needed
                }, jwtSettings);

                var newrefreshToken = Helpers.JWT.JwtHelpers.RefreshToken();
                var Id = res.data.Id;
                var ExpiryDateTime = DateTime.Now.AddDays(1);
                model.UpdateRefreshToken(Id, newrefreshToken, ExpiryDateTime);
                return Ok(new
                {
                    status = true,
                    IstokenExpired = false,
                    message = res.message,
                    data = new
                    {
                        user = res.data,
                        token = newAccessToken
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok();
            }
        }
    }
}
