using System.Collections.Concurrent;
using System.Text.Json;

namespace AvaloniaUI.MCP.Services;

/// <summary>
/// Provides efficient caching for knowledge base resources to improve performance
/// </summary>
public static class ResourceCacheService
{
    static readonly ConcurrentDictionary<string, CachedResource> Cache = new();
    static readonly Lock CacheLock = new();

    // Cache configuration
    static readonly TimeSpan DefaultCacheExpiry = TimeSpan.FromMinutes(30);
    static readonly int MaxCacheSize = 50;

    sealed class CachedResource(string content, TimeSpan cacheExpiry)
    {
        public string Content { get; } = content;
        public DateTime ExpiryTime { get; } = DateTime.UtcNow.Add(cacheExpiry);
        public DateTime LastAccessed { get; set; } = DateTime.UtcNow;

        public bool IsExpired => DateTime.UtcNow > ExpiryTime;
    }

    /// <summary>
    /// Gets a cached resource or loads it from the file system
    /// </summary>
    public static async Task<string> GetOrLoadResourceAsync(string resourceKey, Func<Task<string>> loader, TimeSpan? cacheExpiry = null)
    {
        TimeSpan expiry = cacheExpiry ?? DefaultCacheExpiry;

        // Try to get from cache first
        if (Cache.TryGetValue(resourceKey, out CachedResource? cachedResource))
        {
            if (!cachedResource.IsExpired)
            {
                cachedResource.LastAccessed = DateTime.UtcNow;
                return cachedResource.Content;
            }
            else
            {
                // Remove expired entry
                Cache.TryRemove(resourceKey, out _);
            }
        }

        // Load the resource
        string content = await loader();

        // Cache the result
        await CacheResourceAsync(resourceKey, content, expiry);

        return content;
    }

    /// <summary>
    /// Gets a cached resource synchronously or loads it from the file system
    /// </summary>
    public static string GetOrLoadResource(string resourceKey, Func<string> loader, TimeSpan? cacheExpiry = null)
    {
        TimeSpan expiry = cacheExpiry ?? DefaultCacheExpiry;

        // Try to get from cache first
        if (Cache.TryGetValue(resourceKey, out CachedResource? cachedResource))
        {
            if (!cachedResource.IsExpired)
            {
                cachedResource.LastAccessed = DateTime.UtcNow;
                return cachedResource.Content;
            }
            else
            {
                // Remove expired entry
                Cache.TryRemove(resourceKey, out _);
            }
        }

        // Load the resource
        string content = loader();

        // Cache the result
        CacheResource(resourceKey, content, expiry);

        return content;
    }

    /// <summary>
    /// Loads and caches JSON data from a file
    /// </summary>
    public static JsonElement GetOrLoadJsonResource(string filePath, TimeSpan? cacheExpiry = null)
    {
        string cacheKey = $"json:{filePath}";

        string jsonString = GetOrLoadResource(cacheKey, () => !File.Exists(filePath) ? throw new FileNotFoundException($"Resource file not found: {filePath}") : File.ReadAllText(filePath), cacheExpiry);

        return JsonSerializer.Deserialize<JsonElement>(jsonString);
    }

    /// <summary>
    /// Loads and caches JSON data from a file asynchronously
    /// </summary>
    public static async Task<JsonElement> GetOrLoadJsonResourceAsync(string filePath, TimeSpan? cacheExpiry = null)
    {
        string cacheKey = $"json:{filePath}";

        string jsonString = await GetOrLoadResourceAsync(cacheKey, async () => !File.Exists(filePath)
                ? throw new FileNotFoundException($"Resource file not found: {filePath}")
                : await File.ReadAllTextAsync(filePath), cacheExpiry);

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

    static void CacheResource(string key, string content, TimeSpan expiry)
    {
        lock (CacheLock)
        {
            // Ensure cache doesn't grow too large
            if (Cache.Count >= MaxCacheSize)
            {
                CleanupExpiredEntries();

                // If still too large, remove least recently used
                if (Cache.Count >= MaxCacheSize)
                {
                    RemoveLeastRecentlyUsed();
                }
            }

            Cache.TryAdd(key, new CachedResource(content, expiry));
        }
    }

    static async Task CacheResourceAsync(string key, string content, TimeSpan expiry)
    {
        await Task.Run(() => CacheResource(key, content, expiry));
    }

    /// <summary>
    /// Removes expired entries from the cache
    /// </summary>
    public static void CleanupExpiredEntries()
    {
        lock (CacheLock)
        {
            var expiredKeys = Cache
                .Where(kvp => kvp.Value.IsExpired)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (string? key in expiredKeys)
            {
                Cache.TryRemove(key, out _);
            }
        }
    }

    static void RemoveLeastRecentlyUsed()
    {
        KeyValuePair<string, CachedResource> lruEntry = Cache
            .OrderBy(kvp => kvp.Value.LastAccessed)
            .FirstOrDefault();

        if (!lruEntry.Equals(default(KeyValuePair<string, CachedResource>)))
        {
            Cache.TryRemove(lruEntry.Key, out _);
        }
    }

    /// <summary>
    /// Clears all cached resources
    /// </summary>
    public static void ClearCache()
    {
        lock (CacheLock)
        {
            Cache.Clear();
        }
    }

    /// <summary>
    /// Removes a specific resource from the cache
    /// </summary>
    public static bool RemoveFromCache(string resourceKey)
    {
        return Cache.TryRemove(resourceKey, out _);
    }

    /// <summary>
    /// Gets cache statistics for monitoring
    /// </summary>
    public static CacheStatistics GetCacheStatistics()
    {
        lock (CacheLock)
        {
            DateTime now = DateTime.UtcNow;
            int totalEntries = Cache.Count;
            int expiredEntries = Cache.Count(kvp => kvp.Value.IsExpired);
            int validEntries = totalEntries - expiredEntries;

            CachedResource? oldestEntry = Cache.Values.MinBy(v => v.LastAccessed);
            CachedResource? newestEntry = Cache.Values.MaxBy(v => v.LastAccessed);

            return new CacheStatistics
            {
                TotalEntries = totalEntries,
                ValidEntries = validEntries,
                ExpiredEntries = expiredEntries,
                OldestEntryAge = oldestEntry != null ? now - oldestEntry.LastAccessed : TimeSpan.Zero,
                NewestEntryAge = newestEntry != null ? now - newestEntry.LastAccessed : TimeSpan.Zero,
                CacheKeys = [.. Cache.Keys]
            };
        }
    }

    /// <summary>
    /// Preloads common resources into the cache
    /// </summary>
    public static async Task PreloadCommonResourcesAsync()
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string dataDirectory = Path.Combine(baseDirectory, "Data");

        if (!Directory.Exists(dataDirectory))
        {
            return;
        }

        string[] commonFiles =
        [
            "controls.json",
            "xaml-patterns.json",
            "migration-guide.json"
        ];

        IEnumerable<Task> loadTasks = commonFiles
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
        public List<string> CacheKeys { get; set; } = [];

        public double HitRatio => TotalEntries > 0 ? (double)ValidEntries / TotalEntries : 0.0;

        public override string ToString()
        {
            return $"Cache Statistics: {ValidEntries}/{TotalEntries} valid entries, " +
                   $"{HitRatio:P1} hit ratio, {ExpiredEntries} expired";
        }
    }
}