namespace BlockedCountriesAPI.Services
{
    public class IpLookupService : IIpLookupService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;


        public IpLookupService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<IpInfo?> GetIpInfoAsync(string ipAddress)
        {
            var baseUrl = _configuration["IpApiSettings:BaseUrl"];
            var apiKey = _configuration["IpApiSettings:ApiKey"];

            var url = $"{baseUrl}?apiKey={apiKey}&ip={ipAddress}";

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Response content: {content}");

            if (!response.IsSuccessStatusCode)
                return null;

            var ipInfo = JsonConvert.DeserializeObject<IpInfo>(content);
            return ipInfo;
        }
    }
}

