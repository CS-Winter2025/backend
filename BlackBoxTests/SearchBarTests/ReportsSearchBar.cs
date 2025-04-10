namespace BlackBoxTests.SearchBarTests;

public class ReportsSearchBar
{
    private SearchBarUtils searchUtils = new SearchBarUtils(6, "Reports");

    [SetUp]
    public void Setup()
    {
        searchUtils.login();
    }

    [Test]
    public void SearchById()
    {
        searchUtils.CheckNumericSearch("2", 0);
    }

    [Test]
    public void SearchByName()
    {
        searchUtils.CheckLetterSearch("Alice Smith", 1);
    }

    [Test]
    public void SearchByIsLiving()
    {
        searchUtils.CheckLetterSearch("No", 2);
    }

    [Test]
    public void SearchByServices()
    {
        searchUtils.CheckLetterSearch("Security", 3);
    }

    [Test]
    public void SearchByAge()
    {
        searchUtils.CheckNumericSearch("45", 5);
    }

    [Test]
    public void SearchByEmail()
    {
        searchUtils.CheckLetterSearch("diana@example.com", 5);
    }

    [Test]
    public void SearchByIsMember()
    {
        searchUtils.CheckLetterSearch("true", 5);
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        searchUtils.logout();
    }
}
