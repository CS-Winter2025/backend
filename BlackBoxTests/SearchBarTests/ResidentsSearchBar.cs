namespace BlackBoxTests.SearchBarTests;

public class ResidentsSearchBar
{
    //private SearchBarUtils searchUtils1 = new SearchBarUtils(3, "Residents");
    //private SearchBarUtils searchUtils2 = new SearchBarUtils(3, "Residents", "dt-search-1");
    private SearchBarUtils searchUtils3 = new SearchBarUtils(3, "Residents", "dt-search-2");
    
    [SetUp]
    public void Setup()
    {
        searchUtils3.login();
    }

    [Test]
    public void SearchByName()
    {
        //searchUtils1.CheckLetterSearch("Charlie", 0);
        //searchUtils2.CheckLetterSearch("Charlie", 0);
        searchUtils3.CheckLetterSearch("Charlie", 0);
    }

    [Test]
    public void SearchByIsLiving()
    {
        //searchUtils1.CheckLetterSearch("Charlie", 1);
        searchUtils3.CheckLetterSearch("True", 1);
    }

    [Test]
    public void SearchByWasLiving()
    {
        //searchUtils1.CheckLetterSearch("Charlie", 1);
        searchUtils3.CheckLetterSearch("True", 1);
    }
}
