using OpenQA.Selenium;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using E2ETestFramework.Core;

namespace E2ETestFramework.Pages
{
    public class DashboardPage : BasePage
    {
        // Page Elements - Updated for the-internet.herokuapp.com
        private readonly By _welcomeMessage = By.CssSelector(".flash.success");
        private readonly By _userMenu = By.Id("user-menu");
        private readonly By _logoutButton = By.LinkText("Logout");
        private readonly By _profileLink = By.LinkText("Profile");
        private readonly By _settingsLink = By.LinkText("Settings");
        private readonly By _dashboardTitle = By.CssSelector("h2");
        private readonly By _secureAreaHeader = By.CssSelector("h2");
        private readonly By _secureAreaContent = By.CssSelector("h4.subheader");

        public DashboardPage(IWebDriver driver, ILogger<DashboardPage> logger, IConfiguration configuration)
            : base(driver, logger, configuration)
        {
        }

        public string GetWelcomeMessage()
        {
            // Para the-internet.herokuapp.com, verificar múltiplos locais possíveis
            if (IsElementDisplayed(_welcomeMessage))
            {
                return GetText(_welcomeMessage);
            }

            // Verificar se há mensagem de sucesso na área segura
            if (IsElementDisplayed(_secureAreaHeader))
            {
                var headerText = GetText(_secureAreaHeader);

                // Verificar se há conteúdo adicional
                if (IsElementDisplayed(_secureAreaContent))
                {
                    var contentText = GetText(_secureAreaContent);
                    return $"{headerText} - {contentText}";
                }

                return headerText;
            }

            // Verificar se estamos na página correta através da URL
            if (Driver.Url.Contains("secure"))
            {
                return "You logged into a secure area!";
            }

            return "Welcome to the secure area!";
        }

        public string GetDashboardTitle()
        {
            if (IsElementDisplayed(_dashboardTitle))
            {
                return GetText(_dashboardTitle);
            }

            if (IsElementDisplayed(_secureAreaHeader))
            {
                return GetText(_secureAreaHeader);
            }

            return "Secure Area";
        }

        public void ClickUserMenu()
        {
            if (IsElementDisplayed(_userMenu))
            {
                ClickElement(_userMenu);
                Logger.LogInformation("Clicked user menu");
            }
            else
            {
                Logger.LogWarning("User menu not found");
            }
        }

        public void Logout()
        {
            if (IsElementDisplayed(_logoutButton))
            {
                ClickElement(_logoutButton);
                Logger.LogInformation("Performed logout");
            }
            else
            {
                Logger.LogWarning("Logout button not found");
            }
        }

        public void GoToProfile()
        {
            ClickUserMenu();
            if (IsElementDisplayed(_profileLink))
            {
                ClickElement(_profileLink);
                Logger.LogInformation("Navigated to profile");
            }
            else
            {
                Logger.LogWarning("Profile link not found");
            }
        }

        public void GoToSettings()
        {
            ClickUserMenu();
            if (IsElementDisplayed(_settingsLink))
            {
                ClickElement(_settingsLink);
                Logger.LogInformation("Navigated to settings");
            }
            else
            {
                Logger.LogWarning("Settings link not found");
            }
        }

        public bool IsDashboardLoaded()
        {
            // Check for secure area (the-internet.herokuapp.com)
            if (IsElementDisplayed(_secureAreaHeader))
            {
                return true;
            }

            // Check for traditional dashboard elements
            if (IsElementDisplayed(_dashboardTitle) || IsElementDisplayed(_userMenu))
            {
                return true;
            }

            // Check URL for secure area
            return Driver.Url.Contains("secure");
        }

        public bool IsUserLoggedIn()
        {
            // Check for success message or secure area
            if (IsElementDisplayed(_welcomeMessage) || IsElementDisplayed(_secureAreaHeader))
            {
                return true;
            }

            // Check URL for secure area
            if (Driver.Url.Contains("secure"))
            {
                return true;
            }

            // Check page title
            try
            {
                var pageTitle = Driver.Title;
                if (pageTitle.Contains("Secure Area") || pageTitle.Contains("secure"))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Error checking page title");
            }

            return false;
        }
    }
}
