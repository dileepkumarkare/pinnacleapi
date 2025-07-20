using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class EmployeeBankDetails
    {
        [Key]
        public int Id { get; set; }
        public int? EmpId { get; set; }
        public string? AccountType { get; set; }
        public string? AccNo { get; set; }
        public string? BankName { get; set; }
        public string? Branch { get; set; }
        public string? IFSCCode { get; set; }
        public string? IsPrimary { get; set; }
        public string? IsRemoved { get; set; }
        public int? RoleId { get; set; }
        public int? AddedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
