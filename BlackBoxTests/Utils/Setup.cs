using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BlackBoxTests.Utils
{
    public class Setup
    {
        public ChromeDriver Driver;

        private const string BaseUrl = "https://localhost:7176";
        private const string LoginUri = "Users/Login";
        private const int WaitTimer = 5;

        public Setup()
        {
            Driver = new ChromeDriver();
        }

        public void Initialize()
        {
            Driver.Manage().Window.Maximize();
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(WaitTimer);
            Driver.Navigate().GoToUrl(BaseUrl);
        }

        public void CleanupChromeDriver()
        {
            Driver.Quit();
            Driver.Dispose();
        }
    }
}
