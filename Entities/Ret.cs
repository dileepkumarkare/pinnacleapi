using DevExpress.Charts.Native;
using System.Numerics;

namespace Pinnacle.Entities
{
    public class Ret
    {
        public dynamic status { get; set; }
        public bool? IstokenExpired { get; set; }
        public dynamic? data { get; set; }
        public string? message { get; set; }
        public int? totalCount { get; set; }
        public string? RefreshToken { get; set; }
    }
}
