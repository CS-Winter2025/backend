namespace BlackBoxTests.SearchBarTests;

public class ChargesSearchBar
{
    private SearchBarUtils searchUtils = new SearchBarUtils(5);

    [SetUp]
    public void Setup()
    {
        searchUtils.login();
    }

    [Test]
    public void SearchByResident()
    {
        searchUtils.CheckNumericSearch("1", 0);
    }

    [Test]
    public void SearchByDate()
    {
        searchUtils.CheckLetterSearch("2025-03-31 10:09:02 PM", 1);
    }

    [Test]
    public void SearchByAmountDue()
    {
        searchUtils.CheckNumericSearch("200.00", 2);
    }

    [Test]
    public void SearchByAmountPaid()
    {
        searchUtils.CheckNumericSearch("100.00", 3);
    }
}
