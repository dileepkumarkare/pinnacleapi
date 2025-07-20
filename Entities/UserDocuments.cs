using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class UserDocuments
    {
        [Key]
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? UserType { get; set; }
        public string? DocType { get; set; }
        public string? DocName { get; set; }
        public string? DocOriginalName { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
    }
    public class UserDocUpload
    {
        public int? userId { get; set; }
        public int? userType { get; set; }
        public string? docType { get; set; }
        public IFormFile? document { get; set; }
    }
}
