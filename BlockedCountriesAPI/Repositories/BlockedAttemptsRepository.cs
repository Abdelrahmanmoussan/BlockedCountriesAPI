namespace BlockedCountriesAPI.Repositories
{
    public class BlockedAttemptsRepository : IBlockedAttemptsRepository
    {
        private readonly ConcurrentBag<BlockedAttempt> _blockedAttempts = new();

        public void AddAttempt(BlockedAttempt attempt)
        {
            _blockedAttempts.Add(attempt);
        }

        public IEnumerable<BlockedAttempt> GetAttempts()
        {
            return _blockedAttempts.ToArray();
        }
    }

}
