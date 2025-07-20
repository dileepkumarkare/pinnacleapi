using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Pinnacle.Entities
{
    public class PinnacleDbContext : DbContext
    {
        public virtual DbSet<ConfigEntity> Config { get; set; }
        public virtual DbSet<UserEntity> Users { get; set; }
        public virtual DbSet<StateEntity> States { get; set; }
        public virtual DbSet<CountryEntity> Countries { get; set; }
        public virtual DbSet<CityEntity> Cities { get; set; }
        public virtual DbSet<DesignationEntity> Designation { get; set; }
        public virtual DbSet<DepartmentEntity> Department { get; set; }
        public virtual DbSet<TitleEntity> TitleMaster { get; set; }
        public virtual DbSet<HospitalEntity> Hospital { get; set; }
        public virtual DbSet<EmployeeIdEntity> PKG_GenerateEmployeeId { get; set; }
        public virtual DbSet<EmployeeEntity> Employee { get; set; }
        public virtual DbSet<EmployeeAddress> EmployeeAddress { get; set; }
        public virtual DbSet<EmployeeEducation> EmployeeEducation { get; set; }
        public virtual DbSet<EmployeeExperience> EmployeeExperience { get; set; }
        public virtual DbSet<DocumentsEntity> EmployeeDocuments { get; set; }
        public virtual DbSet<DoctorEntity> Doctors { get; set; }
        public virtual DbSet<SpecializationEntity> Specializations { get; set; }
        public virtual DbSet<PatientEntity> Patient { get; set; }
        public virtual DbSet<DoctorAddress> DoctorPersonalDetails { get; set; }
        public virtual DbSet<DoctorEducation> DoctorEducation { get; set; }
        public virtual DbSet<DoctorExperience> DoctorExperience { get; set; }
        public virtual DbSet<TariffEntity> Tariff { get; set; }
        public virtual DbSet<ReligionEntity> Religion { get; set; }
        public virtual DbSet<PatientRegistration> PatientRegistartion { get; set; }
        public virtual DbSet<OccupationEntity> Occupation { get; set; }

        public virtual DbSet<ReferralEntity> ReferralMaster { get; set; }
        public virtual DbSet<ReferralPercentage> ReferralPercentage { get; set; }

        public virtual DbSet<EmployeeHospitalMapping> EmployeeHospitalMapping { get; set; }
        public virtual DbSet<OrganizationEntity> Organization { get; set; }
        public virtual DbSet<OrganizationTariff> OrganizationTariff { get; set; }

        public virtual DbSet<PincodeEntity> PincodeData { get; set; }
        public virtual DbSet<DistrictEntity> District { get; set; }
        public virtual DbSet<TariffAppEntity> TariffApp { get; set; }
        public virtual DbSet<WardGroupEntity> WardGroup { get; set; }
        public virtual DbSet<DoctorCharges> DoctorCharges { get; set; }

        public virtual DbSet<ServiceEntity> Services { get; set; }
        public virtual DbSet<ServiceGroupEntity> ServiceGroup { get; set; }
        public virtual DbSet<LabReportingSettings> LabReportingSettings { get; set; }
        public virtual DbSet<TariffServiceMapping> TariffServiceMapping { get; set; }
        public virtual DbSet<BillingHeaderEntity> BillingHeader { get; set; }
        public virtual DbSet<OrganizationChargeEntity> OrganizationCharges { get; set; }
        public virtual DbSet<PatientAddress> PatientAddress { get; set; }
        public virtual DbSet<PatientReceiptDetailsEntity> PatientReceiptDetails { get; set; }
        public virtual DbSet<OpConsultationEntity> OpConsultation { get; set; }
        public virtual DbSet<OPBillingEntity> OPBilling { get; set; }
        public virtual DbSet<OSPatientEntity> OSPatient { get; set; }
        public virtual DbSet<OPServiceBookingEntity> OPServiceBooking { get; set; }
        public virtual DbSet<OPBillReceiptEntity> OPBillReceipt { get; set; }
        public virtual DbSet<CorporateRegistrationEntity> CorporateRegistration { get; set; }
        public virtual DbSet<CoLetterDetailsEntity> CoLetterDetails { get; set; }
        public virtual DbSet<AuditLog> AuditLog { get; set; }
        public virtual DbSet<BankDetailsEntity> BankDetails { get; set; }
        public virtual DbSet<ItemMasterEntity> ItemMaster { get; set; }
        public virtual DbSet<DoctorDetails> DoctorDetails { get; set; }
        public virtual DbSet<EmployeeBankDetails> EmployeeBankDetails { get; set; }
        public virtual DbSet<OrganizationAddress> OrganizationAddress { get; set; }
        public virtual DbSet<DoctorFavMedicineServices> DoctorFavMedicineServices { get; set; }
        public virtual DbSet<DoctorPrescription> DoctorPrescription { get; set; }
        public virtual DbSet<BankMaster> BankMaster { get; set; }
        public virtual DbSet<ItemGenericEntity> ItemGenericNames { get; set; }
        public virtual DbSet<UserDocuments> UserDocuments { get; set; }
        public virtual DbSet<LabTestParametersEntity> LabTestParameters { get; set; }
        public virtual DbSet<ParameterRange> ParameterRange { get; set; }
        public virtual DbSet<TestFormatEntity> TestFormat { get; set; }
        public virtual DbSet<TestParameterMapping> TestParameterMapping { get; set; }
        public virtual DbSet<SpecimenEntity> Specimen { get; set; }
        public virtual DbSet<VacutainerEntity> Vacutainer { get; set; }
        public virtual DbSet<UserGroupEntity> UserGroup { get; set; }
        public virtual DbSet<ItemFormCodesEntity> ItemFormCodes { get; set; }
        public virtual DbSet<SampleCollectionEntity> SampleCollection { get; set; }
        public virtual DbSet<SampleCollectionTest> SampleCollectionTests { get; set; }
        public virtual DbSet<UserProfileEntity> UserProfile { get; set; }
        public virtual DbSet<MedicineOnHandStockEntity> MedicineOnHandStock { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
            object value = optionsBuilder.UseSqlServer(configuration.GetConnectionString("IdentityConnection"), options => options.EnableRetryOnFailure());
        }

        public string GetConnectionString()
        {
            var connStr = this.Database.GetDbConnection().ConnectionString;
            return connStr;
        }
    }
}
