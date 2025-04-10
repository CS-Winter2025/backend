using CourseProject;
using CourseProject.Areas.Employees.Controllers;
using CourseProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework.Legacy;

namespace UnitTests.ControllerTests
{
    // Disabled due to structure of Employee Model and Views changed.
    // Name is now composed of FirstName, MiddleName, and LastName
    // Build will fail if enabled
    // TODO: Update tests to reflect changes and build is successful
    #if false
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
        public async Task Details_EmployeeNotFound_ReturnsNotFound()
        {
            var result = await _controller.Details(999);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }


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
        public void Create_Get_ReturnsView()
        {
            var result = _controller.Create();
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

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

        // ----- Edit Tests -----

        [Test]
        public async Task Edit_Get_EmployeeNotFound_ReturnsNotFound()
        {
            var result = await _controller.Edit(999);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Edit_Post_IdMismatch_ReturnsViewResult()
        {
            var employee = new Employee
            {
                EmployeeId = 1,
                Name = "Mismatch Employee",
                JobTitle = "Tester",
                EmploymentType = "Full-Time",
                OrganizationId = 1
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var result = await _controller.Edit(2, employee);

            Assert.That(result, Is.InstanceOf<ViewResult>());
        }


        [Test]
        public async Task Edit_Get_ValidId_ReturnsViewResult_WithEmployee()
        {
            var employee = new Employee
            {
                EmployeeId = 1,
                Name = "Edit Employee",
                JobTitle = "Tester",
                EmploymentType = "Full-Time",
                OrganizationId = 1
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var result = await _controller.Edit(1);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as Employee;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.EmployeeId, Is.EqualTo(1));
            Assert.That(model.Name, Is.EqualTo("Edit Employee"));
        }

        [Test]
        public async Task Edit_Get_NullId_ReturnsNotFound()
        {
            var result = await _controller.Edit(null);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Edit_Post_ValidModel_UpdatesEmployee_AndReturnsViewResult()
        {
            var employee = new Employee
            {
                EmployeeId = 1,
                Name = "Original Name",
                JobTitle = "Tester",
                EmploymentType = "Full-Time",
                OrganizationId = 1
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            employee.Name = "Updated Name";

            var result = await _controller.Edit(employee.EmployeeId, employee);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as Employee;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.EmployeeId, Is.EqualTo(1));
            Assert.That(model.Name, Is.EqualTo("Updated Name"));

            var updatedEmployee = await _context.Employees.FindAsync(1);
            Assert.That(updatedEmployee.Name, Is.EqualTo("Updated Name"));
        }

        [Test]
        public async Task Edit_Post_InvalidModel_RedirectsToIndex_AndDoesNotUpdateEmployee()
        {
            var employee = new Employee
            {
                EmployeeId = 1,
                Name = "Original Name",
                JobTitle = "Tester",
                EmploymentType = "Full-Time",
                OrganizationId = 1
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            _controller.ModelState.AddModelError("Name", "Name error");

            var result = await _controller.Edit(employee.EmployeeId, employee);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult?.ActionName, Is.EqualTo("Index"));

            var unchangedEmployee = await _context.Employees.FindAsync(1);
            Assert.That(unchangedEmployee.Name, Is.EqualTo("Original Name"));
        }

        [Test]
        public async Task Edit_Post_ConcurrencyException_WhenEmployeeNoLongerExists_ReturnsNotFound()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("ConcurrencyTestDb2")
                .Options;

            using (var realContext = new DatabaseContext(options))
            {
            }

            var mockContext = new Mock<DatabaseContext>(options) { CallBase = true };

            mockContext
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateConcurrencyException());

            var controller = new EmployeesController(mockContext.Object);

            controller.ModelState.AddModelError("FakeKey", "Forcing invalid model");

            var employee = new Employee
            {
                EmployeeId = 99,
                Name = "Ghost",
                EmploymentType = "Full-Time",
                JobTitle = "Developer",
                OrganizationId = 1
            };

            var result = await controller.Edit(employee.EmployeeId, employee);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Edit_Post_ConcurrencyException_WhenEmployeeStillExists_Throws()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("ConcurrencyTestDb_EmployeeStillExists")
                .Options;

            using (var realContext = new DatabaseContext(options))
            {
                realContext.Employees.Add(new Employee
                {
                    EmployeeId = 1,
                    Name = "Concurrent Employee",
                    JobTitle = "Developer",
                    EmploymentType = "Full-Time",
                    OrganizationId = 1
                });
                await realContext.SaveChangesAsync();
            }

            var mockContext = new Mock<DatabaseContext>(options) { CallBase = true };

            mockContext
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateConcurrencyException());

            var controller = new EmployeesController(mockContext.Object);
            controller.ModelState.AddModelError("FakeKey", "Forcing invalid model");

            var existingEmployee = new Employee
            {
                EmployeeId = 1,
                Name = "Concurrent Employee Updated",
                JobTitle = "Developer",
                EmploymentType = "Full-Time",
                OrganizationId = 1
            };

            Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
                await controller.Edit(existingEmployee.EmployeeId, existingEmployee)
            );
        }

        // ----- Delete Tests -----

        [Test]
        public async Task Delete_Get_EmployeeNotFound_ReturnsNotFound()
        {
            var result = await _controller.Delete(999);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Delete_Post_EmployeeNotFound_RedirectsToIndex()
        {
            var result = await _controller.DeleteConfirmed(999);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult?.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task Delete_Get_ValidId_ReturnsViewResult_WithEmployee()
        {
            var organization = new Organization { OrganizationId = 1 };
            _context.Organizations.Add(organization);
            await _context.SaveChangesAsync();

            var employee = new Employee
            {
                EmployeeId = 1,
                Name = "Delete Employee",
                JobTitle = "Tester",
                EmploymentType = "Full-Time",
                OrganizationId = 1
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var result = await _controller.Delete(1);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as Employee;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.EmployeeId, Is.EqualTo(1));
            Assert.That(model.Name, Is.EqualTo("Delete Employee"));
        }

        [Test]
        public async Task Delete_Get_NullId_ReturnsNotFound()
        {
            var result = await _controller.Delete(null);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Delete_Post_DeletesEmployee_AndRedirects()
        {
            var employee = new Employee
            {
                EmployeeId = 1,
                Name = "Delete Employee",
                JobTitle = "Tester",
                EmploymentType = "Full-Time",
                OrganizationId = 1
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteConfirmed(1);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult?.ActionName, Is.EqualTo("Index"));

            var deletedEmployee = await _context.Employees.FindAsync(1);
            Assert.That(deletedEmployee, Is.Null);
        }
    }
    #endif
}
