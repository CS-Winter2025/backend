using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Diagnostics;

namespace BlackBoxTests.SearchBarTests;

public class EmployeeSearchBar
{

    private SearchBarUtils searchUtils = new SearchBarUtils(11);

    [SetUp]
    public void Setup()
    {
        searchUtils.login();
    }

    [Test]
    public void SearchByName()
    {
        searchUtils.CheckLetterSearch("Alice", 0);
    }

    [Test]
    public void SearchByManager()
    {
        searchUtils.CheckNumericSearch("2", 1);
    }

    [Test]
    public void SearchByJobTitle()
    {
        searchUtils.CheckLetterSearch("Dev", 2);
    }

    [Test]
    public void SearchByEmploymentType()
    {
        searchUtils.CheckLetterSearch("Part-Time", 3);
    }

    [Test]
    public void SearchByPayRate()
    {
        searchUtils.CheckNumericSearch("1000000.00", 4);
    }

    [Test]
    public void SearchByHoursWorked()
    {
        searchUtils.CheckNumericSearch("9001", 6);
    }

    [Test]
    public void SearchByCertifications()
    {
        searchUtils.CheckLetterSearch("CEO", 7);
    }

    [Test]
    public void SearchByOrganization()
    {
        searchUtils.CheckNumericSearch("1", 8);
    }

    [Test]
    public void SearchByDetails()
    {
        searchUtils.CheckLetterSearch("details", 9);
    }
}
