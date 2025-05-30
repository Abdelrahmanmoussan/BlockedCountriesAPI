namespace BlockedCountriesAPI.Repositories.IRepositories
{
    public interface IBlockedAttemptsRepository
    {
        public void AddAttempt(BlockedAttempt attempt);
        public IEnumerable<BlockedAttempt> GetAttempts();
    }
}
