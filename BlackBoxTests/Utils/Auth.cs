using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BlackBoxTests.Utils
{
    public class Auth
    {
        private readonly ChromeDriver _driver;

        public Auth(ChromeDriver driver)
        {
            _driver = driver;
        }

        public void LoginAsUser(string Username, string Password)
        {
            _driver.FindElement(By.Id("Username")).SendKeys(Username);
            _driver.FindElement(By.Id("Password")).SendKeys(Password);
            _driver.FindElement(By.XPath("//button[text()='Login']")).Click();
        }

        public void Logout()
        {
            _driver.FindElement(By.LinkText("Logout")).Click();
        }
    }
}
