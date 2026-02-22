using FUNewsManagement_v2_AIAPI.Services.Interfaces;
using System.Text.Json;

namespace FUNewsManagement_v2_AIAPI.Services
{
    public class LearningCacheService : ILearningCacheService
    {
        private readonly string _cacheFilePath = "learning_cache.json";
        private Dictionary<string, int> _cache;
        private readonly object _lockObj = new object();

        public LearningCacheService()
        {
            _cache = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            LoadCache();
        }

        private void LoadCache()
        {
            lock (_lockObj)
            {
                if (File.Exists(_cacheFilePath))
                {
                    try
                    {
                        var json = File.ReadAllText(_cacheFilePath);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            var loadedData = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
                            if (loadedData != null)
                            {
                                _cache = new Dictionary<string, int>(loadedData, StringComparer.OrdinalIgnoreCase);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading learning cache: {ex.Message}");
                    }
                }
            }
        }

        private void SaveCache()
        {
            lock (_lockObj)
            {
                try
                {
                    var json = JsonSerializer.Serialize(_cache, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(_cacheFilePath, json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving learning cache: {ex.Message}");
                }
            }
        }

        public void LearnTags(List<string> selectedTags)
        {
            if (selectedTags == null || selectedTags.Count == 0) return;

            lock (_lockObj)
            {
                bool updated = false;
                foreach (var tag in selectedTags)
                {
                    if (string.IsNullOrWhiteSpace(tag)) continue;
                    var cleanTag = tag.Trim();

                    if (_cache.ContainsKey(cleanTag))
                    {
                        _cache[cleanTag]++;
                    }
                    else
                    {
                        _cache[cleanTag] = 1;
                    }
                    updated = true;
                }

                if (updated)
                {
                    SaveCache();
                }
            }
        }

        public Dictionary<string, int> GetCache()
        {
            lock (_lockObj)
            {
                return new Dictionary<string, int>(_cache, StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
