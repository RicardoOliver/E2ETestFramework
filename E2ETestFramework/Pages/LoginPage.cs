using OpenQA.Selenium;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using E2ETestFramework.Core;

namespace E2ETestFramework.Pages
{
    public class LoginPage : BasePage
    {
        // Page Elements - Updated for the-internet.herokuapp.com
        private readonly By _usernameField = By.Id("username");
        private readonly By _passwordField = By.Id("password");
        private readonly By _loginButton = By.CssSelector("button[type='submit']");
        private readonly By _errorMessage = By.Id("flash");
        private readonly By _forgotPasswordLink = By.LinkText("Forgot Password?");
        private readonly By _loginForm = By.Id("login");
        private readonly By _usernameLabel = By.CssSelector("label[for='username']");
        private readonly By _passwordLabel = By.CssSelector("label[for='password']");

        public LoginPage(IWebDriver driver, ILogger<LoginPage> logger, IConfiguration configuration)
            : base(driver, logger, configuration)
        {
        }

        public void EnterUsername(string username)
        {
            SendKeys(_usernameField, username);
            Logger.LogInformation($"Entered username: {username}");
        }

        public void EnterPassword(string password)
        {
            SendKeys(_passwordField, password);
            Logger.LogInformation("Entered password");
        }

        public void ClickLoginButton()
        {
            ClickElement(_loginButton);
            Logger.LogInformation("Clicked login button");
        }

        public void Login(string username, string password)
        {
            EnterUsername(username);
            EnterPassword(password);
            ClickLoginButton();
            Logger.LogInformation($"Performed login with username: {username}");
        }

        public string GetErrorMessage()
        {
            return GetText(_errorMessage);
        }

        public bool IsErrorMessageDisplayed()
        {
            return IsElementDisplayed(_errorMessage);
        }

        public void ClickForgotPassword()
        {
            if (IsElementDisplayed(_forgotPasswordLink))
            {
                ClickElement(_forgotPasswordLink);
                Logger.LogInformation("Clicked forgot password link");
            }
            else
            {
                Logger.LogWarning("Forgot password link not found");
            }
        }

        public bool IsLoginPageLoaded()
        {
            return IsElementDisplayed(_usernameField) &&
                   IsElementDisplayed(_passwordField) &&
                   IsElementDisplayed(_loginButton);
        }

        public bool HasAccessibleElements()
        {
            try
            {
                // Check for basic accessibility features
                var usernameField = WaitForElement(_usernameField, 5);
                var passwordField = WaitForElement(_passwordField, 5);
                var loginButton = WaitForElement(_loginButton, 5);

                // Check if elements have accessible names or labels
                bool hasAccessibleUsername = !string.IsNullOrEmpty(usernameField.GetAttribute("aria-label")) ||
                                           !string.IsNullOrEmpty(usernameField.GetAttribute("placeholder")) ||
                                           IsElementDisplayed(_usernameLabel);

                bool hasAccessiblePassword = !string.IsNullOrEmpty(passwordField.GetAttribute("aria-label")) ||
                                           !string.IsNullOrEmpty(passwordField.GetAttribute("placeholder")) ||
                                           IsElementDisplayed(_passwordLabel);

                bool hasAccessibleButton = !string.IsNullOrEmpty(loginButton.GetAttribute("aria-label")) ||
                                         !string.IsNullOrEmpty(loginButton.Text);

                return hasAccessibleUsername && hasAccessiblePassword && hasAccessibleButton;
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Error checking accessibility features");
                return false;
            }
        }

        public bool HasProperFormLabels()
        {
            try
            {
                // Check if form elements have proper labels
                var usernameField = WaitForElement(_usernameField, 5);
                var passwordField = WaitForElement(_passwordField, 5);

                // Check for explicit labels or aria-labels
                bool usernameHasLabel = IsElementDisplayed(_usernameLabel) ||
                                      !string.IsNullOrEmpty(usernameField.GetAttribute("aria-label"));

                bool passwordHasLabel = IsElementDisplayed(_passwordLabel) ||
                                      !string.IsNullOrEmpty(passwordField.GetAttribute("aria-label"));

                return usernameHasLabel && passwordHasLabel;
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Error checking form labels");
                return false;
            }
        }

        public bool SupportsKeyboardNavigation()
        {
            try
            {
                // Test basic keyboard navigation
                var usernameField = WaitForElement(_usernameField, 5);
                var passwordField = WaitForElement(_passwordField, 5);
                var loginButton = WaitForElement(_loginButton, 5);

                // Check if elements are focusable
                usernameField.Click();
                var activeElement = Driver.SwitchTo().ActiveElement();
                bool usernameIsFocusable = activeElement.Equals(usernameField);

                // Test Tab navigation
                activeElement.SendKeys(Keys.Tab);
                activeElement = Driver.SwitchTo().ActiveElement();
                bool canTabToPassword = activeElement.Equals(passwordField);

                activeElement.SendKeys(Keys.Tab);
                activeElement = Driver.SwitchTo().ActiveElement();
                bool canTabToButton = activeElement.Equals(loginButton);

                return usernameIsFocusable && canTabToPassword && canTabToButton;
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Error testing keyboard navigation");
                return false;
            }
        }

        public override void NavigateTo(string url)
        {
            try
            {
                // Validate URL before navigation
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentException("URL cannot be null or empty");
                }

                if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? validatedUri))
                {
                    throw new ArgumentException($"Invalid URL format: {url}");
                }

                // For the-internet.herokuapp.com, navigate to the login page
                if (url.Contains("the-internet.herokuapp.com"))
                {
                    var loginUrl = url.EndsWith("/login") ? url : $"{url.TrimEnd('/')}/login";
                    Driver.Navigate().GoToUrl(loginUrl);
                    Logger.LogInformation($"Navigated to: {loginUrl}");
                }
                else
                {
                    Driver.Navigate().GoToUrl(url);
                    Logger.LogInformation($"Navigated to: {url}");
                }

                // Wait for page to load
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to navigate to: {url}");
                throw;
            }
        }
    }
}
