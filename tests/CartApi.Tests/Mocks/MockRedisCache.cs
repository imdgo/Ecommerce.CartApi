using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace CartApi.Tests.Mocks
{
    public static class MockRedisCache
    {
        public static Mock<IDistributedCache> GetDistributedCache()
        {
            var cacheStorage = new ConcurrentDictionary<string, string>();
            var mockCache = new Mock<IDistributedCache>();

            // SetStringAsync
            mockCache.Setup(c => c.SetStringAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DistributedCacheEntryOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns<string, string, DistributedCacheEntryOptions, CancellationToken>((key, value, options, token) =>
                {
                    cacheStorage[key] = value;
                    return Task.CompletedTask;
                });

            // GetStringAsync
            mockCache.Setup(c => c.GetStringAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns<string, CancellationToken>((key, token) =>
                {
                    cacheStorage.TryGetValue(key, out var value);
                    return Task.FromResult(value);
                });

            // RemoveAsync
            mockCache.Setup(c => c.RemoveAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns<string, CancellationToken>((key, token) =>
                {
                    cacheStorage.TryRemove(key, out _);
                    return Task.CompletedTask;
                });

            return mockCache;
        }
    }
}
