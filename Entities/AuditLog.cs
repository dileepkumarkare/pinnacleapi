using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string Method { get; set; }
        public string PrevData { get; set; }
        public string IpAddress { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

}
