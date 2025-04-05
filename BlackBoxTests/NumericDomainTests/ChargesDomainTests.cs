namespace BlackBoxTests.NumericDomainTests;

public class ChargesDomainTests
{
    NumericDomainUtils numericDomainUtils = new("Charges");

    [SetUp]
    public void Setup()
    {
        numericDomainUtils.login();
    }

    [Test]
    public void TestAmountDue()
    {
        numericDomainUtils.TestNumericInput(1, 1000, 0, "AmountDue");
    }

    [Test]
    public void TestAmountPaid()
    {
        numericDomainUtils.TestNumericInput(1, 1000, 0, "AmountPaid");
    }
}
