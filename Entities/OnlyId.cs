using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace Pinnacle.Entities
{
    public class OnlyId
    {
        public int Id { get; set; }
    }
    public class Search
    {
        public string? UMRNumber { get; set; }
        public int? PatientId { get; set; }
        public string? PatientType { get; set; }
        public int? DoctorId { get; set; } = 0;
        public int? OrganizationId { get; set; } = 0;
    }
    public class StatusUpdate
    {
        public int Id { get; set; }
        public string? Status { get; set; }
        public bool? IsFinalSubmit { get; set; }
    }


    public class CommonRes
    {
        public string Status { get; set; }
    }
    public class Profile
    {

        public int Id { get; set; }
        public IFormFile? Image { get; set; }

    }
    public class ImportFile
    {
        public IFormFile? File { get; set; }
    }

    [Keyless]
    public class EmployeeIdEntity
    {
        public string EmployeeTempId { get; set; }
    }
    [Keyless]
    public class EmployementType
    {
        public string MedicalTestRequired { get; set; }
    }
    [Keyless]
    public class DoctorProfileUpload
    {
        public int Id { get; set; }
        public IFormFile? DoctorProfile { get; set; }
    }
    public class PatientFilter
    {
        public int? PatientId { get; set; }
        public int? OrganizationId { get; set; }
    }
    public class UpdateStatus
    {
        public int Id { get; set; }
        public string IsActive { get; set; }
    }
}
