using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Pinnacle.Entities;
using System.Data;
 

namespace Pinnacle.Helpers.JWT
{
    public static class JwtHelpers
    {
        public static IEnumerable<Claim> GetClaims(this TokenEntity userAccounts)
        {
            IEnumerable<Claim> claims = new Claim[] {
                new Claim("Id", userAccounts.Id.ToString()),
                    new Claim("userName", userAccounts.UserName),
                    //new Claim("email", userAccounts.Email),
                     new Claim("organizationId", userAccounts.OrganizationId.ToString()),
                    new Claim("Expiration", DateTime.UtcNow.AddHours(10).ToString()),
                     new Claim("UserProfileId", userAccounts.UserProfileId.ToString()),
                     new Claim("RoleId", userAccounts.RoleId.ToString()),
                      new Claim("HospitalId", userAccounts.HospitalId.ToString())
                   

                    //new Claim("Expiration", DateTime.UtcNow.AddDays(1).ToString())
                    //new Claim("Expiration", DateTime.UtcNow.ToString())
                     //new Claim("Expiration", DateTime.UtcNow.AddDays(1).ToString("MMM ddd dd yyyy HH:mm:ss tt"))
            };

            return claims;
        }

        public static IEnumerable<Claim> GetClaims(this TokenEntity userAccounts, out Guid Id)
        {
            Id = Guid.NewGuid();
            return GetClaims(userAccounts);
        }

        public static TokenEntity GenTokenkey(TokenEntity model, JwtSettings jwtSettings)
        {
            try
            {
                var UserToken = new TokenEntity();
                if (model == null) throw new ArgumentException(nameof(model));
                // Get secret key
                var key = System.Text.Encoding.ASCII.GetBytes(jwtSettings.IssuerSigningKey);
                Guid Id = Guid.Empty;
                DateTime expireTime = DateTime.Now.AddHours(10); 
                //DateTime expireTime = DateTime.UtcNow.AddMinutes(5*60+31); //expire in 1 minute
                //DateTime expireTime = DateTime.UtcNow.AddMonths(2).AddMinutes(5 * 60 + 30); //expire in 1 our
                UserToken.Validaty = expireTime.TimeOfDay;
                var JWToken = new JwtSecurityToken(issuer: jwtSettings.ValidIssuer, audience: jwtSettings.ValidAudience, claims: GetClaims(model, out Id), notBefore: new DateTimeOffset(DateTime.Now).DateTime, expires: new DateTimeOffset(expireTime).DateTime, signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));
                UserToken.Token = new JwtSecurityTokenHandler().WriteToken(JWToken);
                UserToken.UserName = model.UserName;
                //UserToken.UserId = model.UserId;
                UserToken.Id = model.Id;
                UserToken.OrganizationId = model.OrganizationId;
                UserToken.HospitalId = model.HospitalId;
                return UserToken;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
