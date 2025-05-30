namespace BlockedCountriesAPI.Controllers
{
    [ApiController]
    [Route("api/countries")]
    public class CountriesController : ControllerBase
    {
        private readonly IBlockedCountryRepository _repository;
        private readonly IIpLookupService _ipLookupService;

        public CountriesController(IBlockedCountryRepository repository, IIpLookupService ipLookupService)
        {
            _repository = repository;
            _ipLookupService = ipLookupService;
        }



        [HttpPost("block")]
        public async Task<IActionResult> BlockCountry([FromQuery] string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Length != 2)
                return BadRequest("Invalid country code. It must be 2 characters.");

            countryCode = countryCode.ToLower();

            if (_repository.Exists(countryCode))
                return Conflict("Country is already blocked.");

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://restcountries.com/v3.1/alpha/{countryCode}");

            if (!response.IsSuccessStatusCode)
                return NotFound("Invalid country code or failed to retrieve country name.");

            var json = await response.Content.ReadAsStringAsync();
            var data = System.Text.Json.JsonSerializer.Deserialize<List<RestCountry>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null || data.Count == 0)
                return NotFound("Country not found.");

            var countryName = data[0].Name.Common;

            var success = _repository.AddCountry(countryCode, countryName);

            if (!success)
                return StatusCode(500, "Failed to block country.");

            return Ok(new { Message = $"CountryName {countryName} ({countryCode.ToUpper()}) blocked successfully." });
        }

        [HttpPost("temporal-block")]
        public async Task<IActionResult> BlockTemporarily([FromQuery] TemporalBlockRequest request)
        {
            var code = request.CountryCode?.ToUpper();

            if (string.IsNullOrWhiteSpace(code) || code.Length != 2)
                return BadRequest("Invalid country code.");

            if (request.DurationMinutes < 1 || request.DurationMinutes > 1440)
                return BadRequest("Duration must be between 1 and 1440 minutes.");

            if (_repository.IsTemporarilyBlocked(code))
                return Conflict("Country is already temporarily blocked.");

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://restcountries.com/v3.1/alpha/{code}");

            if (!response.IsSuccessStatusCode)
                return NotFound("Invalid country code or failed to retrieve country name.");

            var json = await response.Content.ReadAsStringAsync();
            var data = System.Text.Json.JsonSerializer.Deserialize<List<RestCountry>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null || data.Count == 0)
                return NotFound("Country not found.");

            var countryName = data[0].Name.Common;

            var expiresAt = DateTime.UtcNow.AddMinutes(request.DurationMinutes);
            var success = _repository.AddTemporalBlock(code, countryName, expiresAt);

            if (!success)
                return Conflict("Country already blocked.");

            return Ok(new
            {
                Message = $"Country {countryName} ({code}) temporarily blocked for {request.DurationMinutes} minutes.",
                ExpiresAt = expiresAt
            });
        }

        [HttpGet("blocked")]
        public IActionResult GetBlockedCountries([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var all = _repository.GetAll();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                all = all.Where(c =>
                    c.CountryCode.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.CountryName.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            var total = all.Count();
            var items = all
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new PaginatedResult<BlockedCountry>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            });
        }

        [HttpDelete("block/{countryCode}")]
        public IActionResult UnblockCountry(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Length != 2)
                return BadRequest("Invalid country code.");

            if (!_repository.Exists(countryCode))
                return NotFound("Country is not in the blocked list.");

            _repository.RemoveCountry(countryCode);
            return Ok(new { Message = $"Country {countryCode.ToUpper()} unblocked successfully." });
        }

        

    }
}