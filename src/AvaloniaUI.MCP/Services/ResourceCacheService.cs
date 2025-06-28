using System.Collections.Concurrent;
using System.Text.Json;

namespace AvaloniaUI.MCP.Services;

/// <summary>
/// Provides efficient caching for knowledge base resources to improve performance
/// </summary>
public static class ResourceCacheService
{
    private static readonly ConcurrentDictionary<string, CachedResource> _cache = new();
    private static readonly object _cacheLock = new();

    // Cache configuration
    private static readonly TimeSpan DefaultCacheExpiry = TimeSpan.FromMinutes(30);
    private static readonly int MaxCacheSize = 50;

    private class CachedResource
    {
        public string Content { get; }
        public DateTime ExpiryTime { get; }
        public DateTime LastAccessed { get; set; }

        public CachedResource(string content, TimeSpan cacheExpiry)
        {
            Content = content;
            ExpiryTime = DateTime.UtcNow.Add(cacheExpiry);
            LastAccessed = DateTime.UtcNow;
        }

        public bool IsExpired => DateTime.UtcNow > ExpiryTime;
    }

    /// <summary>
    /// Gets a cached resource or loads it from the file system
    /// </summary>
    public static async Task<string> GetOrLoadResourceAsync(string resourceKey, Func<Task<string>> loader, TimeSpan? cacheExpiry = null)
    {
        var expiry = cacheExpiry ?? DefaultCacheExpiry;

        // Try to get from cache first
        if (_cache.TryGetValue(resourceKey, out var cachedResource))
        {
            if (!cachedResource.IsExpired)
            {
                cachedResource.LastAccessed = DateTime.UtcNow;
                return cachedResource.Content;
            }
            else
            {
                // Remove expired entry
                _cache.TryRemove(resourceKey, out _);
            }
        }

        // Load the resource
        var content = await loader();

        // Cache the result
        await CacheResourceAsync(resourceKey, content, expiry);

        return content;
    }

    /// <summary>
    /// Gets a cached resource synchronously or loads it from the file system
    /// </summary>
    public static string GetOrLoadResource(string resourceKey, Func<string> loader, TimeSpan? cacheExpiry = null)
    {
        var expiry = cacheExpiry ?? DefaultCacheExpiry;

        // Try to get from cache first
        if (_cache.TryGetValue(resourceKey, out var cachedResource))
        {
            if (!cachedResource.IsExpired)
            {
                cachedResource.LastAccessed = DateTime.UtcNow;
                return cachedResource.Content;
            }
            else
            {
                // Remove expired entry
                _cache.TryRemove(resourceKey, out _);
            }
        }

        // Load the resource
        var content = loader();

        // Cache the result
        CacheResource(resourceKey, content, expiry);

        return content;
    }

    /// <summary>
    /// Loads and caches JSON data from a file
    /// </summary>
    public static JsonElement GetOrLoadJsonResource(string filePath, TimeSpan? cacheExpiry = null)
    {
        var cacheKey = $"json:{filePath}";

        var jsonString = GetOrLoadResource(cacheKey, () =>
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Resource file not found: {filePath}");

            return File.ReadAllText(filePath);
        }, cacheExpiry);

