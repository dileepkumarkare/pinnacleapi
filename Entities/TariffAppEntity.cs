using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class TariffAppEntity
    {
        [Key]
        public int Id { get; set; }
        public string TariffAppCode { get; set; }
        public string TariffAppName { get; set; }
        public int HospitalId {  get; set; }
        public string IsActive { get; set; } = "Yes";
        public int AddedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int UpdateBy { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
