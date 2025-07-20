using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class UserGroupEntity
    {
        [Key]
        public int UserGroupId { get; set; }
        public string? UserGroupName { get; set; }
        public string? UserGroupCode { get; set; }
        public int? DepartmentId { get; set; }
        public int? HospitalId { get; set; }
        public string? IsActive { get; set; } = "Yes";
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
