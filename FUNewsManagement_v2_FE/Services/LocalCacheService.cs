using System.Text.Json;

namespace FUNewsManagement_v2_FE.Services
{
    public class LocalCacheService
    {
        private readonly string _cacheDirectory;

        public LocalCacheService(IWebHostEnvironment env)
        {
            _cacheDirectory = Path.Combine(env.ContentRootPath, "App_Data", "Cache");
            if (!Directory.Exists(_cacheDirectory))
            {
                Directory.CreateDirectory(_cacheDirectory);
            }
        }

        public async Task SaveDataAsync<T>(string key, T data)
        {
            var filePath = Path.Combine(_cacheDirectory, $"{key}.json");
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await JsonSerializer.SerializeAsync(fileStream, data);
        }

        public async Task<T?> GetDataAsync<T>(string key)
        {
            var filePath = Path.Combine(_cacheDirectory, $"{key}.json");
            if (!File.Exists(filePath))
            {
                return default;
            }

            try
            {
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return await JsonSerializer.DeserializeAsync<T>(fileStream);
            }
            catch
            {
                // In case file is corrupted
                return default;
            }
        }
    }
}
