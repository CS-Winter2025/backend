using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace BlackBoxTests.CRUD;

public class EmployeesTests
{
    private ChromeDriver _driver;
    private const string BaseUrl = "http://localhost:5072/"; // Change as needed
    private const string EmployeeUri = "Employees?area=Employees";

    private const string AdminLogin = "admin";
    private const string AdminPassword = "123";

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
        _driver = new ChromeDriver();
        _driver.Manage().Window.Maximize();

        _driver.Navigate().GoToUrl(BaseUrl);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

        _driver.FindElement(By.XPath("//a[text()='Login']")).Click();

        // Login with Admin
        _driver.FindElement(By.Id("Username")).SendKeys(AdminLogin);
        _driver.FindElement(By.Id("Password")).SendKeys(AdminPassword);
        _driver.FindElement(By.XPath("//button[text()='Login']")).Click();

        // Navigate to Services page
        _driver.FindElement(By.XPath("//a[text()='Employees']")).Click();
        Assert.That(_driver.Url, Does.Contain(EmployeeUri), "Did not navigate to the Employees page.");
    }

    [Test, Order(1)]
    public void CreateEmployee()
    {
        // Create New
        _driver.FindElement(By.LinkText("Create New")).Click();
        Assert.That(_driver.Url, Does.Contain("/Employees/Create"), "Did not navigate to the Create page.");

        // Input Values
        _driver.FindElement(By.Id("Name_FirstName")).SendKeys(FirstName);
        _driver.FindElement(By.Id("Name_MiddleName")).SendKeys(MiddleName);
        _driver.FindElement(By.Id("Name_LastName")).SendKeys(LastName);
        _driver.FindElement(By.Id("Address_Street")).SendKeys(Street);
        _driver.FindElement(By.Id("Address_City")).SendKeys(City);
        _driver.FindElement(By.Id("Address_State")).SendKeys(State);
        _driver.FindElement(By.Id("Address_Country")).SendKeys(Country);
        _driver.FindElement(By.Id("Address_ZipCode")).SendKeys(ZipCode);
        var managerIdDropdown = _driver.FindElement(By.Id("ManagerId"));
        var selectManagerId = new SelectElement(managerIdDropdown);
        selectManagerId.SelectByValue(ManagerId);
        _driver.FindElement(By.Id("JobTitle")).SendKeys(JobTitle);
        _driver.FindElement(By.Id("EmploymentType")).SendKeys(EmploymentType);
        var payRateField = _driver.FindElement(By.Id("PayRate"));
        payRateField.Clear();
        payRateField.SendKeys(PayRate);
        var availabilityField = _driver.FindElement(By.Id("Availability"));
        availabilityField.Clear();
        availabilityField.SendKeys(Availability);
        var hoursWorkedField = _driver.FindElement(By.Id("HoursWorked"));
        hoursWorkedField.Clear();
        hoursWorkedField.SendKeys(HoursWorked);
        var certificationsField = _driver.FindElement(By.Id("Certifications"));
        certificationsField.Clear();
        certificationsField.SendKeys(Certifications);
        var organizationIdDropdown = _driver.FindElement(By.Id("OrganizationId"));
        var selectOrganizationId = new SelectElement(organizationIdDropdown);
        selectOrganizationId.SelectByValue(OrganizationId);

        // Submit Create
        var submitCreate = _driver.FindElement(By.XPath("//Input[@type='submit']"));
        new Actions(_driver)
            .MoveToElement(submitCreate)
            .Click()
            .Perform();

        // Wait till back on Employees home page
        _driver.FindElement(By.LinkText("Create New"));

        // Ensure row exists and check values
        var FullName = $"{FirstName} {MiddleName} {LastName}";
        var employeeRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(FullName));
        Assert.That(employeeRow, Is.Not.Null, $"Employee '{FullName}' not found in the list.");

        var employeeDetails = employeeRow.FindElements(By.TagName("td"));
        Assert.Multiple(() =>
        {
            Assert.That(employeeDetails[0].Text.Trim(), Is.EqualTo(FullName), "Name mismatch.");
            Assert.That(employeeDetails[1].Text.Trim(), Is.EqualTo(ManagerId), "Manager mismatch.");
            Assert.That(employeeDetails[2].Text.Trim(), Is.EqualTo(JobTitle), "JobTitle mismatch.");
            Assert.That(employeeDetails[3].Text.Trim(), Is.EqualTo(EmploymentType), "EmploymentType mismatch.");
            Assert.That(employeeDetails[4].Text.Trim(), Is.EqualTo(PayRate), "PayRate mismatch.");
            Assert.That(employeeDetails[5].Text.Trim(), Is.EqualTo(Availability), "Availability mismatch.");
            Assert.That(employeeDetails[6].Text.Trim(), Is.EqualTo(HoursWorked), "HoursWorked mismatch.");
            Assert.That(employeeDetails[7].Text.Trim(), Is.EqualTo(Certifications), "Certifications mismatch.");
            Assert.That(employeeDetails[8].Text.Trim(), Is.EqualTo(OrganizationId), "Organization mismatch.");
        });
    }

    [Test, Order(2)]
    public void ReadEmployee()
    {
        // Navigate to Employees page
        _driver.FindElement(By.XPath("//a[text()='Employees']")).Click();
        Assert.That(_driver.Url, Does.Contain(EmployeeUri), "Did not navigate to the Employees page.");

        // Ensure row exists
        var FullName = $"{FirstName} {MiddleName} {LastName}";
        var employeeRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(FullName));
        Assert.That(employeeRow, Is.Not.Null, $"Employee '{FullName}' not found in the list.");

        // Navigate to Details
        employeeRow.FindElement(By.XPath(".//a[text()='Details']")).Click();
        Assert.That(_driver.Url, Does.Contain("/Employees/Details"), "Did not navigate to the Details page.");

        var employeeDetailsTable = _driver.FindElement(By.XPath(".//dl"));
        var employeeDetails = employeeDetailsTable.FindElements(By.TagName("dd"));

        var address = $"{Street}, {City}, {State} {ZipCode}, {Country}";

        Assert.Multiple(() =>
        {
            Assert.That(employeeDetails[0].Text.Trim(), Is.EqualTo(FullName), "Name mismatch.");
            Assert.That(employeeDetails[1].Text.Trim(), Is.EqualTo(address), "Address mismatch.");
            Assert.That(employeeDetails[2].Text.Trim(), Is.EqualTo(ManagerId), "Manager mismatch.");
            Assert.That(employeeDetails[3].Text.Trim(), Is.EqualTo(JobTitle), "JobTitle mismatch.");
            Assert.That(employeeDetails[4].Text.Trim(), Is.EqualTo(EmploymentType), "EmploymentType mismatch.");
            Assert.That(employeeDetails[5].Text.Trim(), Is.EqualTo(PayRate), "PayRate mismatch.");
            Assert.That(employeeDetails[6].Text.Trim(), Is.EqualTo(Availability), "Availability mismatch.");
            Assert.That(employeeDetails[7].Text.Trim(), Is.EqualTo(HoursWorked), "HoursWorked mismatch.");
            Assert.That(employeeDetails[8].Text.Trim(), Is.EqualTo(Certifications), "Certifications mismatch.");
            Assert.That(employeeDetails[9].Text.Trim(), Is.EqualTo(OrganizationId), "Organization mismatch.");
        });
    }

    [Test, Order(3)]
    public void UpdateEmployee()
    {
        // Navigate to Employees page
        _driver.FindElement(By.XPath("//a[text()='Employees']")).Click();
        Assert.That(_driver.Url, Does.Contain(EmployeeUri), "Did not navigate to the Employees page.");

        // Ensure row exists
        var FullName = $"{FirstName} {MiddleName} {LastName}";
        var employeeRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(FullName));
        Assert.That(employeeRow, Is.Not.Null, $"Employee '{FullName}' not found in the list.");

        // Navigate to Edit
        employeeRow.FindElement(By.XPath(".//a[text()='Edit']")).Click();
        Assert.That(_driver.Url, Does.Contain("/Employees/Edit"), "Did not navigate to the Edit page.");

        // Fields
        var firstNameField = _driver.FindElement(By.Id("Name_FirstName"));
        var middleNameField = _driver.FindElement(By.Id("Name_MiddleName"));
        var lastNameField = _driver.FindElement(By.Id("Name_LastName"));
        var streetField = _driver.FindElement(By.Id("Address_Street"));
        var cityField = _driver.FindElement(By.Id("Address_City"));
        var stateField = _driver.FindElement(By.Id("Address_State"));
        var countryField = _driver.FindElement(By.Id("Address_Country"));
        var zipCodeField = _driver.FindElement(By.Id("Address_ZipCode"));
        var managerIdDropdown = _driver.FindElement(By.Id("ManagerId"));
        var selectedManagerId = new SelectElement(managerIdDropdown);
        var jobTitleField = _driver.FindElement(By.Id("JobTitle"));
        var employmentTypeField = _driver.FindElement(By.Id("EmploymentType"));
        var payRateField = _driver.FindElement(By.Id("PayRate"));
        var availabilityField = _driver.FindElement(By.Id("Availability"));
        var hoursWorkedField = _driver.FindElement(By.Id("HoursWorked"));
        var certificationsField = _driver.FindElement(By.Id("Certifications"));
        var organizationIdDropdown = _driver.FindElement(By.Id("OrganizationId"));
        var selectedOrganizationId = new SelectElement(organizationIdDropdown);

        // Check Edit fields
        var currentFirstName = firstNameField.GetAttribute("value")!.Trim();
        var currentMiddleName = middleNameField.GetAttribute("value")!.Trim();
        var currentLastName = lastNameField.GetAttribute("value")!.Trim();
        var currentStreet = streetField.GetAttribute("value")!.Trim();
        var currentCity = cityField.GetAttribute("value")!.Trim();
        var currentState = stateField.GetAttribute("value")!.Trim();
        var currentCountry = countryField.GetAttribute("value")!.Trim();
        var currentZipCode = zipCodeField.GetAttribute("value")!.Trim();
        var currentManagerId = selectedManagerId.SelectedOption.Text.Trim();
        var currentJobTitle = jobTitleField.GetAttribute("value")!.Trim();
        var currentEmploymentType = employmentTypeField.GetAttribute("value")!.Trim();
        var currentPayRate = payRateField.GetAttribute("value")!.Trim();
        var currentAvailability = availabilityField.GetAttribute("value")!.Trim();
        var currentHoursWorked = hoursWorkedField.GetAttribute("value")!.Trim();
        var currentCertifications = certificationsField.GetAttribute("value")!.Trim();
        var currentOrganizationId = selectedManagerId.SelectedOption.Text.Trim();

        Assert.Multiple(() =>
        {
            Assert.That(currentFirstName, Is.EqualTo(FirstName), "FirstName mismatch.");
            Assert.That(currentMiddleName, Is.EqualTo(MiddleName), "MiddleName mismatch.");
            Assert.That(currentLastName, Is.EqualTo(LastName), "LastName mismatch.");
            Assert.That(currentStreet, Is.EqualTo(Street), "Street mismatch.");
            Assert.That(currentCity, Is.EqualTo(City), "City mismatch.");
            Assert.That(currentState, Is.EqualTo(State), "State mismatch.");
            Assert.That(currentCountry, Is.EqualTo(Country), "Country mismatch.");
            Assert.That(currentZipCode, Is.EqualTo(ZipCode), "ZipCode mismatch.");
            Assert.That(currentManagerId, Is.EqualTo(ManagerId), "ManagerId mismatch.");
            Assert.That(currentJobTitle, Is.EqualTo(JobTitle), "JobTitle mismatch.");
            Assert.That(currentEmploymentType, Is.EqualTo(EmploymentType), "EmploymentType mismatch.");
            Assert.That(currentPayRate, Is.EqualTo(PayRate), "PayRate mismatch.");
            Assert.That(currentAvailability, Is.EqualTo(Availability), "Availability mismatch.");
            Assert.That(currentHoursWorked, Is.EqualTo(HoursWorked), "HoursWorked mismatch.");
            Assert.That(currentCertifications, Is.EqualTo(Certifications), "Certifications mismatch.");
            Assert.That(currentOrganizationId, Is.EqualTo(OrganizationId), "OrganizationId mismatch.");
        });

        // Input New Values
        firstNameField.Clear();
        firstNameField.SendKeys(NewFirstName);
        middleNameField.Clear();
        middleNameField.SendKeys(NewMiddleName);
        lastNameField.Clear();
        lastNameField.SendKeys(NewLastName);
        streetField.Clear();
        streetField.SendKeys(NewStreet);
        cityField.Clear();
        cityField.SendKeys(NewCity);
        stateField.Clear();
        stateField.SendKeys(NewState);
        countryField.Clear();
        countryField.SendKeys(NewCountry);
        zipCodeField.Clear();
        zipCodeField.SendKeys(NewZipCode);
        selectedManagerId.SelectByValue(NewManagerId);
        jobTitleField.Clear();
        jobTitleField.SendKeys(NewJobTitle);
        employmentTypeField.Clear();
        employmentTypeField.SendKeys(NewEmploymentType);
        payRateField.Clear();
        payRateField.SendKeys(NewPayRate);
        availabilityField.Clear();
        availabilityField.SendKeys(NewAvailability);
        hoursWorkedField.Clear();
        hoursWorkedField.SendKeys(NewHoursWorked);
        certificationsField.Clear();
        certificationsField.SendKeys(NewCertifications);
        selectedOrganizationId.SelectByValue(NewOrganizationId);

        // Submit Save
        var submitCreate = _driver.FindElement(By.XPath("//Input[@type='submit']"));
        new Actions(_driver)
            .MoveToElement(submitCreate)
            .Click()
            .Perform();

        _driver.FindElement(By.LinkText("Create New"));

        // Ensure row exists and check values
        var updatedFullName = $"{NewFirstName} {NewMiddleName} {NewLastName}";
        var updatedEmployeeRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(updatedFullName));
        Assert.That(updatedEmployeeRow, Is.Not.Null, $"Employee '{updatedFullName}' not found in the list.");

        var employeeDetails = updatedEmployeeRow.FindElements(By.TagName("td"));
        Assert.Multiple(() =>
        {
            Assert.That(employeeDetails[0].Text.Trim(), Is.EqualTo(updatedFullName), "Name mismatch.");
            Assert.That(employeeDetails[1].Text.Trim(), Is.EqualTo(NewManagerId), "Manager mismatch.");
            Assert.That(employeeDetails[2].Text.Trim(), Is.EqualTo(NewJobTitle), "JobTitle mismatch.");
            Assert.That(employeeDetails[3].Text.Trim(), Is.EqualTo(NewEmploymentType), "EmploymentType mismatch.");
            Assert.That(employeeDetails[4].Text.Trim(), Is.EqualTo(NewPayRate), "PayRate mismatch.");
            Assert.That(employeeDetails[5].Text.Trim(), Is.EqualTo(NewAvailability), "Availability mismatch.");
            Assert.That(employeeDetails[6].Text.Trim(), Is.EqualTo(NewHoursWorked), "HoursWorked mismatch.");
            Assert.That(employeeDetails[7].Text.Trim(), Is.EqualTo(NewCertifications), "Certifications mismatch.");
            Assert.That(employeeDetails[8].Text.Trim(), Is.EqualTo(NewOrganizationId), "Organization mismatch.");
        });
    }

    [Test, Order(4)]
    public void DeleteEmployee()
    {
        // Navigate to Employees page
        _driver.FindElement(By.XPath("//a[text()='Employees']")).Click();
        Assert.That(_driver.Url, Does.Contain(EmployeeUri), "Did not navigate to the Employees page.");

        // Ensure row exists and check values
        var updatedFullName = $"{NewFirstName} {NewMiddleName} {NewLastName}";
        var updatedEmployeeRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(updatedFullName));
        Assert.That(updatedEmployeeRow, Is.Not.Null, $"Employee '{updatedFullName}' not found in the list.");

        // Navigate to Delete
        updatedEmployeeRow.FindElement(By.XPath(".//a[text()='Delete']")).Click();
        Assert.That(_driver.Url, Does.Contain("/Employees/Delete"), "Did not navigate to the Delete page.");

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

        _driver.FindElement(By.XPath("//Input[@type='submit']")).Click();

        _driver.FindElement(By.LinkText("Create New"));

        // Ensure row is gone
        var deletedEmployeeRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(updatedFullName));
        Assert.That(deletedEmployeeRow, Is.Null, $"Employee '{updatedFullName}' not found the list.");
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}