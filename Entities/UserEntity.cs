using Pinnacle.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pinnacle.Entities
{
    public class UserEntity
    {

        [Key]
        public int Id { get; set; }
        public int? TitleId { get; set; }
        public string? UserName { get; set; }
        public string? UserFullName { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string? Password { get; set; } = CommonLogic.Encrypt("Test@123");
        public string? ContactNo { get; set; }
        public string? Status { get; set; } = "Active";
        public int? AddedBy { get; set; }
        public int? OrganizationId { get; set; }
        public int? UserProfileId { get; set; }
        public string? UserId { get; set; }
        public int? RoleId { get; set; }
        public int? HospitalId { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? IsMedicalTestRequired { get; set; }
        [NotMapped]
        public string? TempEmpCode { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? PWDChangeDate { get; set; }
        public string? SystemName { get; set; }
    }
}