        return JsonSerializer.Deserialize<JsonElement>(jsonString);
    }

    /// <summary>
    /// Loads and caches JSON data from a file asynchronously
    /// </summary>
    public static async Task<JsonElement> GetOrLoadJsonResourceAsync(string filePath, TimeSpan? cacheExpiry = null)
    {
        var cacheKey = $"json:{filePath}";

        var jsonString = await GetOrLoadResourceAsync(cacheKey, async () =>
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Resource file not found: {filePath}");

            return await File.ReadAllTextAsync(filePath);
        }, cacheExpiry);

        return JsonSerializer.Deserialize<JsonElement>(jsonString);
    }

    /// <summary>
    /// Caches a processed resource result
    /// </summary>
    public static void CacheProcessedResource(string cacheKey, string processedContent, TimeSpan? cacheExpiry = null)
    {
        CacheResource(cacheKey, processedContent, cacheExpiry ?? DefaultCacheExpiry);
    }

    /// <summary>
    /// Caches a processed resource result asynchronously
    /// </summary>
    public static Task CacheProcessedResourceAsync(string cacheKey, string processedContent, TimeSpan? cacheExpiry = null)
    {
        return Task.Run(() => CacheResource(cacheKey, processedContent, cacheExpiry ?? DefaultCacheExpiry));
    }

    private static void CacheResource(string key, string content, TimeSpan expiry)
    {
        lock (_cacheLock)
        {
            // Ensure cache doesn't grow too large
            if (_cache.Count >= MaxCacheSize)
            {
                CleanupExpiredEntries();

                // If still too large, remove least recently used
                if (_cache.Count >= MaxCacheSize)
                {
                    RemoveLeastRecentlyUsed();
                }
            }

            _cache.TryAdd(key, new CachedResource(content, expiry));
        }
    }

    private static async Task CacheResourceAsync(string key, string content, TimeSpan expiry)
    {
        await Task.Run(() => CacheResource(key, content, expiry));
    }

    /// <summary>
    /// Removes expired entries from the cache
    /// </summary>
    public static void CleanupExpiredEntries()
    {
        lock (_cacheLock)
        {
            var expiredKeys = _cache
                .Where(kvp => kvp.Value.IsExpired)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _cache.TryRemove(key, out _);
            }
        }
    }

    private static void RemoveLeastRecentlyUsed()
    {
        var lruEntry = _cache
            .OrderBy(kvp => kvp.Value.LastAccessed)
            .FirstOrDefault();

        if (!lruEntry.Equals(default(KeyValuePair<string, CachedResource>)))
        {
            _cache.TryRemove(lruEntry.Key, out _);
        }
    }

    /// <summary>
    /// Clears all cached resources
    /// </summary>
    public static void ClearCache()
    {
        lock (_cacheLock)
        {
            _cache.Clear();
        }
    }

    /// <summary>
    /// Removes a specific resource from the cache
    /// </summary>
    public static bool RemoveFromCache(string resourceKey)
    {
        return _cache.TryRemove(resourceKey, out _);
    }

    /// <summary>
    /// Gets cache statistics for monitoring
    /// </summary>
    public static CacheStatistics GetCacheStatistics()
    {
        lock (_cacheLock)
        {
            var now = DateTime.UtcNow;
            var totalEntries = _cache.Count;
            var expiredEntries = _cache.Count(kvp => kvp.Value.IsExpired);
            var validEntries = totalEntries - expiredEntries;

            var oldestEntry = _cache.Values.MinBy(v => v.LastAccessed);
            var newestEntry = _cache.Values.MaxBy(v => v.LastAccessed);

            return new CacheStatistics
            {
                TotalEntries = totalEntries,
                ValidEntries = validEntries,
                ExpiredEntries = expiredEntries,
                OldestEntryAge = oldestEntry != null ? now - oldestEntry.LastAccessed : TimeSpan.Zero,
                NewestEntryAge = newestEntry != null ? now - newestEntry.LastAccessed : TimeSpan.Zero,
                CacheKeys = _cache.Keys.ToList()
            };
        }
    }

    /// <summary>
    /// Preloads common resources into the cache
    /// </summary>
    public static async Task PreloadCommonResourcesAsync()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var dataDirectory = Path.Combine(baseDirectory, "Data");

        if (!Directory.Exists(dataDirectory))
            return;

        var commonFiles = new[]
        {
            "controls.json",
            "xaml-patterns.json",
            "migration-guide.json"
        };

        var loadTasks = commonFiles
            .Select(fileName => Path.Combine(dataDirectory, fileName))
            .Where(File.Exists)
            .Select(async filePath =>
            {
                try
                {
                    await GetOrLoadJsonResourceAsync(filePath, TimeSpan.FromHours(1)); // Cache for 1 hour
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Failed to preload resource {filePath}: {ex.Message}");
                }
            });

        await Task.WhenAll(loadTasks);
    }

    /// <summary>
    /// Cache statistics for monitoring and debugging
    /// </summary>
    public class CacheStatistics
    {
        public int TotalEntries { get; set; }
        public int ValidEntries { get; set; }
        public int ExpiredEntries { get; set; }
        public TimeSpan OldestEntryAge { get; set; }
        public TimeSpan NewestEntryAge { get; set; }
        public List<string> CacheKeys { get; set; } = new();

        public double HitRatio => TotalEntries > 0 ? (double)ValidEntries / TotalEntries : 0.0;

        public override string ToString()
        {
            return $"Cache Statistics: {ValidEntries}/{TotalEntries} valid entries, " +
                   $"{HitRatio:P1} hit ratio, {ExpiredEntries} expired";
        }
    }
}