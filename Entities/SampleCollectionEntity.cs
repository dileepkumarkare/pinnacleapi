using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pinnacle.Entities
{
    public class SampleCollectionEntity
    {
        public int Id { get; set; }
        public string? PatientType { get; set; }
        public string? BillNo { get; set; }
        public string? BillId { get; set; }
        public int? NoOfBarcodes { get; set; }
        public string? IsUrgent { get; set; }
        public string? TransNo { get; set; }
        public DateTime? TransDate { get; set; }
        public string? LabNo { get; set; }
        public int? PhlebotomistId { get; set; }
        public string? ClinicalHistory { get; set; }
        public string? Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        [NotMapped]
        public ICollection<SampleCollectionTest>? SampleCollectionTests { get; set; }
    }
    public class SampleCollectionTest
    {
        public int Id { get; set; }
        public int? SampleCollId { get; set; }
        public int? TestId { get; set; }
        public string? IsCollected { get; set; }
        public string? IsTested { get; set; }
        public string? IsRejected { get; set; }
        public string? IsRecollected { get; set; }
        public string? IsUrgent { get; set; }
        public string? IsPriority { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string? Remarks { get; set; }
    }
    public class SampleCollectionSearch
    {
        public int Id { get; set; }
        public string? BillNo { get; set; }
    }
}
