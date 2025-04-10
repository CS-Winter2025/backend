using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using BlackBoxTests.Data;

namespace BlackBoxTests.Utils
{
    public class Navigation
    {
        private readonly ChromeDriver _driver;

        public Navigation(ChromeDriver driver)
        {
            _driver = driver;
        }
        public void ToLogin()
        {
            _driver.FindElement(By.LinkText("Login")).Click();
            Assert.That(_driver.Url, Does.Contain(Uris.Login), $"Url: {_driver.Url} does not contain {Uris.Login}");
        }

        public void ToCharges()
        {
            _driver.FindElement(By.LinkText("Charges")).Click();
            Assert.That(_driver.Url, Does.Contain(Uris.Charges), $"Url: {_driver.Url} does not contain {Uris.Charges}");
        }

        public void ToEmployees()
        {
            _driver.FindElement(By.LinkText("Employees")).Click();
            Assert.That(_driver.Url, Does.Contain(Uris.Employees), $"Url: {_driver.Url} does not contain {Uris.Employees}");
        }

        public void ToServices()
        {
            _driver.FindElement(By.LinkText("Services")).Click();
            Assert.That(_driver.Url, Does.Contain(Uris.Services), $"Url: {_driver.Url} does not contain {Uris.Services}");
        }

        // CRUD Naviation
        public void ToCreateNew(string createUri)
        {
            _driver.FindElement(By.LinkText("Create New")).Click();
            Assert.That(_driver.Url, Does.Contain(createUri), "Did not navigate to the Create page.");
        }

        public void ToDetails(IWebElement row, string detailsUri)
        {
            row.FindElement(By.XPath(XPaths.DetailsLink)).Click();
           Assert.That(_driver.Url, Does.Contain(detailsUri), "Did not navigate to the Details page.");
        }

        public void ToEdit(IWebElement row, string editUri)
        {
            row.FindElement(By.XPath(XPaths.EditLink)).Click();
            Assert.That(_driver.Url, Does.Contain(editUri), "Did not navigate to the Edit page.");
        }

        public void ToDelete(IWebElement row, string deleteUri)
        {
            row.FindElement(By.XPath(XPaths.DeleteLink)).Click();
            Assert.That(_driver.Url, Does.Contain(deleteUri), "Did not navigate to the Delete page.");
        }
    }
}
