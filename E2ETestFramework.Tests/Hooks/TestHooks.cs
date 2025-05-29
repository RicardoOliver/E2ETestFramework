using E2ETestFramework.Core;
using E2ETestFramework.Metrics;
using E2ETestFramework.Pages;
using E2ETestFramework.Reporting;
using E2ETestFramework.Tests.StepDefinitions;
using E2ETestFramework.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using TechTalk.SpecFlow;

namespace E2ETestFramework.Tests.Hooks
{
    [Binding]
    public class TestHooks
    {
        private static IServiceProvider? _serviceProvider;
        private static ExtentReportManager? _reportManager;
        private static TestMetricsCollector? _metricsCollector;
        private IServiceScope? _testScope;
        private Core.WebDriverManager? _driverManager;
        private DriverFactory? _driverFactory;

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("Logs/test-log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Setup Dependency Injection
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            // Initialize reporting and metrics
            _reportManager = _serviceProvider.GetRequiredService<ExtentReportManager>();
            _metricsCollector = _serviceProvider.GetRequiredService<TestMetricsCollector>();

            Log.Information("Test run started");
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Logging
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog();
            });

            // Core services - Singleton para DriverFactory para evitar múltiplas instâncias
            services.AddSingleton<DriverFactory>();
            services.AddScoped<Core.WebDriverManager>();
            services.AddScoped<TestDataManager>();
            services.AddSingleton<ExtentReportManager>();
            services.AddSingleton<TestMetricsCollector>();

            // Page Objects - Register as Scoped
            services.AddScoped<LoginPage>(provider =>
            {
                var driverManager = provider.GetRequiredService<Core.WebDriverManager>();
                var logger = provider.GetRequiredService<ILogger<LoginPage>>();
                var config = provider.GetRequiredService<IConfiguration>();
                return new LoginPage(driverManager.GetDriver(), logger, config);
            });

            services.AddScoped<DashboardPage>(provider =>
            {
                var driverManager = provider.GetRequiredService<Core.WebDriverManager>();
                var logger = provider.GetRequiredService<ILogger<DashboardPage>>();
                var config = provider.GetRequiredService<IConfiguration>();
                return new DashboardPage(driverManager.GetDriver(), logger, config);
            });
        }

        [BeforeScenario]
        public void BeforeScenario(ScenarioContext scenarioContext)
        {
            // Create a new scope for this scenario
            _testScope = _serviceProvider!.CreateScope();

            // Get services for this scenario from the scope
            _driverFactory = _testScope.ServiceProvider.GetRequiredService<DriverFactory>();
            _driverManager = _testScope.ServiceProvider.GetRequiredService<Core.WebDriverManager>();

            // Register services in ScenarioContext for step definitions
            scenarioContext.ScenarioContainer.RegisterInstanceAs(_testScope.ServiceProvider.GetRequiredService<LoginPage>());
            scenarioContext.ScenarioContainer.RegisterInstanceAs(_testScope.ServiceProvider.GetRequiredService<DashboardPage>());
            scenarioContext.ScenarioContainer.RegisterInstanceAs(_testScope.ServiceProvider.GetRequiredService<TestDataManager>());
            scenarioContext.ScenarioContainer.RegisterInstanceAs(_testScope.ServiceProvider.GetRequiredService<ILogger<LoginSteps>>());

            // Start test in reporting and metrics
            var testName = $"{scenarioContext.ScenarioInfo.Title}";
            _reportManager!.CreateTest(testName, scenarioContext.ScenarioInfo.Description);
            _metricsCollector!.StartTest(testName);

            Log.Information($"Starting scenario: {testName}");
        }

        [AfterScenario]
        public void AfterScenario(ScenarioContext scenarioContext)
        {
            var testName = scenarioContext.ScenarioInfo.Title;

            try
            {
                // Handle test result
                if (scenarioContext.TestError != null)
                {
                    _reportManager!.LogFail($"Test failed: {scenarioContext.TestError.Message}");
                    _metricsCollector!.EndTest(testName, TestResult.Fail, scenarioContext.TestError.Message);

                    // Take screenshot on failure
                    try
                    {
                        var config = _serviceProvider!.GetRequiredService<IConfiguration>();
                        if (bool.Parse(config["TestSettings:ScreenshotOnFailure"] ?? "true"))
                        {
                            var driver = _driverManager!.GetDriver();
                            var screenshot = ((OpenQA.Selenium.ITakesScreenshot)driver).GetScreenshot();
                            var screenshotPath = Path.Combine("Screenshots", $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                            Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath)!);
                            screenshot.SaveAsFile(screenshotPath);
                            _reportManager.AttachScreenshot(screenshotPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "Failed to take screenshot");
                    }
                }
                else
                {
                    _reportManager!.LogPass("Test completed successfully");
                    _metricsCollector!.EndTest(testName, TestResult.Pass);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in AfterScenario for test: {testName}");
            }
            finally
            {
                // Cleanup resources properly
                try
                {
                    // Dispose o escopo do teste, que vai dispor todos os serviços scoped
                    _testScope?.Dispose();
                    _testScope = null;
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Error disposing test scope");
                }

                Log.Information($"Completed scenario: {testName}");
            }
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            try
            {
                // Generate final reports and metrics
                _reportManager?.FlushReports();

                var metricsPath = Path.Combine("Reports", $"TestMetrics_{DateTime.Now:yyyyMMdd_HHmmss}.json");
                Directory.CreateDirectory(Path.GetDirectoryName(metricsPath)!);
                _metricsCollector?.ExportMetrics(metricsPath);

                // Log test summary
                var summary = _metricsCollector?.GenerateSummary();
                if (summary != null)
                {
                    Log.Information($"Test Summary - Total: {summary.TotalTests}, " +
                                  $"Passed: {summary.PassedTests}, " +
                                  $"Failed: {summary.FailedTests}, " +
                                  $"Skipped: {summary.SkippedTests}, " +
                                  $"Pass Rate: {summary.PassRate:F2}%");
                }

                Log.Information("Test run completed");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in AfterTestRun");
            }
            finally
            {
                // Dispose the main service provider
                if (_serviceProvider is IDisposable disposableProvider)
                {
                    disposableProvider.Dispose();
                }

                Log.CloseAndFlush();
            }
        }
    }
}
