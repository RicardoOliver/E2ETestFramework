using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using Architecture = System.Runtime.InteropServices.Architecture;
using WebDriverManager.Helpers;

namespace E2ETestFramework.Core
{
    public class DriverFactory : IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DriverFactory> _logger;
        private static readonly string DriverDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WebDrivers");
        private static readonly HttpClient _httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(30) };
        private bool _disposed = false;
        private IWebDriver? _driver;

        public DriverFactory(IConfiguration configuration, ILogger<DriverFactory> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Garantir que o diretório de drivers existe
            Directory.CreateDirectory(DriverDirectory);
        }

        public IWebDriver CreateDriver(BrowserType browserType)
        {
            // Se já temos um driver ativo, retorná-lo em vez de criar um novo
            if (_driver != null)
            {
                _logger.LogInformation("Reusing existing WebDriver instance");
                return _driver;
            }

            var headless = bool.Parse(_configuration["TestSettings:Headless"] ?? "false");

            _logger.LogInformation($"Creating {browserType} driver (Headless: {headless})");

            _driver = browserType switch
            {
                BrowserType.Chrome => CreateChromeDriver(headless),
                BrowserType.Firefox => CreateFirefoxDriver(headless),
                BrowserType.Edge => CreateEdgeDriver(headless),
                BrowserType.Safari => CreateSafariDriver(),
                _ => throw new ArgumentException($"Browser type '{browserType}' is not supported")
            };

            return _driver;
        }

        public IWebDriver CreateDriver(string browserName)
        {
            if (!Enum.TryParse<BrowserType>(browserName, true, out var browserType))
            {
                throw new ArgumentException($"Invalid browser name: {browserName}");
            }

            return CreateDriver(browserType);
        }

        private IWebDriver CreateChromeDriver(bool headless)
        {
            var options = new ChromeOptions();
            ConfigureChromeOptions(options, headless);

            // Estratégia unificada: Usar WebDriverManager como principal e ter fallbacks
            try
            {
                _logger.LogInformation("Setting up ChromeDriver using WebDriverManager");

                // Configurar WebDriverManager para baixar o driver correto
                new DriverManager().SetUpDriver(new ChromeConfig());

                // Criar o driver com as opções configuradas
                _logger.LogInformation("Creating ChromeDriver with WebDriverManager");
                return new ChromeDriver(options);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "WebDriverManager failed, trying manual detection");

                // Fallback: Tentar encontrar um driver existente
                string chromeDriverPath = FindExistingChromeDriver();
                if (!string.IsNullOrEmpty(chromeDriverPath))
                {
                    _logger.LogInformation($"Using existing ChromeDriver: {chromeDriverPath}");
                    var service = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(chromeDriverPath));
                    return new ChromeDriver(service, options);
                }

                // Último recurso: tentar com o Selenium Manager padrão
                _logger.LogInformation("Using Selenium Manager as final fallback");
                return new ChromeDriver(options);
            }
        }

        private string FindExistingChromeDriver()
        {
            try
            {
                // Verificar diretórios de drivers personalizados
                var existingDrivers = Directory.GetDirectories(DriverDirectory, "chrome-*");
                foreach (var driverDir in existingDrivers)
                {
                    string driverPath = Path.Combine(driverDir, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "chromedriver.exe" : "chromedriver");
                    if (File.Exists(driverPath))
                    {
                        _logger.LogInformation($"Found existing ChromeDriver: {driverPath}");
                        return driverPath;
                    }
                }

                // Verificar no diretório padrão do WebDriverManager
                string webDriverManagerPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".cache", "selenium", "chromedriver");

                if (Directory.Exists(webDriverManagerPath))
                {
                    var latestVersion = Directory.GetDirectories(webDriverManagerPath)
                        .OrderByDescending(d => d)
                        .FirstOrDefault();

                    if (latestVersion != null)
                    {
                        string driverPath = Path.Combine(
                            latestVersion,
                            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "chromedriver.exe" : "chromedriver");

                        if (File.Exists(driverPath))
                        {
                            _logger.LogInformation($"Found WebDriverManager ChromeDriver: {driverPath}");
                            return driverPath;
                        }
                    }
                }

                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding existing ChromeDriver");
                return "";
            }
        }

        private void ConfigureChromeOptions(ChromeOptions options, bool headless)
        {
            if (headless)
                options.AddArgument("--headless=new");

            // Argumentos básicos
            options.AddArguments(
                "--no-sandbox",
                "--disable-dev-shm-usage",
                "--disable-gpu",
                "--window-size=1920,1080",
                "--disable-extensions",
                "--disable-web-security",
                "--allow-running-insecure-content",
                "--disable-blink-features=AutomationControlled"
            );

            // Configurações anti-detecção
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);

            // Configurações de performance
            options.AddArguments(
                "--disable-background-timer-throttling",
                "--disable-backgrounding-occluded-windows",
                "--disable-renderer-backgrounding"
            );

            // Configurações de segurança para testes
            options.AddArguments(
                "--ignore-certificate-errors",
                "--ignore-ssl-errors",
                "--ignore-certificate-errors-spki-list"
            );

            // Configurações customizadas do arquivo de configuração
            var customArgsSection = _configuration.GetSection("TestSettings:ChromeArguments");
            if (customArgsSection.Exists())
            {
                var customArgs = customArgsSection.Get<string[]>();
                if (customArgs != null && customArgs.Length > 0)
                {
                    options.AddArguments(customArgs);
                    _logger.LogInformation($"Added custom Chrome arguments: {string.Join(", ", customArgs)}");
                }
            }
        }

        private IWebDriver CreateFirefoxDriver(bool headless)
        {
            try
            {
                // Configuração simplificada para Firefox
                _logger.LogInformation("Setting up FirefoxDriver using WebDriverManager");
                new DriverManager().SetUpDriver(new FirefoxConfig());

                var options = new FirefoxOptions();
                ConfigureFirefoxOptions(options, headless);

                _logger.LogInformation("FirefoxDriver configured successfully");
                return new FirefoxDriver(options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Firefox driver");
                throw;
            }
        }

        private void ConfigureFirefoxOptions(FirefoxOptions options, bool headless)
        {
            if (headless)
                options.AddArgument("--headless");

            options.AddArguments("--width=1920", "--height=1080");

            // Configurações de performance
            options.SetPreference("dom.webdriver.enabled", false);
            options.SetPreference("useAutomationExtension", false);

            // Configurações customizadas do arquivo de configuração
            var customPrefsSection = _configuration.GetSection("TestSettings:FirefoxPreferences");
            if (customPrefsSection.Exists())
            {
                var customPrefs = customPrefsSection.Get<Dictionary<string, object>>();
                if (customPrefs != null)
                {
                    foreach (var pref in customPrefs)
                    {
                        // Converter o valor para o tipo apropriado
                        if (pref.Value is bool boolValue)
                        {
                            options.SetPreference(pref.Key, boolValue);
                        }
                        else if (pref.Value is int intValue)
                        {
                            options.SetPreference(pref.Key, intValue);
                        }
                        else if (pref.Value is string stringValue)
                        {
                            options.SetPreference(pref.Key, stringValue);
                        }
                        else if (pref.Value != null)
                        {
                            // Tentar converter string para bool ou int
                            string valueStr = pref.Value.ToString()!;
                            if (bool.TryParse(valueStr, out bool parsedBool))
                            {
                                options.SetPreference(pref.Key, parsedBool);
                            }
                            else if (int.TryParse(valueStr, out int parsedInt))
                            {
                                options.SetPreference(pref.Key, parsedInt);
                            }
                            else
                            {
                                options.SetPreference(pref.Key, valueStr);
                            }
                        }
                    }
                    _logger.LogInformation($"Added {customPrefs.Count} custom Firefox preferences");
                }
            }
        }

        private IWebDriver CreateEdgeDriver(bool headless)
        {
            try
            {
                // Configuração simplificada para Edge
                _logger.LogInformation("Setting up EdgeDriver using WebDriverManager");
                new DriverManager().SetUpDriver(new EdgeConfig());

                var options = new EdgeOptions();
                ConfigureEdgeOptions(options, headless);

                _logger.LogInformation("EdgeDriver configured successfully");
                return new EdgeDriver(options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Edge driver");
                throw;
            }
        }

        private void ConfigureEdgeOptions(EdgeOptions options, bool headless)
        {
            if (headless)
                options.AddArgument("--headless=new");

            options.AddArguments(
                "--no-sandbox",
                "--disable-dev-shm-usage",
                "--disable-gpu",
                "--window-size=1920,1080",
                "--disable-extensions"
            );

            // Configurações anti-detecção
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);
        }

        private IWebDriver CreateSafariDriver()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new PlatformNotSupportedException("Safari is only supported on macOS");
            }

            _logger.LogInformation("Creating Safari driver");
            return new OpenQA.Selenium.Safari.SafariDriver();
        }

        public static BrowserType GetBrowserTypeFromString(string browserName)
        {
            return browserName.ToLower() switch
            {
                "chrome" => BrowserType.Chrome,
                "firefox" => BrowserType.Firefox,
                "edge" => BrowserType.Edge,
                "safari" => BrowserType.Safari,
                _ => throw new ArgumentException($"Unsupported browser: {browserName}")
            };
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                try
                {
                    // Fechar e dispor o driver se existir
                    if (_driver != null)
                    {
                        try
                        {
                            _driver.Quit();
                            _driver.Dispose();
                            _driver = null;
                            _logger.LogInformation("WebDriver disposed successfully");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error disposing WebDriver");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing DriverFactory");
                }
                finally
                {
                    _disposed = true;
                }
            }
        }
    }

    public enum BrowserType
    {
        Chrome,
        Firefox,
        Edge,
        Safari
    }
}
