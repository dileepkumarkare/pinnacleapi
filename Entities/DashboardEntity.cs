namespace Pinnacle.Entities
{
    public class DashboardEntity
    {
        public DateTime? Date { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string? FilterType { get; set; }
    }
    public class OPConsultationBillingFilter
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
