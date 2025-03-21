using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using NUnit.Framework.Legacy;

public class SeleniumTests
{
    [Test]
    public void OpenGoogleTest()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");

        string driverPath = Environment.GetEnvironmentVariable("CHROME_DRIVER_PATH");

        using var driver = new ChromeDriver(driverPath, options);
        driver.Navigate().GoToUrl("https://www.google.com");
        StringAssert.Contains("Google", driver.Title);
    }
}