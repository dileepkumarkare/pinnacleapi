using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class BankDetailsEntity
    {
        [Key]
        public int Id { get; set; }
        public int EmpId { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public string IFSCCode { get; set; }
        public string AccNo { get; set; }
        public string IsPrimary { get; set; }
        public string IsRemoved { get; set; }
        public int AddedBy { get; set; }
        public DateTime CreatedDate = DateTime.Now;
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
    public class BankMaster
    {
        [Key]
        public int Id { get; set; }
        public string? BankCode { get; set; }
        public string? BankName { get; set; }
        public string? BranchName { get; set; }
        public int? HospitalId { get; set; }
        public string? IFSCCode { get; set; }
        public string? MICRCode { get; set; }
        public string? BSRCode { get; set; }
        public string? Address { get; set; }
        public string? IsRequiredTrans { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
