using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class UserProfileEntity
    {
        [Key]
        public int UserProfileId { get; set; }
        public int? UserGroupId { get; set; }
        public string? UserProfileName { get; set; }
        public string? UserProfileCode { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifyDate { get; set; }
        public int? DepartmentId { get; set; }
        public string? IsActive { get; set; }
        public int? ModifyBy { get; set; }
        public int? VerifyBy { get; set; }
        public DateTime? VerifyDate { get; set; }
        public int? CancelBy { get; set; }
        public DateTime? CancelDate { get; set; }
        public int? DaysToRestrict { get; set; }
        public int? HospitalId { get; set; }
    }
}
