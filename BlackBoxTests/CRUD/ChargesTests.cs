using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace BlackBoxTests.CRUD;

public class ChargesTests
{
    private ChromeDriver _driver;
    private const string BaseUrl = "http://localhost:5072/"; // Change as needed
    private const string LoginUri = "Users/Login";
    private const string ChargesUri = "Invoices?area=Charges";
    
    private const string AdminLogin = "admin";
    private const string AdminPassword = "123";
    
    private const string InvoiceResidentId = "2";
    private const string InvoiceDateYear = "2025";
    private const string InvoiceDateOther = "04010000AM";
    private const string InvoiceDateList = "2025-04-01 12:00:00 AM";
    private const string InvoiceDateCalender = "2025-04-01T00:00";
    private const string InvoiceAmountDue = "100.00";
    private const string InvoiceAmountPaid = "50.00";

    private const string NewInvoiceResidentId = "1";
    private const string NewInvoiceDateYear = "2025";
    private const string NewInvoiceDateOther = "12210630PM";
    private const string NewInvoiceDateList = "2025-12-21 06:30:00 PM";
    private const string NewInvoiceAmountDue = "137.54";
    private const string NewInvoiceAmountPaid = "24.33";

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
        
        // Navigate to Charges page
        _driver.FindElement(By.XPath("//a[text()='Charges']")).Click();
        Assert.That(_driver.Url, Does.Contain(ChargesUri), "Did not navigate to the Invoices page.");
    }

    [Test, Order(1)]
    public void CreateInvoice()
    {
        // Create New
        _driver.FindElement(By.LinkText("Create New")).Click();
        Assert.That(_driver.Url, Does.Contain("/Invoices/Create"), "Did not navigate to the Create page.");
        
        // Input Values
        var residentIdDropdown = _driver.FindElement(By.Id("ResidentID"));
        var selectResidentId = new SelectElement(residentIdDropdown);
        selectResidentId.SelectByValue(InvoiceResidentId);
        var dateTimeField = _driver.FindElement(By.Id("Date"));
        dateTimeField.Clear();
        dateTimeField.SendKeys(InvoiceDateYear);
        dateTimeField.SendKeys(Keys.Tab);
        dateTimeField.SendKeys(InvoiceDateOther);
        _driver.FindElement(By.Id("AmountDue")).SendKeys(InvoiceAmountDue);
        _driver.FindElement(By.Id("AmountPaid")).SendKeys(InvoiceAmountPaid);
        
        // Submit Create
        _driver.FindElement(By.XPath("//Input[@type='submit']")).Click();
        
        // Wait till back on Invoice home
        _driver.FindElement(By.LinkText("Create New"));
        Assert.That(_driver.Url, Does.Contain("Invoices"), "Did not navigate to the Invoices page.");
        
        // Ensure row exists and check values
        var invoiceRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(InvoiceDateList));
        Assert.That(invoiceRow, Is.Not.Null, $"Invoice with date: '{InvoiceDateList}' not found in the list.");

        var invoiceColumns = invoiceRow.FindElements(By.TagName("td"));
        Assert.Multiple(() =>
        {
            Assert.That(invoiceColumns[0].Text.Trim(), Is.EqualTo(InvoiceResidentId), "ResidentID mismatch.");
            Assert.That(invoiceColumns[1].Text.Trim(), Is.EqualTo(InvoiceDateList), "Date mismatch.");
            Assert.That(invoiceColumns[2].Text.Trim(), Is.EqualTo(InvoiceAmountDue), "AmountDue mismatch.");
            Assert.That(invoiceColumns[3].Text.Trim(), Is.EqualTo(InvoiceAmountPaid), "AmountPaid mismatch.");
        });
    }
    
    [Test, Order(2)]
    public void ReadInvoice()
    {
        Assert.That(_driver.Url, Does.Contain("Invoices"), "Did not navigate to the Invoices page.");
        
        // Ensure row exists
        var invoiceRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(InvoiceDateList));
        Assert.That(invoiceRow, Is.Not.Null, $"Invoice with date: '{InvoiceDateList}' not found in the list.");
        
        invoiceRow.FindElement(By.XPath(".//a[text()='Details']")).Click();
        
        Assert.That(_driver.Url, Does.Contain("/Invoices/Details"), "Did not navigate to the Details page.");

        var invoiceDetailsTable = _driver.FindElement(By.XPath(".//dl"));
        var invoiceDetails = invoiceDetailsTable.FindElements(By.TagName("dd"));
        
        Assert.Multiple(() =>
        {
            Assert.That(invoiceDetails[0].Text.Trim(), Is.EqualTo(InvoiceResidentId), "ResidentID mismatch.");
            Assert.That(invoiceDetails[1].Text.Trim(), Is.EqualTo(InvoiceDateList), "Date mismatch.");
            Assert.That(invoiceDetails[2].Text.Trim(), Is.EqualTo(InvoiceAmountDue), "AmountDue mismatch.");
            Assert.That(invoiceDetails[3].Text.Trim(), Is.EqualTo(InvoiceAmountPaid), "AmountPaid mismatch.");
        });
    }

    [Test, Order(3)]
    public void UpdateInvoice()
    {
        // Navigate to Charges page
        _driver.FindElement(By.XPath("//a[text()='Charges']")).Click();
        Assert.That(_driver.Url, Does.Contain(ChargesUri), "Did not navigate to the Invoices page.");
        
        // Ensure row exists
        var invoiceRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(InvoiceDateList));
        Assert.That(invoiceRow, Is.Not.Null, $"Invoice with date: '{InvoiceDateList}' not found in the list.");
        
        invoiceRow.FindElement(By.XPath(".//a[text()='Edit']")).Click();
        
        Assert.That(_driver.Url, Does.Contain("/Invoices/Edit"), "Did not navigate to the Edit page.");
        
        // Check current fields
        var dropdown = _driver.FindElement(By.Id("ResidentID"));
        var selectedElement = new SelectElement(dropdown);
        var currentInvoiceResidentId = selectedElement.SelectedOption.Text.Trim();
        var currentInvoiceDate = _driver.FindElement(By.Id("Date")).GetAttribute("value")!.Trim();
        var currentInvoiceAmountDue = _driver.FindElement(By.Id("AmountDue")).GetAttribute("value")!.Trim();
        var currentInvoiceAmountPaid = _driver.FindElement(By.Id("AmountPaid")).GetAttribute("value")!.Trim();
        Assert.Multiple(() =>
        {
            Assert.That(currentInvoiceResidentId, Is.EqualTo(InvoiceResidentId), "ResidentID mismatch.");
            Assert.That(currentInvoiceDate, Is.EqualTo(InvoiceDateCalender), "Date mismatch.");
            Assert.That(currentInvoiceAmountDue, Is.EqualTo(InvoiceAmountDue), "AmountDue mismatch.");
            Assert.That(currentInvoiceAmountPaid, Is.EqualTo(InvoiceAmountPaid), "AmountPaid mismatch.");
        });
        
        // Input new values
        var residentIdDropdown = _driver.FindElement(By.Id("ResidentID"));
        var selectResidentId = new SelectElement(residentIdDropdown);
        selectResidentId.SelectByValue(NewInvoiceResidentId);
        var dateTimeField = _driver.FindElement(By.Id("Date"));
        dateTimeField.Clear();
        dateTimeField.SendKeys(NewInvoiceDateYear);
        dateTimeField.SendKeys(Keys.Tab);
        dateTimeField.SendKeys(NewInvoiceDateOther);
        var amountDue = _driver.FindElement(By.Id("AmountDue"));
        amountDue.Clear();
        amountDue.SendKeys( NewInvoiceAmountDue);
        var amountPaid = _driver.FindElement(By.Id("AmountPaid"));
        amountPaid.Clear();
        amountPaid.SendKeys(NewInvoiceAmountPaid);
        
        // Submit Save
        _driver.FindElement(By.XPath("//Input[@type='submit']")).Click();
        Assert.That(_driver.Url, Does.Contain("Invoices"), "Did not navigate to the Invoices page.");
        
        // Ensure row exists and check values
        var updatedInvoiceRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(NewInvoiceDateList));
        Assert.That(updatedInvoiceRow, Is.Not.Null, $"Invoice with date: '{NewInvoiceDateList}' not found in the list.");

        var invoiceDetails = updatedInvoiceRow.FindElements(By.TagName("td"));
        Assert.Multiple(() =>
        {
            Assert.That(invoiceDetails[0].Text.Trim(), Is.EqualTo(NewInvoiceResidentId), "ResidentID mismatch.");
            Assert.That(invoiceDetails[1].Text.Trim(), Is.EqualTo(NewInvoiceDateList), "Date mismatch.");
            Assert.That(invoiceDetails[2].Text.Trim(), Is.EqualTo(NewInvoiceAmountDue), "AmountDue mismatch.");
            Assert.That(invoiceDetails[3].Text.Trim(), Is.EqualTo(NewInvoiceAmountPaid), "AmountPaid mismatch.");
        });
    }

    [Test, Order(4)]
    public void DeleteInvoice()
    {
        Assert.That(_driver.Url, Does.Contain("Invoices"), "Did not navigate to the Invoices page.");
        
        // Ensure row exists
        var invoiceRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(NewInvoiceDateList));
        Assert.That(invoiceRow, Is.Not.Null, $"Invoice with date: '{NewInvoiceDateList}' not found in the list.");
        
        invoiceRow.FindElement(By.XPath(".//a[text()='Delete']")).Click();
        
        Assert.That(_driver.Url, Does.Contain("/Invoices/Delete"), "Did not navigate to the Delete page.");
        
        _driver.FindElement(By.XPath("//Input[@type='submit']")).Click();
        Assert.That(_driver.Url, Does.Contain("Invoices"), "Did not navigate to the Invoices page.");
        
        // Ensure row is gone
        var deletedInvoiceRow = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(NewInvoiceDateList));
        Assert.That(deletedInvoiceRow, Is.Null, $"Invoice with date: '{NewInvoiceDateList}' found in the list.");
    }
    
    [OneTimeTearDown]
    public void TearDown()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}