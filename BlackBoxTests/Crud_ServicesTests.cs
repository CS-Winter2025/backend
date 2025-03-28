using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BlackBoxTests;

public class Crud_ServicesTests
{
    private ChromeDriver _driver;
    private const string BaseUrl = "http://localhost:5072/"; // Change as needed
    
    [OneTimeSetUp]
    public void Setup()
    {
        _driver = new ChromeDriver();
        _driver.Manage().Window.Maximize();
    }

    [Test]
    public void CreateService()
    {
        const string serviceUri = "Services?area=Services";
        _driver.Navigate().GoToUrl(BaseUrl + serviceUri);
        
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        _driver.FindElement(By.LinkText("Create New")).Click();
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        
        _driver.FindElement(By.Id("Name")).SendKeys("Coaching");
        _driver.FindElement(By.Id("Rate")).SendKeys("");
        _driver.FindElement(By.Id("Address")).SendKeys("Your House");
    }
    
    [OneTimeTearDown]
    public void TearDown()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}