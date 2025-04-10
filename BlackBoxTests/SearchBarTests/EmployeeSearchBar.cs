﻿using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Diagnostics;

namespace BlackBoxTests.SearchBarTests;

public class EmployeeSearchBar
{

    private SearchBarUtils searchUtils = new SearchBarUtils(12, "Employees");

    [SetUp]
    public void Setup()
    {
        searchUtils.login();
    }

    [Test]
    public void SearchByName()
    {
        searchUtils.CheckLetterSearch("Eve", 0);
    }

    [Test]
    public void SearchByManager()
    {
        searchUtils.CheckNumericSearch("1", 1);
    }

    [Test]
    public void SearchByJobTitle()
    {
        searchUtils.CheckLetterSearch("Quality Assurance", 2);
    }

    [Test]
    public void SearchByEmploymentType()
    {
        searchUtils.CheckLetterSearch("Part-Time", 3);
    }

    [Test]
    public void SearchByPayRate()
    {
        searchUtils.CheckNumericSearch("69000.00", 4);
    }

    [Test]
    public void SearchByHoursWorked()
    {
        searchUtils.CheckNumericSearch("222", 6);
    }

    [Test]
    public void SearchByCertifications()
    {
        searchUtils.CheckLetterSearch("First-aid", 7);
    }

    [Test]
    public void SearchByOrganization()
    {
        searchUtils.CheckNumericSearch("1", 8);
    }

    [Test]
    public void SearchByDetails()
    {
        searchUtils.CheckLetterSearch("details", 9);
    }

    [Test]
    public void SearchByPhoto()
    {
        searchUtils.CheckLetterSearch("No photo", 10);
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        searchUtils.logout();
    }
}
