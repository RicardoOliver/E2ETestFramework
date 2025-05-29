using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;

namespace E2ETestFramework.Metrics
{
    public class TestMetricsCollector
    {
        private readonly ILogger<TestMetricsCollector> _logger;
        private readonly List<TestMetric> _metrics;
        private readonly Stopwatch _stopwatch;

        public TestMetricsCollector(ILogger<TestMetricsCollector> logger)
        {
            _logger = logger;
            _metrics = new List<TestMetric>();
            _stopwatch = new Stopwatch();
        }

        public void StartTest(string testName, string category = "")
        {
            _stopwatch.Restart();
            _logger.LogInformation($"Started test: {testName}");
        }

        public void EndTest(string testName, TestResult result, string errorMessage = "")
        {
            _stopwatch.Stop();
            
            var metric = new TestMetric
            {
                TestName = testName,
                Result = result,
                Duration = _stopwatch.Elapsed,
                Timestamp = DateTime.UtcNow,
                ErrorMessage = errorMessage
            };

            _metrics.Add(metric);
            _logger.LogInformation($"Completed test: {testName} - Result: {result} - Duration: {_stopwatch.Elapsed}");
        }

        public void RecordPerformanceMetric(string metricName, double value, string unit = "ms")
        {
            var metric = new PerformanceMetric
            {
                Name = metricName,
                Value = value,
                Unit = unit,
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation($"Performance metric: {metricName} = {value} {unit}");
        }

        public TestSummary GenerateSummary()
        {
            var summary = new TestSummary
            {
                TotalTests = _metrics.Count,
                PassedTests = _metrics.Count(m => m.Result == TestResult.Pass),
                FailedTests = _metrics.Count(m => m.Result == TestResult.Fail),
                SkippedTests = _metrics.Count(m => m.Result == TestResult.Skip),
                TotalDuration = TimeSpan.FromMilliseconds(_metrics.Sum(m => m.Duration.TotalMilliseconds)),
                AverageDuration = _metrics.Count > 0 ? 
                    TimeSpan.FromMilliseconds(_metrics.Average(m => m.Duration.TotalMilliseconds)) : 
                    TimeSpan.Zero,
                ExecutionDate = DateTime.UtcNow
            };

            return summary;
        }

        public void ExportMetrics(string filePath)
        {
            try
            {
                var summary = GenerateSummary();
                var metricsData = new
                {
                    Summary = summary,
                    DetailedMetrics = _metrics
                };

                var json = JsonConvert.SerializeObject(metricsData, Formatting.Indented);
                File.WriteAllText(filePath, json);
                
                _logger.LogInformation($"Metrics exported to: {filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to export metrics to: {filePath}");
            }
        }
    }

    public class TestMetric
    {
        public string TestName { get; set; } = string.Empty;
        public TestResult Result { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime Timestamp { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class PerformanceMetric
    {
        public string Name { get; set; } = string.Empty;
        public double Value { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class TestSummary
    {
        public int TotalTests { get; set; }
        public int PassedTests { get; set; }
        public int FailedTests { get; set; }
        public int SkippedTests { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public TimeSpan AverageDuration { get; set; }
        public DateTime ExecutionDate { get; set; }
        public double PassRate => TotalTests > 0 ? (double)PassedTests / TotalTests * 100 : 0;
    }

    public enum TestResult
    {
        Pass,
        Fail,
        Skip
    }
}
