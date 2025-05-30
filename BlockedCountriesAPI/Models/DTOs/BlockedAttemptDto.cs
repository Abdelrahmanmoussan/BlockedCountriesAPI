namespace BlockedCountriesAPI.Models.DTOs
{
    public class BlockedAttemptDto
    {
        public string IpAddress { get; set; } = null!;
        public string CountryCode { get; set; } = null!;
        public bool IsBlocked { get; set; }
        public string UserAgent { get; set; } = null!;
        public DateTime Timestamp { get; set; }
    }

}
