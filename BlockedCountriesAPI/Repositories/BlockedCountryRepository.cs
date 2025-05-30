namespace BlockedCountriesAPI.Repositories
{
    public class BlockedCountryRepository : IBlockedCountryRepository
    {
        private readonly ConcurrentDictionary<string, BlockedCountry> _blockedCountries = new();

        public bool AddCountry(string code,  string countryName)
        {
            return _blockedCountries.TryAdd(code.ToUpper(), new BlockedCountry { CountryCode = code.ToUpper(), BlockedAt = DateTime.Now, CountryName = countryName });
        }

        public bool RemoveCountry(string code)
        {
            return _blockedCountries.TryRemove(code.ToUpper(), out _);
        }

        public IEnumerable<BlockedCountry> GetAll()
        {
            return _blockedCountries.Values;
        }

        public bool Exists(string code)
        {
            return _blockedCountries.ContainsKey(code.ToUpper());
        }

        public bool AddTemporalBlock(string countryCode, string countryName, DateTime expiresAt)
        {
            return _blockedCountries.TryAdd(countryCode.ToUpper(), new BlockedCountry
            {
                CountryCode = countryCode.ToUpper(),
                CountryName = countryName,
                BlockedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt
            });
        }

        public bool IsTemporarilyBlocked(string countryCode)
        {
            if (_blockedCountries.TryGetValue(countryCode.ToUpper(), out var block))
            {
                if (DateTime.UtcNow < block.ExpiresAt)
                    return true;

                _blockedCountries.TryRemove(countryCode.ToUpper(), out _); 
            }

            return false;
        }

        public void RemoveExpiredBlocks()
        {
            foreach (var item in _blockedCountries)
            {
                if (item.Value.ExpiresAt <= DateTime.UtcNow)
                {
                    _blockedCountries.TryRemove(item.Key, out _);
                }
            }
        }
    }
}
