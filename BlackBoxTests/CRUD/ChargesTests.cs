using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using BlackBoxTests.Utils;
using BlackBoxTests.Data;
using OpenQA.Selenium.Interactions;

namespace BlackBoxTests.CRUD;

public class ChargesTests
{
    private ChromeDriver _driver;
    private Setup _setup;
    private Auth _auth;
    private Navigation _nav;
    private ElementActions _elementActions;
    
    private const string InvoiceResidentId = "2";
    private const string InvoiceDate = "4012025";
    private const string InvoiceDateTime = "1200AM";
    private const string InvoiceDateList = "4/1/2025 12:00:00 AM";
    private const string InvoiceDateCalender = "2025-04-01T00:00";
    private const string InvoiceAmountDue = "100.00";
    private const string InvoiceAmountPaid = "50.00";

    private const string NewInvoiceResidentId = "1";
    private const string NewInvoiceDate = "12212025";
    private const string NewInvoiceDateTime = "06:30PM";
    private const string NewInvoiceDateList = "12/21/2025 6:30:00 PM";
    private const string NewInvoiceAmountDue = "137.54";
    private const string NewInvoiceAmountPaid = "24.33";

    [OneTimeSetUp]
    public void TestSetup()
    {
        _setup = new Setup();
        _driver = _setup.Driver;
        _auth = new Auth(_driver);
        _nav = new Navigation(_driver);
        _elementActions = new ElementActions(_driver);

        _setup.Initialize();
        _nav.ToLogin();
        _auth.LoginAsUser(Users.AdminUsername, Users.AdminPassword);

        _nav.ToCharges();
    }

    [Test, Order(1)]
    public void CreateInvoice()
    {
        _nav.ToCreateNew(Uris.ChargesCreate);

        _elementActions.SelectDropdownItem("ResidentId", InvoiceResidentId);
        _elementActions.FillDateField("Date", InvoiceDate, InvoiceDateTime);
        _elementActions.FillTextField("AmountDue", InvoiceAmountDue);
        _elementActions.FillTextField("AmountPaid", InvoiceAmountPaid);

        var submitCreate = _driver.FindElement(By.XPath(XPaths.SubmitBtn));
        new Actions(_driver)
            .MoveToElement(submitCreate)
            .Click()
            .Perform();

        // Search for date and check values
        _elementActions.SearchByValue(InvoiceDateList);
        var invoiceRow = _elementActions.CheckRowContaining(InvoiceDateList);
        var invoiceColumns = invoiceRow.FindElements(By.TagName("td"));
        Assert.Multiple(() =>
        {
            Assert.That(invoiceColumns[0].Text.Trim(), Is.EqualTo(InvoiceResidentId), "ResidentId mismatch.");
            Assert.That(invoiceColumns[1].Text.Trim(), Is.EqualTo(InvoiceDateList), "Date mismatch.");
            Assert.That(invoiceColumns[2].Text.Trim(), Is.EqualTo(InvoiceAmountDue), "AmountDue mismatch.");
            Assert.That(invoiceColumns[3].Text.Trim(), Is.EqualTo(InvoiceAmountPaid), "AmountPaid mismatch.");
        });
    }
    
    [Test, Order(2)]
    public void ReadInvoice()
    {
        _nav.ToCharges();

        var invoiceRow = _elementActions.CheckRowContaining(InvoiceDateList);

        _nav.ToDetails(invoiceRow, Uris.ChargesDetails);

        // Confirm detail values
        var invoiceDetailsTable = _driver.FindElement(By.XPath(".//dl"));
        var invoiceDetails = invoiceDetailsTable.FindElements(By.TagName("dd"));
        
        Assert.Multiple(() =>
        {
            Assert.That(invoiceDetails[0].Text.Trim(), Is.EqualTo(InvoiceResidentId), "ResidentId mismatch!");
            Assert.That(invoiceDetails[1].Text.Trim(), Is.EqualTo(InvoiceDateList), "Date mismatch!");
            Assert.That(invoiceDetails[2].Text.Trim(), Is.EqualTo(InvoiceAmountDue), "AmountDue mismatch!");
            Assert.That(invoiceDetails[3].Text.Trim(), Is.EqualTo(InvoiceAmountPaid), "AmountPaid mismatch!");
        });
    }

