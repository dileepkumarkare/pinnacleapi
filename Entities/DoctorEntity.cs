using DevExpress.XtraRichEdit.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pinnacle.Entities
{
    public class DoctorEntity
    {

        [Key]
        public int DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public string? ConsultationType { get; set; }
        public string? DoctorType { get; set; }
        public string? ConsultingType { get; set; }
        [NotMapped]
        public string? DoctorCode { get; set; }
        public int? DesignationId { get; set; }
        public int? DepartmentId { get; set; }
        public string? RegistrationNo { get; set; }
        public int? SpecializationId1 { get; set; }
        public string? SpecializationId2 { get; set; }
        public string? DateOfJoining { get; set; }
        public string? RelieveDate { get; set; }
        public string? ResignedDate { get; set; }
        public string? PaymentType { get; set; }
        public string? Status { get; set; } = "Active";
        public int? ConsultationLimitPerDay { get; set; }
        public int? AdmissionLimit { get; set; }
        public string? Teleconsultation { get; set; }
        public string? IsHOD { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? VerifyBy { get; set; }
        public DateTime? VerifyDate { get; set; }
        public int? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string? DoctorProfile { get; set; }
        public string? Qualification { get; set; }
        public string? Title { get; set; }
        [NotMapped]
        public int? RoleId { get; set; }
        [NotMapped]
        public string? ContactNo { get; set; }
        [NotMapped]
        public string? Email { get; set; }
        [NotMapped]
        public string? Gender { get; set; }
        [NotMapped]
        public DateTime? DOB { get; set; }
        public int UserId { get; set; }
        public int? ReservedTokens { get; set; }
        public int? AssistantId { get; set; }
    }

    public class DoctorDetails
    {
        [Key]
        public int Id { get; set; }
        public int? DoctorId { get; set; }
        public string? BloodGroup { get; set; }
        public string? Religion { get; set; }
        public string? Nationality { get; set; }
        public string? MaritalStatus { get; set; }
        public string? RelationPrefix { get; set; }
        public string? RelationName { get; set; }
        public string? RelationMobileNo { get; set; }
        public string? AadharNo { get; set; }
        public string? PanNo { get; set; }
        public string? UanoNo { get; set; }
        public string? ESINo { get; set; }
        public string? Country { get; set; }
        public int? AreaCode { get; set; }
        public string? Area { get; set; }
        public string? Address { get; set; }
        public string? IdProof { get; set; }
        public string? IdNumber { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
    public class DoctorDetailsEntity
    {
        [Key]
        public int Id { get; set; }
        public int? DoctorId { get; set; }
        public string? BloodGroup { get; set; }
        public string? Religion { get; set; }
        public string? Nationality { get; set; }
        public string? MaritalStatus { get; set; }
        public string? RelationPrefixType { get; set; }
        public string? RelationName { get; set; }
        public string? RelationMobileNo { get; set; }
        public string? AadharNumber { get; set; }
        public string? PanNumber { get; set; }
        public string? UanNumber { get; set; }
        public string? ESINo { get; set; }
        public string? Country { get; set; }
        public int? AreaCode { get; set; }
        public string? Area { get; set; }
        public string? Address { get; set; }
        public string? IdProof { get; set; }
        public string? IdNumber { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string? AccountType { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? Branch { get; set; }
        public string? IfscCode { get; set; }
    }
    public class DoctorAddress
    {
        [Key]
        public int Id { get; set; }
        public int? DoctorId { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? BloodGroup { get; set; }
        public string? MaritalStatus { get; set; }
        public string? MarriageDate { get; set; }
        public int? AddressTypeId { get; set; }
        public string? Address { get; set; }
        public int? CityId { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }
        public string? TelephoneNumber { get; set; }
        public string? MobileNumber { get; set; }
        public string? Fax { get; set; }
        public string? Pan { get; set; }
        public string? AadharNo { get; set; }
        public string? DoctorEmail { get; set; }
        public int? PinCodeId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; } = DateTime.Now;
    }

    public class DoctorEducation
    {
        [Key]
        public int Id { get; set; }
        public int? DoctorId { get; set; }
        public string? DegreeName { get; set; }
        public string? CourseType { get; set; }
        public int? SpecializationId { get; set; }
        public string? University { get; set; }
        public string? Country { get; set; }
        public int? YearofPass { get; set; }
        public string? MedicalCouncilName { get; set; }
        public string? CertificateName { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string? RegistrationNumber { get; set; }
    }
    public class DoctorEducationUpload
    {
        [Key]
        public int id { get; set; }
        public int? doctorId { get; set; }
        public string? degreeName { get; set; }
        public string? courseType { get; set; }
        public int? specializationId { get; set; }
        public string? university { get; set; }
        public string? country { get; set; }
        public int? yearofPass { get; set; }
        public string? medicalCouncilName { get; set; }
        public IFormFile? certificate { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdDate { get; set; } = DateTime.Now;
        public int? modifyBy { get; set; }
        public DateTime? modifyDate { get; set; }
        public string? RegistrationNumber { get; set; }
    }
    public class DoctorExperience
    {
        [Key]
        public int Id { get; set; }
        public int? DoctorId { get; set; }
        public string? Designation { get; set; }
        public int? SpecializationId { get; set; }
        public string? HospitalName { get; set; }
        public string? Location { get; set; }
        public string? EmploymentType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CertificateName { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
    public class DoctorExperienceUpload
    {
        [Key]
        public int id { get; set; }
        public int? doctorId { get; set; }
        public string? designation { get; set; }
        public int? specializationId { get; set; }
        public string? hospitalName { get; set; }
        public string? location { get; set; }
        public string? employmentType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? endDate { get; set; }
        public IFormFile? certificate { get; set; }

    }

    public class DoctorCharges
    {
        [Key]
        public int ChargeId { get; set; }
        public string? IPorOP { get; set; }
        public string? VisitType { get; set; }
        public int? DoctorId { get; set; }
        public decimal? Charge { get; set; }
        public DateTime? EffectFrom { get; set; }
        public DateTime? EffectTo { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [NotMapped]
        public DateTime? ConsultationDate { get; set; }
        [NotMapped]
        public string? PaymentBy { get; set; }
        [NotMapped]
        public int? OrganizationId { get; set; }
        [NotMapped]
        public int? Visit { get; set; }
    }
}
