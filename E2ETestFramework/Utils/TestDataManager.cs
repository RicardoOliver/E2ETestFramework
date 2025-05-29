using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace E2ETestFramework.Utils
{
    public class TestDataManager
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TestDataManager> _logger;
        private readonly Dictionary<string, object> _testData;

        public TestDataManager(IConfiguration configuration, ILogger<TestDataManager> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _testData = new Dictionary<string, object>();
            LoadTestData();
        }

        private void LoadTestData()
        {
            try
            {
                var testDataPath = _configuration["TestSettings:TestDataPath"] ?? "TestData";
                var files = Directory.GetFiles(testDataPath, "*.json", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var content = File.ReadAllText(file);
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                    
                    if (data != null)
                    {
                        _testData[fileName] = data;
                        _logger.LogInformation($"Loaded test data from: {file}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load test data");
            }
        }

        public T GetTestData<T>(string category, string key)
        {
            try
            {
                if (_testData.ContainsKey(category))
                {
                    var categoryData = (Dictionary<string, object>)_testData[category];
                    if (categoryData.ContainsKey(key))
                    {
                        var value = categoryData[key];
                        return JsonConvert.DeserializeObject<T>(value.ToString()!);
                    }
                }
                
                _logger.LogWarning($"Test data not found: {category}.{key}");
                return default(T)!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get test data: {category}.{key}");
                return default(T)!;
            }
        }

        public string GetConnectionString(string name)
        {
            return _configuration.GetConnectionString(name) ?? string.Empty;
        }

        public string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name) ?? string.Empty;
        }

        public Dictionary<string, string> GetUserCredentials(string userType)
        {
            return GetTestData<Dictionary<string, string>>("users", userType) ?? new Dictionary<string, string>();
        }
    }
}
