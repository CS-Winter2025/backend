namespace BlackBoxTests.SearchBarTests;

public class ChargesSearchBar
{
    private SearchBarUtils searchUtils = new SearchBarUtils(5, "Charges");

    [SetUp]
    public void Setup()
    {
        searchUtils.login();
    }

    [Test]
    public void SearchByResident()
    {
        searchUtils.CheckLetterSearch("1", 0);
    }

    [Test]
    public void SearchByDate()
    {
        searchUtils.CheckLetterSearch("2025-02-21 2:00:00 PM", 1);
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

    [OneTimeTearDown]
    public void Cleanup()
    {
        searchUtils.logout();
    }
}
