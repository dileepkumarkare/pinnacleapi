using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class AuthenticationEntity
    {
    }
    public class LoginEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class UpdatePasswordEntity
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
    public class ServerClass
    {
        public string ServerName { get; set; }
        public string? Email { get; set; }
    }
    public class PasswordClass
    {
        public string? Token { get; set; }
        public string? Password { get; set; }

    }
    public class ModifyToken
    {
        public string? Token { get; set; }
        public string? UserProfileId { get; set; }

    }


}
