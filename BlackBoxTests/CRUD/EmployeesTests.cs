using BlackBoxTests.Data;
using BlackBoxTests.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace BlackBoxTests.CRUD;

public class EmployeesTests
{
    private ChromeDriver _driver;
    private Setup _setup;
    private Auth _auth;
    private Navigation _nav;
    private ElementActions _elementActions;

    private const string FirstName = "John";
    private const string MiddleName = "Paul";
    private const string LastName = "Doe";
    private const string Street = "123 Example St.";
    private const string City = "Whoville";
    private const string State = "WA";
    private const string Country = "United States";
    private const string ZipCode = "98001";
    private const string ManagerId = "1";
    private const string JobTitle = "Cleaner";
    private const string EmploymentType = "Full-Time";
    private const string PayRate = "40000.00";
    private const string Availability = "8";
    private const string HoursWorked = "4";
    private const string Certifications = "Janitor";
    private const string OrganizationId = "1";

    private const string NewFirstName = "Sarah";
    private const string NewMiddleName = "John";
    private const string NewLastName = "Smith";
    private const string NewStreet = "1 Example St.";
    private const string NewCity = "Los Angeles";
    private const string NewState = "CA";
    private const string NewCountry = "US";
    private const string NewZipCode = "12304";
    private const string NewManagerId = "2";
    private const string NewJobTitle = "Server";
    private const string NewEmploymentType = "Part-Time";
    private const string NewPayRate = "50000.00";
    private const string NewAvailability = "4";
    private const string NewHoursWorked = "2";
    private const string NewCertifications = "Food Safe";
    private const string NewOrganizationId = "2";

    [OneTimeSetUp]
    public void Setup()
    {
        _setup = new Setup();
        _driver = _setup.Driver;
        _auth = new Auth(_driver);
        _nav = new Navigation(_driver);
        _elementActions = new ElementActions(_driver);

        _setup.Initialize();
        _nav.ToLogin();
        _auth.LoginAsUser(Users.AdminUsername, Users.AdminPassword);

        _nav.ToEmployees();
    }

    [Test, Order(1)]
    public void CreateEmployee()
    {
        _nav.ToCreateNew(Uris.EmployeesCreate);

        // Input Values
        _elementActions.FillTextField("Name_FirstName", FirstName);
        _elementActions.FillTextField("Name_MiddleName", MiddleName);
        _elementActions.FillTextField("Name_LastName", LastName);
        _elementActions.FillTextField("Address_Street", Street);
        _elementActions.FillTextField("Address_City", City);
        _elementActions.FillTextField("Address_State", State);
        _elementActions.FillTextField("Address_Country", Country);
        _elementActions.FillTextField("Address_ZipCode", ZipCode);
        _elementActions.SelectDropdownItem("ManagerId", ManagerId);
        _elementActions.FillTextField("JobTitle", JobTitle);
        _elementActions.FillTextField("EmploymentType", EmploymentType);
        _elementActions.FillTextField("PayRate", PayRate);
        _elementActions.FillTextField("Availability", Availability);
        _elementActions.FillTextField("HoursWorked", HoursWorked);
        _elementActions.FillTextField("Certifications", Certifications);
        _elementActions.SelectDropdownItem("OrganizationId", OrganizationId);

        var submitCreate = _driver.FindElement(By.XPath(XPaths.SubmitBtn));
        new Actions(_driver)
            .MoveToElement(submitCreate)
            .Click()
            .Perform();

        _nav.ToEmployees();

        // Ensure row exists and check values
        var fullName = $"{FirstName} {MiddleName} {LastName}";
        _elementActions.SearchByValue(fullName);
        var employeeRow = _elementActions.CheckRowContaining(fullName);

        var employeeDetails = employeeRow.FindElements(By.TagName("td"));
        Assert.Multiple(() =>
        {
            Assert.That(employeeDetails[0].Text.Trim(), Is.EqualTo(fullName), "Name mismatch!");
            Assert.That(employeeDetails[1].Text.Trim(), Is.EqualTo(ManagerId), "Manager mismatch!");
            Assert.That(employeeDetails[2].Text.Trim(), Is.EqualTo(JobTitle), "JobTitle mismatch!");
            Assert.That(employeeDetails[3].Text.Trim(), Is.EqualTo(EmploymentType), "EmploymentType mismatch!");
            Assert.That(employeeDetails[4].Text.Trim(), Is.EqualTo(PayRate), "PayRate mismatch!");
            Assert.That(employeeDetails[5].Text.Trim(), Is.EqualTo(Availability), "Availability mismatch!");
            Assert.That(employeeDetails[6].Text.Trim(), Is.EqualTo(HoursWorked), "HoursWorked mismatch!");
            Assert.That(employeeDetails[7].Text.Trim(), Is.EqualTo(Certifications), "Certifications mismatch!");
            Assert.That(employeeDetails[8].Text.Trim(), Is.EqualTo(OrganizationId), "Organization mismatch!");
        });
    }

