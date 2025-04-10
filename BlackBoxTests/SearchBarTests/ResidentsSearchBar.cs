namespace BlackBoxTests.SearchBarTests;

public class ResidentsSearchBar
{
    private SearchBarUtils searchUtils3 = new SearchBarUtils(2, "Residents", "dt-search-2");
    
    [SetUp]
    public void Setup()
    {
        searchUtils3.login();
    }

    [Test]
    public void SearchByName()
    {
        searchUtils3.CheckLetterSearch("Charlie", 0);
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        searchUtils3.logout();
    }
}
