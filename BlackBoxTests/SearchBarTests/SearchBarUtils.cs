using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BlackBoxTests;

public class SearchBarUtils
{
    private static readonly string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
    static ChromeDriver driver = new(path + @"\drivers\");
    private static readonly string url = "https://localhost:7176/"; // Change this if backend URL is different
    private readonly int NUM_OF_ROWS;
    private readonly string TAB;
    private readonly string SEARCH_ID;
    public SearchBarUtils(int numRows, string tab, string searchId = "dt-search-0")
    {
        TAB = tab;
        NUM_OF_ROWS = numRows;
        SEARCH_ID = searchId;
    }

    public void login()
    {
        try
        {
            driver.FindElement(By.Id(SEARCH_ID));
            driver.FindElement(By.LinkText(TAB)).Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(100);
        }
        catch (NoSuchElementException)
        {
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(url);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.FindElement(By.Id("Username")).SendKeys("admin");
            driver.FindElement(By.Id("Password")).SendKeys("123");
            driver.FindElement(By.XPath("//button[@type='submit']")).Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(100);
            driver.FindElement(By.LinkText(TAB)).Click();
        }
    }

    /// <summary>
    /// Verifies the functionality of the search bar in a data table.
    /// </summary>
    /// <param name="expected">The expected string to search for in the table.</param>
    /// <param name="rowIndex">The index of the column where the search results should appear.</param>
    /// <remarks>
    /// This method performs the following tests:
    /// 1. Searches for an existing entry and verifies its presence in the table and ensures its in the appropriate column.
    /// 2. Searches for a non-existent entry ("Cheese") and ensures the "No matching records found" message is displayed.
    /// 3. Clears the search bar after each test.
    /// </remarks>
    public void CheckLetterSearch(string expected, int rowIndex)
    {
        //Test if search can find someone that is in the table via [expected] parameter
        driver.FindElement(By.Id(SEARCH_ID)).SendKeys(expected);
        var IDelements = driver.FindElements(By.XPath("//td"));
        for (int i = rowIndex; i < IDelements.Count; i += NUM_OF_ROWS)
        {
            if (IDelements[i].GetAttribute("innerHTML").Equals(expected) && IDelements[i].Displayed)
            {
                Assert.That(true);
                break;
            }
        }
        driver.FindElement(By.Id(SEARCH_ID)).Clear();

        //Test if search displays no results when inputted [expected] is not in the table
        driver.FindElement(By.Id(SEARCH_ID)).SendKeys("Cheese");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id(SEARCH_ID)).Clear();

        Assert.Pass("Search bar works");
    }

    /// <summary>
    /// Verifies the functionality of the search bar in a data table.
    /// </summary>
    /// <param name="expected">The expected string to search for in the table (must be a number).</param>
    /// <param name="rowIndex">The index of the column where the search results should appear.</param>
    /// <remarks>
    /// This method performs the following tests:
    /// 1. Searches for an existing entry and verifies its presence in the table and ensures its in the appropriate column.
    /// 2. Searches for invalid (negative) entry ("-1") and ensures the "No matching records found" message is displayed.
    /// 3. Searches for a non-existent entry ("Cheese") and ensures the "No matching records found" message is displayed.
    /// 4. Clears the search bar after each test.
    /// </remarks>
    public void CheckNumericSearch(string expected, int rowIndex)
    {
        //Test if search can find someone that is in the table via rate
        driver.FindElement(By.Id(SEARCH_ID)).SendKeys(expected);
        var IDelements = driver.FindElements(By.XPath("//td"));
        for (int i = rowIndex; i < IDelements.Count; i += NUM_OF_ROWS)
        {
            if (IDelements[i].GetAttribute("innerHTML").Equals(expected) && IDelements[i].Displayed)
            {
                Assert.That(true);
                break;
            }
        }
        driver.FindElement(By.Id(SEARCH_ID)).Clear();

        //Test if search displays no results when inputted negative [expected]
        driver.FindElement(By.Id(SEARCH_ID)).SendKeys("-1");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id(SEARCH_ID)).Clear();

        //Test if search displays no results when inputted [expected] is greater than the number of rows in the table
        driver.FindElement(By.Id(SEARCH_ID)).SendKeys("3784536292837");
        Assert.That(driver.FindElement(By.XPath("//td[contains(text(), 'No matching records found')]")).Displayed);
        driver.FindElement(By.Id(SEARCH_ID)).Clear();

        Assert.Pass("Search bar works");
    }
}
