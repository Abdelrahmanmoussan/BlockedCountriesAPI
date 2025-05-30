namespace BlockedCountriesAPI.Models
{
    public class IpInfo
    {
        [JsonProperty("ip")]
        public string? Ip { get; set; }

        [JsonProperty("continent_name")]
        public string? Continent { get; set; }

        [JsonProperty("country_name")]
        public string? Country { get; set; }

        [JsonProperty("country_code2")]
        public string? CountryCode { get; set; }

        [JsonProperty("state_prov")]
        public string? State { get; set; }

        [JsonProperty("city")]
        public string? City { get; set; }

        [JsonProperty("isp")]
        public string? Isp { get; set; }
    }
}
