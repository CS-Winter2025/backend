namespace BlackBoxTests.NumericDomainTests;

public class EmployeeDomainTests
{
    NumericDomainUtils numericDomainUtils = new("Employees");

    [SetUp]
    public void Setup()
    {
        numericDomainUtils.login();
    }

    [Test]
    public void TestPayRate()
    {
        numericDomainUtils.TestNumericInput(10, 1000, 0, "PayRate");
    }

    [Test]
    public void TestHoursWorked()
    {
        numericDomainUtils.TestNumericInput(10, 1000, 0, "HoursWorked");
    }
    [OneTimeTearDown]
    public void Cleanup()
    {
        numericDomainUtils.logout();
    }
}