    [Test, Order(2)]
    public void ReadEmployee()
    {
        _nav.ToEmployees();

        // Ensure row exists
        var fullName = $"{FirstName} {MiddleName} {LastName}";
        _elementActions.SearchByValue(fullName);
        var employeeRow = _elementActions.CheckRowContaining(fullName);

        _nav.ToDetails(employeeRow, Uris.EmployeesDetails);

        var employeeDetailsTable = _driver.FindElement(By.XPath(".//dl"));
        var employeeDetails = employeeDetailsTable.FindElements(By.TagName("dd"));

        var address = $"{Street}, {City}, {State} {ZipCode}, {Country}";

        Assert.Multiple(() =>
        {
            Assert.That(employeeDetails[0].Text.Trim(), Is.EqualTo(fullName), "Name mismatch!");
            Assert.That(employeeDetails[1].Text.Trim(), Is.EqualTo(address), "Address mismatch!");
            Assert.That(employeeDetails[2].Text.Trim(), Is.EqualTo(ManagerId), "Manager mismatch!");
            Assert.That(employeeDetails[3].Text.Trim(), Is.EqualTo(JobTitle), "JobTitle mismatch!");
            Assert.That(employeeDetails[4].Text.Trim(), Is.EqualTo(EmploymentType), "EmploymentType mismatch!");
            Assert.That(employeeDetails[5].Text.Trim(), Is.EqualTo(PayRate), "PayRate mismatch!");
            Assert.That(employeeDetails[6].Text.Trim(), Is.EqualTo(Availability), "Availability mismatch!");
            Assert.That(employeeDetails[7].Text.Trim(), Is.EqualTo(HoursWorked), "HoursWorked mismatch!");
            Assert.That(employeeDetails[8].Text.Trim(), Is.EqualTo(Certifications), "Certifications mismatch!");
            Assert.That(employeeDetails[9].Text.Trim(), Is.EqualTo(OrganizationId), "Organization mismatch!");
        });
    }

    [Test, Order(3)]
    public void UpdateEmployee()
    {
        _nav.ToEmployees();

        // Ensure row exists
        var fullName = $"{FirstName} {MiddleName} {LastName}";
        _elementActions.SearchByValue(fullName);
        var employeeRow = _elementActions.CheckRowContaining(fullName);

        _nav.ToEdit(employeeRow, Uris.EmployeesEdit);

        // Verify Current Values
        _elementActions.VerifyTextField("Name_FirstName", FirstName, "FirstName mismatch!");
        _elementActions.VerifyTextField("Name_MiddleName", MiddleName, "MiddleName mismatch!");
        _elementActions.VerifyTextField("Name_LastName", LastName, "LastName mismatch!");
        _elementActions.VerifyTextField("Address_Street", Street, "Street mismatch!");
        _elementActions.VerifyTextField("Address_City", City, "City mismatch!");
        _elementActions.VerifyTextField("Address_State", State, "State mismatch!");
        _elementActions.VerifyTextField("Address_Country", Country, "Country mismatch!");
        _elementActions.VerifyTextField("Address_ZipCode", ZipCode, "ZipCode mismatch!");
        _elementActions.VerifyDropdownItem("ManagerId", ManagerId, "ManagerId mismatch!");
        _elementActions.VerifyTextField("JobTitle", JobTitle, "JobTitle mismatch!");
        _elementActions.VerifyTextField("EmploymentType", EmploymentType, "EmploymentType mismatch!");
        _elementActions.VerifyTextField("PayRate", PayRate, "PayRate mismatch!");
        _elementActions.VerifyTextField("Availability", Availability, "Availability mismatch!");
        _elementActions.VerifyTextField("HoursWorked", HoursWorked, "HoursWorked mismatch!");
        _elementActions.VerifyTextField("Certifications", Certifications, "Certifications mismatch!");
        _elementActions.VerifyDropdownItem("OrganizationId", OrganizationId, "OrganizationId mismatch!");

        // Input New Values
        _elementActions.FillTextField("Name_FirstName", NewFirstName);
        _elementActions.FillTextField("Name_MiddleName", NewMiddleName);
        _elementActions.FillTextField("Name_LastName", NewLastName);
        _elementActions.FillTextField("Address_Street", NewStreet);
        _elementActions.FillTextField("Address_City", NewCity);
        _elementActions.FillTextField("Address_State", NewState);
        _elementActions.FillTextField("Address_Country", NewCountry);
        _elementActions.FillTextField("Address_ZipCode", NewZipCode);
        _elementActions.SelectDropdownItem("ManagerId", NewManagerId);
        _elementActions.FillTextField("JobTitle", NewJobTitle);
        _elementActions.FillTextField("EmploymentType", NewEmploymentType);
        _elementActions.FillTextField("PayRate", NewPayRate);
        _elementActions.FillTextField("Availability", NewAvailability);
        _elementActions.FillTextField("HoursWorked", NewHoursWorked);
        _elementActions.FillTextField("Certifications", NewCertifications);
        _elementActions.SelectDropdownItem("OrganizationId", NewOrganizationId);

        var submitEdit = _driver.FindElement(By.XPath(XPaths.SubmitBtn));
        new Actions(_driver)
            .MoveToElement(submitEdit)
            .Click()
            .Perform();

        _nav.ToEmployees();

        // Ensure row exists and check values
        var updatedFullName = $"{NewFirstName} {NewMiddleName} {NewLastName}";
        _elementActions.SearchByValue(updatedFullName);
        var updatedEmployeeRow = _elementActions.CheckRowContaining(updatedFullName);

        var employeeDetails = updatedEmployeeRow.FindElements(By.TagName("td"));
        Assert.Multiple(() =>
        {
            Assert.That(employeeDetails[0].Text.Trim(), Is.EqualTo(updatedFullName), "Name mismatch!");
            Assert.That(employeeDetails[1].Text.Trim(), Is.EqualTo(NewManagerId), "Manager mismatch!");
            Assert.That(employeeDetails[2].Text.Trim(), Is.EqualTo(NewJobTitle), "JobTitle mismatch!");
            Assert.That(employeeDetails[3].Text.Trim(), Is.EqualTo(NewEmploymentType), "EmploymentType mismatch!");
            Assert.That(employeeDetails[4].Text.Trim(), Is.EqualTo(NewPayRate), "PayRate mismatch!");
            Assert.That(employeeDetails[5].Text.Trim(), Is.EqualTo(NewAvailability), "Availability mismatch!");
            Assert.That(employeeDetails[6].Text.Trim(), Is.EqualTo(NewHoursWorked), "HoursWorked mismatch!");
            Assert.That(employeeDetails[7].Text.Trim(), Is.EqualTo(NewCertifications), "Certifications mismatch!");
            Assert.That(employeeDetails[8].Text.Trim(), Is.EqualTo(NewOrganizationId), "Organization mismatch!");
        });
    }

