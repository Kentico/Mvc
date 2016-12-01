namespace DancingGoat.Infrastructure
{
    /// <summary>
    /// Defines a method to get a cache key for the current state of the instance.
    /// </summary>
    public interface ICacheKey
    {
        /// <summary>
        /// Returns a cache key which represents the current state of the instance.
        /// </summary>
        string GetCacheKey();
    }
}
