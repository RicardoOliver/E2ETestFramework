using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace E2ETestFramework.Core
{
    public abstract class BasePage
    {
        protected readonly IWebDriver Driver;
        protected readonly WebDriverWait Wait;
        protected readonly ILogger Logger;
        protected readonly IConfiguration Configuration;

        protected BasePage(IWebDriver driver, ILogger logger, IConfiguration configuration)
        {
            Driver = driver;
            Logger = logger;
            Configuration = configuration;
            
            var timeout = TimeSpan.FromSeconds(int.Parse(configuration["TestSettings:TimeoutSeconds"] ?? "30"));
            Wait = new WebDriverWait(driver, timeout);
        }

        protected IWebElement WaitForElement(By locator, int timeoutSeconds = 30)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }

        protected IWebElement WaitForClickableElement(By locator, int timeoutSeconds = 30)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(ExpectedConditions.ElementToBeClickable(locator));
        }

        protected void WaitForElementToDisappear(By locator, int timeoutSeconds = 30)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(locator));
        }

        protected void ScrollToElement(IWebElement element)
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
            Thread.Sleep(500); // Small delay for smooth scrolling
        }

        protected void ClickElement(By locator)
        {
            try
            {
                var element = WaitForClickableElement(locator);
                ScrollToElement(element);
                element.Click();
                Logger.LogInformation($"Clicked element: {locator}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to click element: {locator}");
                throw;
            }
        }

        protected void SendKeys(By locator, string text)
        {
            try
            {
                var element = WaitForElement(locator);
                element.Clear();
                element.SendKeys(text);
                Logger.LogInformation($"Sent keys to element: {locator}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to send keys to element: {locator}");
                throw;
            }
        }

        protected string GetText(By locator)
        {
            try
            {
                var element = WaitForElement(locator);
                var text = element.Text;
                Logger.LogInformation($"Got text from element: {locator} - Text: {text}");
                return text;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to get text from element: {locator}");
                throw;
            }
        }

        protected bool IsElementDisplayed(By locator, int timeoutSeconds = 5)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
                wait.Until(ExpectedConditions.ElementIsVisible(locator));
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        protected void TakeScreenshot(string fileName)
        {
            try
            {
                var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
                var directory = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
                Directory.CreateDirectory(directory);
                
                var filePath = Path.Combine(directory, $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                screenshot.SaveAsFile(filePath);
                
                Logger.LogInformation($"Screenshot saved: {filePath}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to take screenshot");
            }
        }

        public virtual void NavigateTo(string url)
        {
            try
            {
                Driver.Navigate().GoToUrl(url);
                Logger.LogInformation($"Navigated to: {url}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to navigate to: {url}");
                throw;
            }
        }
    }
}
