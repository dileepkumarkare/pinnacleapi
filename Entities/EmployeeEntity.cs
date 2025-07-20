using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pinnacle.Entities
{
    public class EmployeeEntity
    {

        [Key]
        public int EmpId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeType { get; set; }

        public string? DateOfJoining { get; set; }
        public int? ReligionId { get; set; }
        public string? MaritalStatus { get; set; }
        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }
        public int UserId { get; set; }

        //public string? MedicalTestRequired { get; set; }
        public string? TempEmpCode { get; set; }
        public string? DateofInitiation { get; set; }
        public string? UserIdRequired { get; set; }
        public string? ProfileMaintain { get; set; }
        public string? Qualification { get; set; }
        public string? PreviousExperience { get; set; }
        public string? BloodGroups { get; set; }
        public string? Nationality { get; set; }
        public string? Caste { get; set; }
        public string? EmployeelivingType { get; set; }
        public string? RelationPrefixType { get; set; }
        public string? RelationName { get; set; }
        public string? PanNumber { get; set; }
        public string? UanNumber { get; set; }
        public string? AadharNumber { get; set; }
        public string? AccountType { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? PfNumber { get; set; }
        public string? BranchCode { get; set; }
        public string? IFSCCode { get; set; }
        public string? EmpProfile { get; set; }
        public string? IsMedicalCheckUpStatus { get; set; }
        public string? IsSystemNeedAccess { get; set; }
        public int? JobNatureId { get; set; }
        public string? EmployeeCadre { get; set; }
        public int? SubDepartmentId { get; set; }
        public int? CostDeptId { get; set; }
        public string? EmployeeCategory { get; set; }
        public string? EmpShift { get; set; }
        public string? WeekOff { get; set; }
        public string? PaymentMode { get; set; }
        public string? BiometricrefNo { get; set; }
        public string? MemberId { get; set; }
        public decimal? Salary { get; set; }
        public decimal? Stipend { get; set; }
        public string? EmpStatus { get; set; }
        public DateTime? TerminatedDate { get; set; }
        public DateTime? ResignedDate { get; set; }
        public DateTime? RelievedDate { get; set; }
    }

    public class EmployeeDetails
    {
        public int EmpId { get; set; }
        public string? UserFullName { get; set; }
        public DateTime? DOB { get; set; }
        public string? Gender { get; set; }
        public string? ContactNo { get; set; }
        public string? Email { get; set; }
        public int? TitleId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? DateOfJoining { get; set; }
        public int? DepartmentId { get; set; }
        public int? SubDepartmentId { get; set; }
        public int? CostDeptId { get; set; }
        public int? DesignationId { get; set; }
        public string? EmployeeType { get; set; }
        public string? EmployeeCadre { get; set; }
        public string? EmployeeCategory { get; set; }
        public string? EmpShift { get; set; }
        public string? WeekOff { get; set; }
        public string? PaymentMode { get; set; }
        public string? BiometricrefNo { get; set; }
        public string? MemberId { get; set; }
        public string? EmployeelivingType { get; set; }
        public decimal? Salary { get; set; }
        public decimal? Stipend { get; set; }
        public string? EmpStatus { get; set; }
        public DateTime? TerminatedDate { get; set; }
        public DateTime? ResignedDate { get; set; }
        public DateTime? RelievedDate { get; set; }
    }
    public class EmployeeEducation
    {
        [Key]
        public int Id { get; set; }
        public int EmpId { get; set; }
        public string? University { get; set; }
        public string? Institution { get; set; }
        public string? Degree { get; set; }
        public string? Grade { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
    }
    public class EmployeeExperience
    {
        [Key]
        public int Id { get; set; }
        public int EmpId { get; set; }
        public string? JobTitle { get; set; }
        public string? HospitalName { get; set; }
        public string? EmploymentType { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
    }

    public class EmployeeAddress
    {
        [Key]

        public int Id { get; set; }
        public int? EmpId { get; set; }
        public int? AddressType { get; set; }
        public string? Address { get; set; }
        public int? City { get; set; }
        public int? State { get; set; }
        public int? Country { get; set; }
        public string? TelephoneNumber { get; set; }
        public string? PinCode { get; set; }
        public string? MobileNumber { get; set; }
        public string? Fax { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPersonMobile { get; set; }
        public string? ContactPersonEmail { get; set; }
        public int? TempAddressType { get; set; }
        public string? TempAddress { get; set; }
        public int? TempCity { get; set; }
        public int? TempState { get; set; }
        public int TempCountry { get; set; }
        public string? TempTelephoneNumber { get; set; }
        public string? TempPinCode { get; set; }
        public string? TempMobileNumber { get; set; }
        public string? TempFax { get; set; }
        public string? TempContactPerson { get; set; }
        public string? TempContactPersonMobile { get; set; }
        public string? TempContactPersonEmail { get; set; }
        public string? IsSame { get; set; }
        public int? AddedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
    }
    public class DocumentsEntity
    {
        [Key]
        public int Id { get; set; }
        public int? EmpId { get; set; }
        public string? DocumentName { get; set; }
        public string? FileDescription { get; set; }
    }

    [Keyless]
    public class FileUploadEntity
    {
        public int Id { get; set; }
        public IFormFile? DocumentName { get; set; }
        public string? FileDescription { get; set; }
    }
    public class ProfilePhoto
    {
        public int Id { get; set; }
        public IFormFile? EmpProfile { get; set; }
    }
}
