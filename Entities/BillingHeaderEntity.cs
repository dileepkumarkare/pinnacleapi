using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class BillingHeaderEntity
    {
        [Key]
        public int BillingHeaderId { get; set; }
        public string? BillingHeaderName { get; set; }
        public string? ServiceType { get; set; }
        public int? HospitalId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
