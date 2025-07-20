using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace Pinnacle.Entities
{
    public class LabTestParametersEntity
    {
        [Key]
        public int Id { get; set; }
        public string? ParamCode { get; set; }
        public string? ShortName { get; set; }
        public int? HospitalId { get; set; }
        public string? ParamDesc { get; set; }
        public string? Method { get; set; }
        public string? IsIncAntiBiotics { get; set; }
        public string? Units { get; set; }
        public string? TextSize { get; set; }
        public string? ParamDisplay { get; set; }
        public string? IsAccreditationNeed { get; set; }
        public string? IsMultipleValues { get; set; }
        public string? IsNewIOrganism { get; set; }
        public string? IsNormalRange { get; set; }
        public string? IsCriticalRange { get; set; }
        public string? IsDefaultDRange { get; set; }
        public string? Remarks { get; set; }
        public string? Notes { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? LabGroupId { get; set; }
        public List<ParameterRange>? ParameterRanges { get; set; }
    }
    public class ParameterRange
    {
        [Key]
        public int Id { get; set; }
        public int? ParamId { get; set; }
        public string? Gender { get; set; }
        public int? MinAge { get; set; }
        public string? MinAgeUOM { get; set; }
        public int? MaxAge { get; set; }
        public string? MaxAgeUOM { get; set; }
        public string? Description { get; set; }
        public string? NSymbol { get; set; }
        public string? NMinRange { get; set; }
        public string? NMaxRange { get; set; }
        public string? NUnits { get; set; }
        public string? NormalRange { get; set; }
        public string? CMinRange { get; set; }
        public string? CMaxRange { get; set; }
        
        public string? CSymbol { get; set; }
        public string? DMinRange { get; set; }
        public string? DMaxRange { get; set; }
    }
}
