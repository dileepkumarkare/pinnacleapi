using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace Pinnacle.Entities
{
    public class ServiceEntity
    {
        [Key]
        public int Id { get; set; }
        public string? ServiceCode { get; set; }
        public string? ServiceType { get; set; }
        public string? ServiceName { get; set; }
        public int? ServiceGroupId { get; set; }
        public decimal? Charge { get; set; }
        public int? BillingHeadId { get; set; }
        public string? IsActive { get; set; } = "Yes";
        public string? IsPackage { get; set; }
        public string? IsProcedure { get; set; }
        public string? IsDiet { get; set; }
        public string? IsOutSide { get; set; }
        public string? IsSampleNeeded { get; set; }
        public string? IsConsentSlipNeeded { get; set; }
        public string? ApplicableFor { get; set; }
        public int? HospitalId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? IsFavorite { get; set; }
    }
    public class DoctorFavMedicineServices
    {
        [Key]
        public int Id { get; set; }
        public int? ServiceId { get; set; }
        public string? IsFavorite { get; set; }
        public int? DoctorId { get; set; }
        public int? Type { get; set; }
    }
}
