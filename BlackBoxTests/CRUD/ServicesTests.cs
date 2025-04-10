using BlackBoxTests.Data;
using BlackBoxTests.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace BlackBoxTests.CRUD;

public class ServicesTests
{
    private ChromeDriver _driver;
    private Setup _setup;
    private Auth _auth;
    private Navigation _nav;
    private ElementActions _elementActions;

    private const string ServiceType = "Coaching";
    private const string ServiceRate = "95.00";
    private const string ServiceRequirements = "None";

    private const string NewServiceType = "Therapy";
    private const string NewServiceRate = "50.00";
    private const string NewServiceRequirements = "Licence";
    
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

        _nav.ToServices();
    }

    [Test, Order(1)]
    public void CreateService()
    {
        _nav.ToCreateNew(Uris.ServicesCreate);

        // Input Values
        _elementActions.FillTextField("Type", ServiceType);
        _elementActions.FillTextField("Rate", ServiceRate);
        _elementActions.FillTextField("Requirements", ServiceRequirements);

        var submitCreate = _driver.FindElement(By.XPath(XPaths.SubmitBtn));
        new Actions(_driver)
            .MoveToElement(submitCreate)
            .Click()
            .Perform();

        // Ensure row exists and check values
        _elementActions.SearchByValue(ServiceType);
        var serviceRow = _elementActions.CheckRowContaining(ServiceType);

        var serviceColumns = serviceRow.FindElements(By.TagName("td"));
        Assert.Multiple(() =>
        {
            Assert.That(serviceColumns[0].Text.Trim(), Is.EqualTo(ServiceType), "Service type mismatch!");
            Assert.That(serviceColumns[1].Text.Trim(), Is.EqualTo(ServiceRate), "Service rate mismatch!");
            Assert.That(serviceColumns[2].Text.Trim(), Is.EqualTo(ServiceRequirements), "Service requirements mismatch!");
        });
    }
    
    [Test, Order(2)]
    public void ReadService()
    {
        _nav.ToServices();

        // Ensure row exists
        _elementActions.SearchByValue(ServiceType);
        var serviceRow = _elementActions.CheckRowContaining(ServiceType);
        _nav.ToDetails(serviceRow, Uris.ServicesDetails);

        var serviceDetailsTable = _driver.FindElement(By.XPath(".//dl"));
        var serviceDetails = serviceDetailsTable.FindElements(By.TagName("dd"));
        
        Assert.Multiple(() =>
        {
            Assert.That(serviceDetails[0].Text.Trim(), Is.EqualTo(ServiceType), "Service type mismatch!");
            Assert.That(serviceDetails[1].Text.Trim(), Is.EqualTo(ServiceRate), "Service rate mismatch!");
            Assert.That(serviceDetails[2].Text.Trim(), Is.EqualTo(ServiceRequirements), "Service requirements mismatch!");
        });
    }

    [Test, Order(3)]
    public void UpdateService()
    {
        _nav.ToServices();

        // Ensure row exists
        _elementActions.SearchByValue(ServiceType);
        var serviceRow = _elementActions.CheckRowContaining(ServiceType);

        _nav.ToEdit(serviceRow, Uris.ServicesEdit);

        _elementActions.VerifyTextField("Type", ServiceType, "Sevice type mismatch!");
        _elementActions.VerifyTextField("Rate", ServiceRate, "Sevice rate mismatch!");
        // NOTE currently not working as Requirements field returns: System.Collections.Generic.List`1[System.String]
        //_elementActions.VerifyTextField("Requirements", ServiceRequirements, "Sevice requirements mismatch!");

        // Input new values
        _elementActions.FillTextField("Type", NewServiceType);
        _elementActions.FillTextField("Rate", NewServiceRate);
        _elementActions.FillTextField("Requirements", NewServiceRequirements);

        var submitEdit = _driver.FindElement(By.XPath(XPaths.SubmitBtn));
        new Actions(_driver)
            .MoveToElement(submitEdit)
            .Click()
            .Perform();

        // Ensure row exists and check values
        _elementActions.SearchByValue(NewServiceType);
        var updatedServiceRow = _elementActions.CheckRowContaining(NewServiceType);

        var serviceColumns = updatedServiceRow.FindElements(By.TagName("td"));
        Assert.Multiple(() =>
        {
            Assert.That(serviceColumns[0].Text.Trim(), Is.EqualTo(NewServiceType), "Service type mismatch!");
            Assert.That(serviceColumns[1].Text.Trim(), Is.EqualTo(NewServiceRate), "Service rate mismatch!");
            Assert.That(serviceColumns[2].Text.Trim(), Is.EqualTo(NewServiceRequirements), "Service requirements mismatch!");
        });
    }

    [Test, Order(4)]
    public void DeleteService()
    {
        _nav.ToServices();

        // Ensure row exists
        _elementActions.SearchByValue(NewServiceType);
        var serviceRow = _elementActions.CheckRowContaining(NewServiceType);

        _nav.ToDelete(serviceRow, Uris.ServicesDelete);

        var serviceDetailsTable = _driver.FindElement(By.XPath(".//dl"));
        var serviceDetails = serviceDetailsTable.FindElements(By.TagName("dd"));

        Assert.Multiple(() =>
        {
            Assert.That(serviceDetails[0].Text.Trim(), Is.EqualTo(NewServiceType), "Service type mismatch!");
            Assert.That(serviceDetails[1].Text.Trim(), Is.EqualTo(NewServiceRate), "Service rate mismatch!");
            Assert.That(serviceDetails[2].Text.Trim(), Is.EqualTo(NewServiceRequirements), "Service requirements mismatch!");
        });

        var submitDelete = _driver.FindElement(By.XPath(XPaths.SubmitBtn));
        new Actions(_driver)
            .MoveToElement(submitDelete)
            .Click()
            .Perform();

        _elementActions.SearchByValue(NewServiceType);
        _elementActions.CheckRowDoesNotExist(NewServiceType);
    }
    
    [OneTimeTearDown]
    public void TearDown()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}