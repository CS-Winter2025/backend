using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Diagnostics;

namespace BlackBoxTests.SearchBarTests;

public class ServicesSearchBar
{
    private SearchBarUtils searchUtils = new SearchBarUtils(4);

    [SetUp]
    public void Setup()
    {
        searchUtils.login();
    }

    [Test]
    public void SearchByType()
    {
        searchUtils.CheckLetterSearch("Cleaning", 0);
    }

    [Test]
    public void SearchByRate()
    {
        searchUtils.CheckNumericSearch("50.00", 1);
    }

    [Test]
    public void SearchByRequirements()
    {
        searchUtils.CheckLetterSearch("The cleaning requirements", 2);
    }
}
