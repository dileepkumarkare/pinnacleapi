using Microsoft.AspNetCore.Routing.Constraints;
using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class WardGroupEntity
    {
        [Key]
        public int Id { get; set; }
        public string? WardGroupCode { get; set; }
        public string WardGroupName { get; set; }
        public int TariffIdAppId { get; set; }
        public int HospitalId {  get; set; }
        public string IsActive { get; set; } = "Yes";
        public int AddedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
