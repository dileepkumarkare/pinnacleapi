using DevExpress.XtraRichEdit.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pinnacle.Entities
{
    public class SampleReceiveEntity
    {
        [Key]
        public int Id { get; set; }
        public int SCId { get; set; }
        public int? SCTID { get; set; }
        public string? IsReceived { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        [NotMapped]
        public List<SampleReceiveCollection>? SampleReceivedTests { get; set; }
    }
    public class SampleReceiveCollection
    {
        public int? SCTID { get; set; }
        public string? IsReceived { get; set; }

    }
}
