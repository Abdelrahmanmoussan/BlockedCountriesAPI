using System.Net;

namespace BlockedCountriesAPI.Controllers
{
    [ApiController]
    [Route("api/ip")]
    public class IpController : ControllerBase
    {
        private readonly IIpLookupService _ipLookupService;
        private readonly IBlockedAttemptsRepository _blockedAttemptsRepository;
        private readonly IBlockedCountryRepository _repository;

        public IpController(IIpLookupService ipLookupService,IBlockedAttemptsRepository blockedAttemptsRepository, IBlockedCountryRepository repository)
        {
            _ipLookupService = ipLookupService;
            _blockedAttemptsRepository = blockedAttemptsRepository;
            _repository = repository;
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup([FromQuery] string? ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            }

            if (string.IsNullOrWhiteSpace(ipAddress))
                return BadRequest("Unable to resolve IP address.");

            if (!IPAddress.TryParse(ipAddress, out _))
                return BadRequest("Invalid IP format.");

            var result = await _ipLookupService.GetIpInfoAsync(ipAddress);
            if (result == null)
                return StatusCode(500, "Failed to retrieve IP info.");

            return Ok(result);
        }


        [HttpGet("check-block")]
        public async Task<IActionResult> CheckBlock([FromQuery] string? ip)
        {

            //Comment this line because i using LocalHost and this line not support api online 
            //var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var ipAddress = ip;


            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ipAddress))
                return BadRequest("Unable to resolve caller IP address.");

           
            if (ipAddress == "::1" || ipAddress == "127.0.0.1")
            {
                ipAddress = "8.8.8.8";
            }



            if (string.IsNullOrEmpty(ipAddress))
                return BadRequest("Unable to resolve caller IP address.");


            var ipInfo = await _ipLookupService.GetIpInfoAsync(ipAddress);
            if (ipInfo == null)
                return StatusCode(500, "Failed to retrieve IP info.");

            bool isBlocked = _repository.Exists(ipInfo.CountryCode);

            var userAgent = Request.Headers["User-Agent"].ToString();
            _blockedAttemptsRepository.AddAttempt(new BlockedAttempt
            {
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow,
                CountryCode = ipInfo.CountryCode,
                IsBlocked = isBlocked,
                UserAgent = userAgent
            });

            return Ok(new BlockedAttemptDto
            {
                IpAddress = ipAddress,
                CountryCode = ipInfo.CountryCode,
                IsBlocked = isBlocked,
                UserAgent = userAgent,
                Timestamp = DateTime.UtcNow
            });

        }

        [HttpGet("blocked-attempts")]

        public IActionResult GetAllAttempts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var all = _blockedAttemptsRepository.GetAttempts().ToList();
            var total = all.Count;
            var paged = all.Skip((page - 1) * pageSize).Take(pageSize);

            return Ok(new PaginatedResult<BlockedAttempt>
            {
                Items = paged,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            });
        }









    }
}
