namespace BlockedCountriesAPI.Repositories.IRepositories
{
    public interface IBlockedCountryRepository
    {
        public bool AddCountry(string code, string countryName);
        public bool RemoveCountry(string code);
        public IEnumerable<BlockedCountry> GetAll();
        public bool Exists(string code);

        bool AddTemporalBlock(string countryCode, string countryName, DateTime expiresAt);
        bool IsTemporarilyBlocked(string countryCode);
        void RemoveExpiredBlocks();
    }
}
