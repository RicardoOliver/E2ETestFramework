using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace E2ETestFramework.Reporting
{
    public class ExtentReportManager
    {
        private static ExtentReports? _extent;
        private static ExtentTest? _test;
        private static readonly object _lock = new object();
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExtentReportManager> _logger;

        public ExtentReportManager(IConfiguration configuration, ILogger<ExtentReportManager> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public ExtentReports GetExtentReports()
        {
            if (_extent == null)
            {
                lock (_lock)
                {
                    if (_extent == null)
                    {
                        InitializeExtentReports();
                    }
                }
            }
            return _extent!;
        }

        private void InitializeExtentReports()
        {
            try
            {
                var reportPath = Path.Combine(Directory.GetCurrentDirectory(), "Reports",
                    $"TestReport_{DateTime.Now:yyyyMMdd_HHmmss}.html");

                Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);

                var htmlReporter = new ExtentSparkReporter(reportPath);

                // Configure HTML Reporter
                htmlReporter.Config.Theme = Theme.Standard;
                htmlReporter.Config.DocumentTitle = "E2E Test Automation Report";
                htmlReporter.Config.ReportName = "Test Execution Report";

                // Correcting the error: TimeStampFormat is not a valid property in ExtentSparkReporterConfig.
                // Removing the line as it is not supported.
                // htmlReporter.Config.TimeStampFormat = "MMM dd, yyyy HH:mm:ss";

                _extent = new ExtentReports();
                _extent.AttachReporter(htmlReporter);

                // Correcting the error: SetSystemInfo is not a valid method in ExtentReports.
                // Replacing with the appropriate method to add system information.
                _extent.AddSystemInfo("Application", _configuration["TestSettings:ApplicationName"] ?? "Test Application");
                _extent.AddSystemInfo("Environment", _configuration["TestSettings:Environment"] ?? "Test");
                _extent.AddSystemInfo("Browser", _configuration["TestSettings:Browser"] ?? "Chrome");
                _extent.AddSystemInfo("OS", Environment.OSVersion.ToString());
                _extent.AddSystemInfo("User", Environment.UserName);
                _extent.AddSystemInfo("Machine", Environment.MachineName);

                _logger.LogInformation($"Extent Reports initialized: {reportPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Extent Reports");
                throw;
            }
        }

        public ExtentTest CreateTest(string testName, string description = "")
        {
            _test = GetExtentReports().CreateTest(testName, description);
            _logger.LogInformation($"Created test: {testName}");
            return _test;
        }

        public void LogInfo(string message)
        {
            _test?.Info(message);
            _logger.LogInformation(message);
        }

        public void LogPass(string message)
        {
            _test?.Pass(message);
            _logger.LogInformation($"PASS: {message}");
        }

        public void LogFail(string message)
        {
            _test?.Fail(message);
            _logger.LogError($"FAIL: {message}");
        }

        public void LogWarning(string message)
        {
            _test?.Warning(message);
            _logger.LogWarning(message);
        }

        public void LogSkip(string message)
        {
            _test?.Skip(message);
            _logger.LogInformation($"SKIP: {message}");
        }

        public void AttachScreenshot(string screenshotPath)
        {
            try
            {
                if (File.Exists(screenshotPath))
                {
                    _test?.AddScreenCaptureFromPath(screenshotPath);
                    _logger.LogInformation($"Screenshot attached: {screenshotPath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to attach screenshot: {screenshotPath}");
            }
        }

        public void FlushReports()
        {
            try
            {
                _extent?.Flush();
                _logger.LogInformation("Extent Reports flushed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to flush Extent Reports");
            }
        }
    }
}
