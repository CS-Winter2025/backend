using CourseProject;
using CourseProject.Areas.Employees.Controllers;
using CourseProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Legacy;

namespace UnitTests.ControllerTests
{
    [TestFixture]
    public class EmployeesControllerTests
    {
        private DatabaseContext _context;
        private EmployeesController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "CourseProject_" + Guid.NewGuid().ToString())
                .Options;

            _context = new DatabaseContext(options);
            _controller = new EmployeesController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
            (_controller as IDisposable)?.Dispose();
        }

        // ----- Index and Details Tests -----

        [Test]
        public async Task Index_ReturnsViewResult_WithListOfEmployees()
        {
            var manager = new Employee
            {
                EmployeeId = 3,
                Name = "Manager Doe",
                JobTitle = "Senior Manager",
                EmploymentType = "Full-Time"
            };
            var organization = new Organization { OrganizationId = 1 };

            _context.Employees.Add(manager);
            _context.Organizations.Add(organization);
            await _context.SaveChangesAsync();

            _context.Employees.AddRange(new List<Employee>
            {
                new Employee
                {
                    EmployeeId = 1,
                    Name = "John Doe",
                    JobTitle = "Developer",
                    EmploymentType = "Full-Time",
                    ManagerId = 3,
                    OrganizationId = 1
                },
                new Employee
                {
                    EmployeeId = 2,
                    Name = "Jane Doe",
                    JobTitle = "Manager",
                    EmploymentType = "Full-Time",
                    ManagerId = 3,
                    OrganizationId = 1
                }
            });
            await _context.SaveChangesAsync();

            var result = await _controller.Index();

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as List<Employee>;
            Assert.That(model?.Count, Is.EqualTo(2));
            var names = model.Select(e => e.Name).ToList();
            CollectionAssert.AreEquivalent(new List<string> { "John Doe", "Jane Doe" }, names);
        }

        [Test]
        public async Task Details_ValidId_ReturnsViewResult_WithEmployee()
        {
            var manager = new Employee
            {
                EmployeeId = 2,
                Name = "Manager Doe",
                JobTitle = "Senior Manager",
                EmploymentType = "Full-Time"
            };
            var organization = new Organization { OrganizationId = 1 };

            _context.Employees.Add(manager);
            _context.Organizations.Add(organization);
            await _context.SaveChangesAsync();

            var employee = new Employee
            {
                EmployeeId = 1,
                Name = "John Doe",
                JobTitle = "Developer",
                EmploymentType = "Full-Time",
                ManagerId = 2,
                OrganizationId = 1
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var result = await _controller.Details(1);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as Employee;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.EmployeeId, Is.EqualTo(1));
            Assert.That(model.Name, Is.EqualTo("John Doe"));
        }

        [Test]
        public async Task Details_NullId_ReturnsNotFound()
        {
            var result = await _controller.Details(null);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        // ----- Create Tests -----

        [Test]
        public async Task Create_ValidModel_ReturnsViewResult()
        {
            var employee = new Employee
            {
                EmployeeId = 1,
                Name = "New Employee",
                JobTitle = "Developer", 
                EmploymentType = "Full-Time", 
                PayRate = 50.00m,
                Availability = new List<int> { 1, 2 },
                HoursWorked = new List<int> { 40 },
                Certifications = new List<string>(), 
                OrganizationId = 1
            };
            string certificationsInput = "Cert1, Cert2";

            var result = await _controller.Create(employee, certificationsInput);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as Employee;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Name, Is.EqualTo("New Employee"));
            Assert.That(model.Certifications.Count, Is.EqualTo(0));

            var savedEmployee = await _context.Employees.FindAsync(1);
            Assert.That(savedEmployee, Is.Null);
        }

        [Test]
        public async Task Create_InvalidModel_RedirectsToIndex()
        {
            var employee = new Employee
            {
                EmployeeId = 1,
                Name = "New Employee",
                JobTitle = "Developer",
                EmploymentType = "Full-Time",
                PayRate = 50.00m,
                Availability = new List<int> { 1, 2 },
                HoursWorked = new List<int> { 40 },
                Certifications = new List<string>(),
                OrganizationId = 1
            };
            _controller.ModelState.AddModelError("JobTitle", "The JobTitle field is required.");
            string certificationsInput = "Cert1, Cert2";

            var result = await _controller.Create(employee, certificationsInput);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult?.ActionName, Is.EqualTo("Index"));

            var savedEmployee = await _context.Employees.FindAsync(1);
            Assert.That(savedEmployee, Is.Not.Null);
            Assert.That(savedEmployee.Name, Is.EqualTo("New Employee"));
            Assert.That(savedEmployee.Certifications.Count, Is.EqualTo(2));
            Assert.That(savedEmployee.Certifications, Contains.Item("Cert1"));
            Assert.That(savedEmployee.Certifications, Contains.Item("Cert2"));
        }

        
    }
}
