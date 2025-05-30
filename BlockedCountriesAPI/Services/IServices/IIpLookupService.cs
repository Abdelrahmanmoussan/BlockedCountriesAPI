namespace BlockedCountriesAPI.Services.IServices
{
    public interface IIpLookupService
    {

        Task<IpInfo?> GetIpInfoAsync(string ipAddress);
    }
}
