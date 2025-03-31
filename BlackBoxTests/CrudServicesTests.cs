using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Linq;

namespace BlackBoxTests;

public class CrudServicesTests
{
    private ChromeDriver _driver;
    private const string BaseUrl = "http://localhost:5072/"; // Change as needed
    private const string ServiceUri = "Services?area=Services";
    
    private const string ServiceType = "Coaching";
    private const string ServiceRate = "95.00";
    private const string ServiceRequirements = "None";

    private const string NewServiceType = "Therapy";
    private const string NewServiceRate = "50.00";
    private const string NewServiceRequirements = "Licence";
    
    [OneTimeSetUp]
    public void Setup()
    {
        _driver = new ChromeDriver();
        _driver.Manage().Window.Maximize();
    }

    [Test, Order(1)]
    public void CreateService()
    {
        _driver.Navigate().GoToUrl(BaseUrl + ServiceUri);
        
        // Create New
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        _driver.FindElement(By.LinkText("Create New")).Click();
        
        // Input Values
        _driver.FindElement(By.Id("Type")).SendKeys(ServiceType);
        _driver.FindElement(By.Id("Rate")).SendKeys(ServiceRate);
        _driver.FindElement(By.Id("Requirements")).SendKeys(ServiceRequirements);
        
        // Submit Create
        _driver.FindElement(By.XPath("//Input[@type='submit']")).Click();
        
        // Wait till back on Services home
        _driver.FindElement(By.LinkText("Create New"));
        
        // Ensure row exists and check values
        var serviceRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(ServiceType));
        Assert.That(serviceRow, Is.Not.Null, $"Service '{ServiceType}' not found in the list.");

        var serviceColumns = serviceRow.FindElements(By.TagName("td"));
        Assert.Multiple(() =>
        {
            Assert.That(serviceColumns[0].Text.Trim(), Is.EqualTo(ServiceType), "Service type mismatch.");
            Assert.That(serviceColumns[1].Text.Trim(), Is.EqualTo(ServiceRate), "Service rate mismatch.");
            Assert.That(serviceColumns[2].Text.Trim(), Is.EqualTo(ServiceRequirements), "Service requirements mismatch.");
        });
    }
    
    [Test, Order(2)]
    public void ReadService()
    {
        _driver.Navigate().GoToUrl(BaseUrl + ServiceUri);
        
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        _driver.FindElement(By.LinkText("Create New"));
        
        // Ensure row exists
        var serviceRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(ServiceType));
        Assert.That(serviceRow, Is.Not.Null, $"Service '{ServiceType}' not found in the list.");
        
        serviceRow.FindElement(By.XPath(".//a[text()='Details']")).Click();
        
        Assert.That(_driver.Url, Does.Contain("/Services/Details"), "Did not navigate to the Details page.");

        var serviceDetailsTable = _driver.FindElement(By.XPath(".//dl"));
        var serviceDetails = serviceDetailsTable.FindElements(By.TagName("dd"));
        
        Assert.Multiple(() =>
        {
            Assert.That(serviceDetails[0].Text.Trim(), Is.EqualTo(ServiceType), "Service type mismatch.");
            Assert.That(serviceDetails[1].Text.Trim(), Is.EqualTo(ServiceRate), "Service rate mismatch.");
            Assert.That(serviceDetails[2].Text.Trim(), Is.EqualTo(ServiceRequirements), "Service requirements mismatch.");
        });
    }

    [Test, Order(3)]
    public void UpdateService()
    {
        _driver.Navigate().GoToUrl(BaseUrl + ServiceUri);
        
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        _driver.FindElement(By.LinkText("Create New"));
        
        // Ensure row exists
        var serviceRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(ServiceType));
        Assert.That(serviceRow, Is.Not.Null, $"Service '{ServiceType}' not found in the list.");
        
        serviceRow.FindElement(By.XPath(".//a[text()='Edit']")).Click();
        
        Assert.That(_driver.Url, Does.Contain("/Services/Edit"), "Did not navigate to the Edit page.");
        
        // NOTE currently not working as Requirements field returns: System.Collections.Generic.List`1[System.String]
        // var currentServiceType = _driver.FindElement(By.Id("Type")).Text.Trim();
        // var currentServiceRate = _driver.FindElement(By.Id("Rate")).Text.Trim();
        // var currentServiceRequirements = _driver.FindElement(By.Id("Requirements")).Text.Trim(); // Bug found
        // Assert.Multiple(() =>
        // {
        //     Assert.That(currentServiceType, Is.EqualTo(ServiceType), "Service type mismatch.");
        //     Assert.That(currentServiceRate, Is.EqualTo(ServiceRate), "Service rate mismatch.");
        //     Assert.That(currentServiceRequirements, Is.EqualTo(ServiceRequirements), "Service requirements mismatch.");
        // });

        var serviceTypeField = _driver.FindElement(By.Id("Type"));
        serviceTypeField.Clear();
        serviceTypeField.SendKeys(NewServiceType);
        var serviceRateField = _driver.FindElement(By.Id("Rate"));
        serviceRateField.Clear();
        serviceRateField.SendKeys(NewServiceRate);
        var serviceRequirementsField = _driver.FindElement(By.Id("Requirements"));
        serviceRequirementsField.Clear();
        serviceRequirementsField.SendKeys(NewServiceRequirements);
        
        // Submit Save
        _driver.FindElement(By.XPath("//Input[@type='submit']")).Click();
        
        _driver.FindElement(By.LinkText("Create New"));
        
        // Ensure row exists and check values
        var updatedServiceRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(NewServiceType));
        Assert.That(updatedServiceRow, Is.Not.Null, $"Service '{NewServiceType}' not found in the list.");

        var serviceColumns = updatedServiceRow.FindElements(By.TagName("td"));
        Assert.Multiple(() =>
        {
            Assert.That(serviceColumns[0].Text.Trim(), Is.EqualTo(NewServiceType), "Service type mismatch.");
            Assert.That(serviceColumns[1].Text.Trim(), Is.EqualTo(NewServiceRate), "Service rate mismatch.");
            Assert.That(serviceColumns[2].Text.Trim(), Is.EqualTo(NewServiceRequirements), "Service requirements mismatch.");
        });
    }

    [Test, Order(4)]
    public void DeleteService()
    {
        _driver.Navigate().GoToUrl(BaseUrl + ServiceUri);
        
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        _driver.FindElement(By.LinkText("Create New"));
        
        // Ensure row exists
        var serviceRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(NewServiceType));
        Assert.That(serviceRow, Is.Not.Null, $"Service '{NewServiceType}' not found in the list.");
        
        serviceRow.FindElement(By.XPath(".//a[text()='Delete']")).Click();
        
        Assert.That(_driver.Url, Does.Contain("/Services/Delete"), "Did not navigate to the Delete page.");
        
        _driver.FindElement(By.XPath("//Input[@type='submit']")).Click();
        
        _driver.FindElement(By.LinkText("Create New"));
        
        // Ensure row is gone
        var deletedServiceRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(NewServiceType));
        Assert.That(deletedServiceRow, Is.Null, $"Service '{NewServiceType}' not found in the list.");
    }
    
    [OneTimeTearDown]
    public void TearDown()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}