    [Test, Order(3)]
    public void UpdateInvoice()
    {
        _nav.ToCharges();
        
        // Ensure row exists
        var invoiceRow = _elementActions.CheckRowContaining(InvoiceDateList);

        _nav.ToEdit(invoiceRow, Uris.ChargesEdit);

        // Verify fields
        _elementActions.VerifyDropdownItem("ResidentId", InvoiceResidentId, "ResidentId mismatch!");
        _elementActions.VerifyDateFieldValue("Date", InvoiceDateCalender);
        _elementActions.VerifyTextField("AmountDue", InvoiceAmountDue, "AmountDue mismatch!");
        _elementActions.VerifyTextField("AmountPaid", InvoiceAmountPaid, "AmountPaid mismatch!");

        // Input new values
        _elementActions.SelectDropdownItem("ResidentId", NewInvoiceResidentId);
        _elementActions.FillDateField("Date", NewInvoiceDate, NewInvoiceDateTime);
        _elementActions.FillTextField("AmountDue", NewInvoiceAmountDue);
        _elementActions.FillTextField("AmountPaid", NewInvoiceAmountPaid);

        var submitEdit = _driver.FindElement(By.XPath(XPaths.SubmitBtn));
        new Actions(_driver)
            .MoveToElement(submitEdit)
            .Click()
            .Perform();

        // Search for date and check values
        _elementActions.SearchByValue(NewInvoiceDateList);
        var updatedInvoiceRow = _elementActions.CheckRowContaining(NewInvoiceDateList);

        var invoiceDetails = updatedInvoiceRow.FindElements(By.TagName("td"));
        Assert.Multiple(() =>
        {
            Assert.That(invoiceDetails[0].Text.Trim(), Is.EqualTo(NewInvoiceResidentId), "ResidentId mismatch!");
            Assert.That(invoiceDetails[1].Text.Trim(), Is.EqualTo(NewInvoiceDateList), "Date mismatch!");
            Assert.That(invoiceDetails[2].Text.Trim(), Is.EqualTo(NewInvoiceAmountDue), "AmountDue mismatch!");
            Assert.That(invoiceDetails[3].Text.Trim(), Is.EqualTo(NewInvoiceAmountPaid), "AmountPaid mismatch!");
        });
    }

    [Test, Order(4)]
    public void DeleteInvoice()
    {
        _nav.ToCharges();

        // Ensure row exists
        _elementActions.SearchByValue(NewInvoiceDateList);
        var invoiceRow = _elementActions.CheckRowContaining(NewInvoiceDateList);

        _nav.ToDelete(invoiceRow, Uris.ChargesDelete);

        // Check Delete Details
        var invoiceDetailsTable = _driver.FindElement(By.XPath(".//dl"));
        var invoiceDetails = invoiceDetailsTable.FindElements(By.TagName("dd"));

        Assert.Multiple(() =>
        {
            Assert.That(invoiceDetails[0].Text.Trim(), Is.EqualTo(NewInvoiceResidentId), "ResidentId mismatch!");
            Assert.That(invoiceDetails[1].Text.Trim(), Is.EqualTo(NewInvoiceDateList), "Date mismatch!");
            Assert.That(invoiceDetails[2].Text.Trim(), Is.EqualTo(NewInvoiceAmountDue), "AmountDue mismatch!");
            Assert.That(invoiceDetails[3].Text.Trim(), Is.EqualTo(NewInvoiceAmountPaid), "AmountPaid mismatch!");
        });

        var submitDelete = _driver.FindElement(By.XPath(XPaths.SubmitBtn));
        new Actions(_driver)
            .MoveToElement(submitDelete)
            .Click()
            .Perform();

        _elementActions.SearchByValue(NewInvoiceDateList);
        _elementActions.CheckRowDoesNotExist(NewInvoiceDateList);
    }
    
    [OneTimeTearDown]
    public void TearDown()
    {
        try
        {
            _auth.Logout();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unable to logout: " + ex.Message);
        }
        _setup.CleanupChromeDriver();
    }
}