namespace Pinnacle.Helpers.JWT
{
    public class JwtStatus
    {
        public int Id { get; set; }
        public int HospitalId { get; set; }
        public bool IsExpired { get; set; }
        public string? UserName { get; set; }
    }
}
