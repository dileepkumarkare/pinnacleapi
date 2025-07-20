using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class PincodeEntity
    {
        [Key]
        public int Id { get; set; }
        public int? Pincode { get; set; }
        public string? OfficeName { get; set; }
        public string? OfficeType { get; set; }
        public int? DistrictId { get; set; }
    }

    public class DistrictEntity
    {
        [Key]
        public int Id { get; set; }
        public string DistrictName { get; set; }
        public int StateId { get; set; }
    }
}
