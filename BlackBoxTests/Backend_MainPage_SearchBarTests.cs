using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Diagnostics;

namespace BlackBoxTests;

public class Backend_MainPage_SearchBarTests
{
    private static readonly string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
    static ChromeDriver driver = new(path + @"\drivers\");
    private static readonly string url = "https://localhost:7176/"; // Change this if backend URL is different

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void SearchByFirstName()
    {
        driver.Manage().Window.Maximize();
        driver.Navigate().GoToUrl(url);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

        //Test if search can find someone that is in the table via first name
        driver.FindElement(By.Id("dt-search-0")).SendKeys("Alice");
        var IDelements = driver.FindElements(By.XPath("//td"));
        for (int i = 1; i < IDelements.Count; i += 5)
        {
            if (IDelements[i].GetAttribute("innerHTML").Equals("Alice") && IDelements[i].Displayed)
            {
                Assert.That(true);
                break;
            }
        }
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted first name is not in the table
        driver.FindElement(By.Id("dt-search-0")).SendKeys("Cheese");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        Assert.Pass("Search bar works");
    }

    [Test]
    public void SearchByLastName()
    {
        driver.Manage().Window.Maximize();
        driver.Navigate().GoToUrl(url);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

        //Test if search can find someone that is in the table via last name
        driver.FindElement(By.Id("dt-search-0")).SendKeys("Doe");
        var IDelements = driver.FindElements(By.XPath("//td"));
        for (int i = 2; i < IDelements.Count; i += 5)
        {
            if (IDelements[i].GetAttribute("innerHTML").Equals("Alice") && IDelements[i].Displayed)
            {
                Assert.That(true);
                break;
            }
        }
        driver.FindElement(By.Id("dt-search-0")).Clear();
        
        //Test if search displays no results when inputted last name is not in the table
        driver.FindElement(By.Id("dt-search-0")).SendKeys("Cheese");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();
        
        Assert.Pass("Search bar works");
    }


    [Test]
    public void SearchById()
    {
        driver.Manage().Window.Maximize();
        driver.Navigate().GoToUrl(url);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

        //Test if search can find someone that is in the table via ID
        driver.FindElement(By.Id("dt-search-0")).SendKeys("3");
        var IDelements = driver.FindElements(By.XPath("//td"));
        for (int i = 0; i < IDelements.Count; i+=5)
        {
            if (IDelements[i].GetAttribute("innerHTML").Equals("3") && IDelements[i].Displayed)
            {
                Assert.That(true);
                break;
            }
        }
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted negative ID
        driver.FindElement(By.Id("dt-search-0")).SendKeys("-1");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted ID is greater than the number of rows in the table
        driver.FindElement(By.Id("dt-search-0")).SendKeys("9000");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        Assert.Pass("Search bar works");
    }

    [Test]
    public void SearchByAge()
    {
        driver.Manage().Window.Maximize();
        driver.Navigate().GoToUrl(url);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

        //Test if search can find someone that is in the table via age
        driver.FindElement(By.Id("dt-search-0")).SendKeys("33");
        var IDelements = driver.FindElements(By.XPath("//td"));
        for (int i = 3; i < IDelements.Count; i += 5)
        {
            if (IDelements[i].GetAttribute("innerHTML").Equals("33") && IDelements[i].Displayed)
            {
                Assert.That(true);
                break;
            }
        }
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted negative age
        driver.FindElement(By.Id("dt-search-0")).SendKeys("-1");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted age is greater than the number of rows in the table
        driver.FindElement(By.Id("dt-search-0")).SendKeys("9000");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        Assert.Pass("Search bar works");
    }

    [Test]
    public void SearchByEmail()
    {
        driver.Manage().Window.Maximize();
        driver.Navigate().GoToUrl(url);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

        //Test if search can find someone that is in the table via email
        driver.FindElement(By.Id("dt-search-0")).SendKeys("john.doe@example.com");
        var IDelements = driver.FindElements(By.XPath("//td"));
        for (int i = 4; i < IDelements.Count; i += 5)
        {
            if (IDelements[i].GetAttribute("innerHTML").Equals("john.doe@example.com") && IDelements[i].Displayed)
            {
                Assert.That(true);
                break;
            }
        }
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted email is not in the table
        driver.FindElement(By.Id("dt-search-0")).SendKeys("cheese.doe@example.com");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        Assert.Pass("Search bar works");
    }
}
