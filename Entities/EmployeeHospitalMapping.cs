using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class EmployeeHospitalMapping
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int HospitalId { get; set; }
        public string IsDelete { get; set; } = "No";

    }
}