    [Test, Order(4)]
    public void DeleteEmployee()
    {
        _nav.ToEmployees();

        // Ensure row exists
        var updatedFullName = $"{NewFirstName} {NewMiddleName} {NewLastName}";
        _elementActions.SearchByValue(updatedFullName);
        var updatedEmployeeRow = _elementActions.CheckRowContaining(updatedFullName);

        _nav.ToDelete(updatedEmployeeRow, Uris.EmployeesDelete);

        // Check Delete Details
        var employeeDetailsTable = _driver.FindElement(By.XPath(".//dl"));
        var employeeDetails = employeeDetailsTable.FindElements(By.TagName("dd"));

        var address = $"{NewStreet}, {NewCity}, {NewState} {NewZipCode}, {NewCountry}";

        Assert.Multiple(() =>
        {
            Assert.That(employeeDetails[0].Text.Trim(), Is.EqualTo(updatedFullName), "Name mismatch.");
            Assert.That(employeeDetails[1].Text.Trim(), Is.EqualTo(address), "Address mismatch.");
            Assert.That(employeeDetails[2].Text.Trim(), Is.EqualTo(NewManagerId), "Manager mismatch.");
            Assert.That(employeeDetails[3].Text.Trim(), Is.EqualTo(NewJobTitle), "JobTitle mismatch.");
            Assert.That(employeeDetails[4].Text.Trim(), Is.EqualTo(NewEmploymentType), "EmploymentType mismatch.");
            Assert.That(employeeDetails[5].Text.Trim(), Is.EqualTo(NewPayRate), "PayRate mismatch.");
            Assert.That(employeeDetails[6].Text.Trim(), Is.EqualTo(NewAvailability), "Availability mismatch.");
            Assert.That(employeeDetails[7].Text.Trim(), Is.EqualTo(NewHoursWorked), "HoursWorked mismatch.");
            Assert.That(employeeDetails[8].Text.Trim(), Is.EqualTo(NewCertifications), "Certifications mismatch.");
            Assert.That(employeeDetails[9].Text.Trim(), Is.EqualTo(NewOrganizationId), "Organization mismatch.");
        });

        var submitDelete = _driver.FindElement(By.XPath(XPaths.SubmitBtn));
        new Actions(_driver)
            .MoveToElement(submitDelete)
            .Click()
            .Perform();

        _elementActions.SearchByValue(updatedFullName);
        _elementActions.CheckRowDoesNotExist(updatedFullName);
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}