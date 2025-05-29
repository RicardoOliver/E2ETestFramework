using OpenQA.Selenium;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace E2ETestFramework.Core
{
    public class WebDriverManager : IDisposable
    {
        private IWebDriver? _driver;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WebDriverManager> _logger;
        private readonly DriverFactory _driverFactory;
        private bool _disposed = false;

        public WebDriverManager(IConfiguration configuration, ILogger<WebDriverManager> logger, DriverFactory driverFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _driverFactory = driverFactory;
        }

        public IWebDriver GetDriver()
        {
            if (_driver == null)
            {
                _driver = CreateDriver();
                ConfigureDriver();
            }
            return _driver;
        }

        private IWebDriver CreateDriver()
        {
            var browserType = _configuration["TestSettings:Browser"] ?? "Chrome";

            try
            {
                var browser = DriverFactory.GetBrowserTypeFromString(browserType);
                return _driverFactory.CreateDriver(browser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create driver for browser: {browserType}");
                throw;
            }
        }

        private void ConfigureDriver()
        {
            var timeout = TimeSpan.FromSeconds(int.Parse(_configuration["TestSettings:TimeoutSeconds"] ?? "30"));

            _driver!.Manage().Timeouts().ImplicitWait = timeout;
            _driver.Manage().Timeouts().PageLoad = timeout;
            _driver.Manage().Window.Maximize();

            _logger.LogInformation("WebDriver configured successfully");
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                try
                {
                    // Não precisamos dispor o driver aqui, pois o DriverFactory já cuida disso
                    // Apenas registrar que o WebDriverManager foi disposto
                    _logger.LogInformation("WebDriverManager disposed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing WebDriverManager");
                }
                finally
                {
                    _disposed = true;
                }
            }
        }
    }
}
