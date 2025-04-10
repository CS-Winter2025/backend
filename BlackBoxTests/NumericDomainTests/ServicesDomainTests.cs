namespace BlackBoxTests.NumericDomainTests;

public class ServicesDomainTests
{
    
    NumericDomainUtils numericDomainUtils = new("Services");

    [SetUp]
    public void Setup()
    {
        numericDomainUtils.login();
    }

    [Test]
    public void TestRate()
    {
        numericDomainUtils.TestNumericInput(10, 1000, 0, "Rate");
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        numericDomainUtils.logout();
    }
}
