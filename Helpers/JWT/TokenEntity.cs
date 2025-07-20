namespace Pinnacle.Helpers.JWT
{
    public class TokenEntity
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public TimeSpan Validaty { get; set; }
        public string RefreshToken { get; set; }
        public int Id { get; set; } 
        public int OrganizationId { get; set; }
        public int UserProfileId { get; set; } 
        public string Email { get; set; }
        public string UserId { get; set; }
        public int RoleId { get; set; }
         public int HospitalId { get; set; }
        public DateTime ExpiredTime { get; set; }
   
    }

    public class TokenList
    {
        public string Id { get; set; } 
        public string Email { get; set; }
        public string UserId { get; set; }
        public string? UserName { get; set; }
        //public DateTime? ExpiredTime { get; set; }
        public bool? IstokenExpired { get; set; }
        public string OrganizationId { get; set; }
        public string UserProfileId { get; set; }
        public string RoleId { get; set; }
        public string HospitalId { get; set; }

    }
}
