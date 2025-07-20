using System.ComponentModel.DataAnnotations;

namespace Pinnacle.Entities
{
    public class MedicineOnHandStockEntity
    {
        [Key]
        public int Id { get; set; }
        public string? ONHANDQTY { get; set; }
        public string? ITEMCD { get; set; }
    }
}
