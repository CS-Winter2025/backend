using NUnit.Framework.Constraints;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V132.Security;

namespace BlackBoxTests;

public class ServicesDomainTests
{
    ChromeDriver driver = new ChromeDriver();
    private static readonly string url = "https://localhost:7176/"; // Change this if backend URL is different

    [SetUp]
    public void Setup()
    {
        driver.Manage().Window.Maximize();
        driver.Navigate().GoToUrl(url);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        driver.FindElement(By.Id("Username")).SendKeys("admin");
        driver.FindElement(By.Id("Password")).SendKeys("123");
        driver.FindElement(By.XPath("//button[@type='submit']")).Click();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(100);
        driver.FindElement(By.LinkText("Services")).Click();
    }

    [Test]
    public void RateTest()
    {
        driver.FindElement(By.LinkText("Edit")).Click();

        //Test rate that is too high
        driver.FindElement(By.Id("Rate")).Clear();
        driver.FindElement(By.Id("Rate")).SendKeys("100");
        driver.FindElement(By.XPath("//button[@type='submit']")).Click();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        try
        {
            Assert.That(driver.FindElement(By.LinkText("Back to List")).Displayed);
        }
        catch (NoSuchElementException e)
        {
            Assert.Fail("Input allows values above X");
        }
        driver.FindElement(By.Id("Rate")).Clear();

        //Test rate that is too low
        driver.FindElement(By.Id("Rate")).Clear();
        driver.FindElement(By.Id("Rate")).SendKeys("1");
        driver.FindElement(By.XPath("//button[@type='submit']")).Click();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        try
        {
            Assert.That(driver.FindElement(By.LinkText("Back to List")).Displayed);
        }
        catch (NoSuchElementException e)
        {
            Assert.Fail("Input allows values below X");
        }

        //Test rate that is negative
        driver.FindElement(By.Id("Rate")).Clear();
        driver.FindElement(By.Id("Rate")).SendKeys("100");
        driver.FindElement(By.XPath("//button[@type='submit']")).Click();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        try
        {
            Assert.That(driver.FindElement(By.LinkText("Back to List")).Displayed);
        }
        catch (NoSuchElementException e)
        {
            Assert.Fail("Input allows negative values");
        }

        //Test rate that is valid
        driver.FindElement(By.Id("Rate")).Clear();
        driver.FindElement(By.Id("Rate")).SendKeys("100");
        driver.FindElement(By.XPath("//button[@type='submit']")).Click();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        try
        {
            Assert.That(driver.FindElement(By.LinkText("Back to List")).Displayed);
        }
        catch (NoSuchElementException e)
        {
            Assert.Fail("Something wrong with numeric input");
        }
        Assert.Pass("Input validation works");
    }

    [OneTimeTearDown]
    public void CloseChromeDrivers()
    {
        driver.Quit();
        driver.Dispose();
    }
}
