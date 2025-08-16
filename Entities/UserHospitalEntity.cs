using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class UserHospitalEntity
    {
        [Key]
        public int Id { get; set; }
        public int? HospitalId { get; set; }
        public int? UserId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
