using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class OPCancellation
    {
        [Key]
        public int Id { get; set; }
        public int? OpConsId { get; set; }
        public string? Action { get; set; }
        public int? ActionBy { get; set; }
        public DateTime? ActionDate { get; set; }
        public int? Role { get; set; }
    }
}
