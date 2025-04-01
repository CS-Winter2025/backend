using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Diagnostics;

namespace BlackBoxTests.SearchBarTests;

public class EmployeeSearchBar
{
    private static readonly string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
    static ChromeDriver driver = new(path + @"\drivers\");
    private static readonly string url = "https://localhost:7176/"; // Change this if backend URL is different
    private const int NUM_OF_ROWS = 11;

    [SetUp]
    public void Setup()
    {
        try
        {
            driver.FindElement(By.Id("dt-search-0"));
        } catch(NoSuchElementException)
        {
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(url);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.FindElement(By.Id("Username")).SendKeys("nav@gmail.com");
            driver.FindElement(By.Id("Password")).SendKeys("123");
            driver.FindElement(By.XPath("//button[@type='submit']")).Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(100);
            driver.FindElement(By.LinkText("Employees")).Click();
        }
    }

    [Test]
    public void SearchByName()
    {
        //Test if search can find someone that is in the table via name
        driver.FindElement(By.Id("dt-search-0")).SendKeys("Alice");
        var IDelements = driver.FindElements(By.XPath("//td"));
        for (int i = 0; i < IDelements.Count; i += NUM_OF_ROWS)
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
    public void SearchByManager()
    {
        //Test if search can find someone that is in the table via Manager ID
        driver.FindElement(By.Id("dt-search-0")).SendKeys("2");
        var IDelements = driver.FindElements(By.XPath("//td"));
        for (int i = 1; i < IDelements.Count; i += NUM_OF_ROWS)
        {
            if (IDelements[i].GetAttribute("innerHTML").Equals("2") && IDelements[i].Displayed)
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
    public void SearchByJobTitle()
    {
        //Test if search can find someone that is in the table via job title
        driver.FindElement(By.Id("dt-search-0")).SendKeys("Dev");
        var IDelements = driver.FindElements(By.XPath("//td"));
        for (int i = 2; i < IDelements.Count; i += NUM_OF_ROWS)
        {
            if (IDelements[i].GetAttribute("innerHTML").Equals("Dev") && IDelements[i].Displayed)
            {
                Assert.That(true);
                break;
            }
        }
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted job title is not in the table
        driver.FindElement(By.Id("dt-search-0")).SendKeys("Cheese");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        Assert.Pass("Search bar works");
    }

    [Test]
    public void SearchByEmploymentType()
    {
        //Test if search can find someone that is in the table via emplpyment type
        driver.FindElement(By.Id("dt-search-0")).SendKeys("Part-Time");
        var IDelements = driver.FindElements(By.XPath("//td"));
        for (int i = 3; i < IDelements.Count; i += NUM_OF_ROWS)
        {
            if (IDelements[i].GetAttribute("innerHTML").Equals("Part-Time") && IDelements[i].Displayed)
            {
                Assert.That(true);
                break;
            }
        }
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted invalid emplpyment type
        driver.FindElement(By.Id("dt-search-0")).SendKeys("Cheese");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        Assert.Pass("Search bar works");
    }

    [Test]
    public void SearchByPayRate()
    {
        //Test if search can find someone that is in the table via Manager ID
        driver.FindElement(By.Id("dt-search-0")).SendKeys("1000000.00");
        var IDelements = driver.FindElements(By.XPath("//td"));
        for (int i = 4; i < IDelements.Count; i += NUM_OF_ROWS)
        {
            if (IDelements[i].GetAttribute("innerHTML").Equals("1000000.00") && IDelements[i].Displayed)
            {
                Assert.That(true);
                break;
            }
        }
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted negative pay rate
        driver.FindElement(By.Id("dt-search-0")).SendKeys("-1");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted pay rate is greater than the number of rows in the table
        driver.FindElement(By.Id("dt-search-0")).SendKeys("384973442322332");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        Assert.Pass("Search bar works");
    }

    [Test]
    public void SearchByHoursWorked()
    {
        //Test if search can find someone that is in the table via hours worked
        driver.FindElement(By.Id("dt-search-0")).SendKeys("9001");
        var IDelements = driver.FindElements(By.XPath("//td"));
        for (int i = 6; i < IDelements.Count; i += NUM_OF_ROWS)
        {
            if (IDelements[i].GetAttribute("innerHTML").Equals("9001") && IDelements[i].Displayed)
            {
                Assert.That(true);
                break;
            }
        }
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted negative hours worked
        driver.FindElement(By.Id("dt-search-0")).SendKeys("-1");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted hours worked is greater than the number of rows in the table
        driver.FindElement(By.Id("dt-search-0")).SendKeys("384973442322332");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        Assert.Pass("Search bar works");
    }

    [Test]
    public void SearchByCertifications()
    {
        //Test if search can find someone that is in the table via certifications
        driver.FindElement(By.Id("dt-search-0")).SendKeys("CEO");
        var IDelements = driver.FindElements(By.XPath("//td"));
        for (int i = 7; i < IDelements.Count; i += NUM_OF_ROWS)
        {
            if (IDelements[i].GetAttribute("innerHTML").Equals("CEO") && IDelements[i].Displayed)
            {
                Assert.That(true);
                break;
            }
        }
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted invalid certifications
        driver.FindElement(By.Id("dt-search-0")).SendKeys("Cheese");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        Assert.Pass("Search bar works");
    }

    [Test]
    public void SearchByOrganization()
    {
        //Test if search can find someone that is in the table via organization
        driver.FindElement(By.Id("dt-search-0")).SendKeys("1");
        var IDelements = driver.FindElements(By.XPath("//td"));
        for (int i = 8; i < IDelements.Count; i += NUM_OF_ROWS)
        {
            if (IDelements[i].GetAttribute("innerHTML").Equals("1") && IDelements[i].Displayed)
            {
                Assert.That(true);
                break;
            }
        }
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted negative organization
        driver.FindElement(By.Id("dt-search-0")).SendKeys("-1");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted organization is greater than the number of rows in the table
        driver.FindElement(By.Id("dt-search-0")).SendKeys("384973442322332");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        Assert.Pass("Search bar works");
    }

    [Test]
    public void SearchByDetails()
    {
        //Test if search can find someone that is in the table via details
        driver.FindElement(By.Id("dt-search-0")).SendKeys("details");
        var IDelements = driver.FindElements(By.XPath("//td"));
        for (int i = 9; i < IDelements.Count; i += NUM_OF_ROWS)
        {
            if (IDelements[i].GetAttribute("innerHTML").Equals("details") && IDelements[i].Displayed)
            {
                Assert.That(true);
                break;
            }
        }
        driver.FindElement(By.Id("dt-search-0")).Clear();

        //Test if search displays no results when inputted invalid details
        driver.FindElement(By.Id("dt-search-0")).SendKeys("Cheese");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id("dt-search-0")).Clear();

        Assert.Pass("Search bar works");
    }
}
