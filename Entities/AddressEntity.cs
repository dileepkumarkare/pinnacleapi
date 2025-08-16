using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class AddressEntity
    {

    }
    public class CountryEntity
    {
        [Key]
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string? DialCode { get; set; }
        public string? CurrencyName { get; set; }
        public string? CurrencyCode { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public int? Priority { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
    public class StateEntity
    {
        [Key]
        public int Id { get; set; }
        public string StateName { get; set; }
        public int CountryId { get; set; }
        public int? IsActive { get; set; }
        public int? AddedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
    }
    public class CityEntity
    {
        [Key]
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int StateId { get; set; }
        public int? IsActive { get; set; }
        public int? AddedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
    }
    [Keyless]
    public class Pagination
    {
        public string? SearchKey { get; set; } = String.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 0;
        public bool? AllKeys { get; set; } = false;
        public int? Id { get; set; } = 0;

    }
}
