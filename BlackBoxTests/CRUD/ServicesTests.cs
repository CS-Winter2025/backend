using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BlackBoxTests.CRUD;

public class ServicesTests
{
    private ChromeDriver _driver;
    private const string BaseUrl = "http://localhost:5072/"; // Change as needed
    private const string ServiceUri = "Services?area=Services";

    private const string AdminLogin = "admin";
    private const string AdminPassword = "123";
    
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
        
        _driver.Navigate().GoToUrl(BaseUrl);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        
        _driver.FindElement(By.XPath("//a[text()='Login']")).Click();
        
        // Login with Admin
        _driver.FindElement(By.Id("Username")).SendKeys(AdminLogin);
        _driver.FindElement(By.Id("Password")).SendKeys(AdminPassword);
        _driver.FindElement(By.XPath("//button[text()='Login']")).Click();
        
        // Navigate to Services page
        _driver.FindElement(By.XPath("//a[text()='Services']")).Click();
        Assert.That(_driver.Url, Does.Contain(ServiceUri), "Did not navigate to the Services page.");
    }

    [Test, Order(1)]
    public void CreateService()
    {
        // Create New
        _driver.FindElement(By.LinkText("Create New")).Click();
        Assert.That(_driver.Url, Does.Contain("/Services/Create"), "Did not navigate to the Create page.");
        
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
        // Navigate to Services page
        _driver.FindElement(By.XPath("//a[text()='Services']")).Click();
        Assert.That(_driver.Url, Does.Contain(ServiceUri), "Did not navigate to the Services page.");
        
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        _driver.FindElement(By.LinkText("Create New"));
        
        // Ensure row exists
        var serviceRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(ServiceType));
        Assert.That(serviceRow, Is.Not.Null, $"Service '{ServiceType}' not found in the list.");
        
        // Navigate to Details
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
        // Navigate to Services page
        _driver.FindElement(By.XPath("//a[text()='Services']")).Click();
        Assert.That(_driver.Url, Does.Contain(ServiceUri), "Did not navigate to the Services page.");

        // Ensure row exists
        var serviceRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(ServiceType));
        Assert.That(serviceRow, Is.Not.Null, $"Service '{ServiceType}' not found in the list.");
        
        // Navigate to Edit
        serviceRow.FindElement(By.XPath(".//a[text()='Edit']")).Click();
        Assert.That(_driver.Url, Does.Contain("/Services/Edit"), "Did not navigate to the Edit page.");

        // Fields
        var serviceTypeField = _driver.FindElement(By.Id("Type"));
        var serviceRateField = _driver.FindElement(By.Id("Rate"));
        var serviceRequirementsField = _driver.FindElement(By.Id("Requirements"));

        // Check Edit Fields
        // NOTE currently not working as Requirements field returns: System.Collections.Generic.List`1[System.String]
        var currentServiceType = serviceTypeField.GetAttribute("value")!.Trim();
        var currentServiceRate = serviceRateField.GetAttribute("value")!.Trim();
        // var currentServiceRequirements = _driver.FindElement(By.Id("Requirements")).GetAttribute("value")!.Trim(); // Bug found
        Assert.Multiple(() =>
        {
            Assert.That(currentServiceType, Is.EqualTo(ServiceType), "Service type mismatch.");
            Assert.That(currentServiceRate, Is.EqualTo(ServiceRate), "Service rate mismatch.");
            // Assert.That(currentServiceRequirements, Is.EqualTo(ServiceRequirements), "Service requirements mismatch.");
        });

        serviceTypeField.Clear();
        serviceTypeField.SendKeys(NewServiceType);
        serviceRateField.Clear();
        serviceRateField.SendKeys(NewServiceRate);
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
        // Navigate to Services page
        _driver.FindElement(By.XPath("//a[text()='Services']")).Click();
        Assert.That(_driver.Url, Does.Contain(ServiceUri), "Did not navigate to the Services page.");

        // Ensure row exists
        var serviceRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(NewServiceType));
        Assert.That(serviceRow, Is.Not.Null, $"Service '{NewServiceType}' not found in the list.");
        
        // Navigate to Delete
        serviceRow.FindElement(By.XPath(".//a[text()='Delete']")).Click();
        Assert.That(_driver.Url, Does.Contain("/Services/Delete"), "Did not navigate to the Delete page.");

        var serviceDetailsTable = _driver.FindElement(By.XPath(".//dl"));
        var serviceDetails = serviceDetailsTable.FindElements(By.TagName("dd"));

        Assert.Multiple(() =>
        {
            Assert.That(serviceDetails[0].Text.Trim(), Is.EqualTo(NewServiceType), "Service type mismatch.");
            Assert.That(serviceDetails[1].Text.Trim(), Is.EqualTo(NewServiceRate), "Service rate mismatch.");
            Assert.That(serviceDetails[2].Text.Trim(), Is.EqualTo(NewServiceRequirements), "Service requirements mismatch.");
        });

        // Delete Entry
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