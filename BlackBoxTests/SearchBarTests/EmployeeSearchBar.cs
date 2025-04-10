using OpenQA.Selenium;

namespace BlackBoxTests.SearchBarTests;

public class EmployeeSearchBar
{

    private SearchBarUtils searchUtils = new SearchBarUtils(12, "Employees");

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
        searchUtils.CheckNumericSearch("1", 1);
    }

    [Test]
    public void SearchByJobTitle()
    {
        searchUtils.CheckLetterSearch("HR Specialist", 2);
    }

    [Test]
    public void SearchByEmploymentType()
    {
        searchUtils.CheckLetterSearch("Part-Time", 3);
    }

    [Test]
    public void SearchByPayRate()
    {
        searchUtils.CheckNumericSearch("53000.00", 4);
    }

    //[Test]
    //public void SearchByHoursWorked()
    //{
    //    string hours = "222";
    //    searchUtils.insertSampleData("HoursWorked", hours);
    //    searchUtils.CheckNumericSearch(hours, 6);
    //}

    //[Test]
    //public void SearchByCertifications()
    //{
    //    string cert = "First-aid";
    //    searchUtils.insertSampleData("Certifications", cert);
    //    searchUtils.CheckLetterSearch(cert, 7);
    //}

    [Test]
    public void SearchByOrganization()
    {
        searchUtils.CheckNumericSearch("1", 8);
    }

    [Test]
    public void SearchByPhoto()
    {
        searchUtils.CheckLetterSearch("No photo", 10);
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        searchUtils.logout();
    }
}
