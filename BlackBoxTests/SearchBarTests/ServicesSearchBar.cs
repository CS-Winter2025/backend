using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Diagnostics;

namespace BlackBoxTests.SearchBarTests;

public class ServicesSearchBar
{
    private static readonly string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
    static ChromeDriver driver = new(path + @"\drivers\");
    private static readonly string url = "https://localhost:7176/"; // Change this if backend URL is different
    private const int NUM_OF_ROWS = 4;

    [SetUp]
    public void Setup()
    {
        try
        {
            driver.FindElement(By.Id("dt-search-0"));
        }
        catch (NoSuchElementException)
        {
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(url);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.FindElement(By.Id("Username")).SendKeys("nav@gmail.com");
            driver.FindElement(By.Id("Password")).SendKeys("123");
            driver.FindElement(By.XPath("//button[@type='submit']")).Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(100);
            driver.FindElement(By.LinkText("Services")).Click();
        }
    }

    [Test]
    public void SearchByType()
    {
        //Test if search can find someone that is in the table type
        driver.FindElement(By.Id("dt-search-0")).SendKeys("Cleaning");
        var IDelements = driver.FindElements(By.XPath("//td"));
        for (int i = 0; i < IDelements.Count; i += NUM_OF_ROWS)
        {
            if (IDelements[i].GetAttribute("innerHTML").Equals("Cleaning") && IDelements[i].Displayed)
            {
                Assert.That(true);
                break;
            }
        }
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted type is not in the table
        driver.FindElement(By.Id("dt-search-0")).SendKeys("Cheese");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        Assert.Pass("Search bar works");
    }

    [Test]
    public void SearchByRate()
    {
        //Test if search can find someone that is in the table via rate
        driver.FindElement(By.Id("dt-search-0")).SendKeys("50.00");
        var IDelements = driver.FindElements(By.XPath("//td"));
        for (int i = 1; i < IDelements.Count; i += NUM_OF_ROWS)
        {
            if (IDelements[i].GetAttribute("innerHTML").Equals("50.00") && IDelements[i].Displayed)
            {
                Assert.That(true);
                break;
            }
        }
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted negative rate
        driver.FindElement(By.Id("dt-search-0")).SendKeys("-1");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted ID is greater than the number of rows in the table
        driver.FindElement(By.Id("dt-search-0")).SendKeys("3784536292837");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        Assert.Pass("Search bar works");
    }

    [Test]
    public void SearchByRequirements()
    {
        //Test if search can find someone that is in the table via requirements
        driver.FindElement(By.Id("dt-search-0")).SendKeys("The cleaning requirements");
        var IDelements = driver.FindElements(By.XPath("//td"));
        for (int i = 2; i < IDelements.Count; i += NUM_OF_ROWS)
        {
            if (IDelements[i].GetAttribute("innerHTML").Equals("The cleaning requirements") && IDelements[i].Displayed)
            {
                Assert.That(true);
                break;
            }
        }
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted requirements is not in the table
        driver.FindElement(By.Id("dt-search-0")).SendKeys("Cheese");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        Assert.Pass("Search bar works");
    }
}
