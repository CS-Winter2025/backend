using CourseProject.Models;
using NUnit.Framework.Legacy;

namespace UnitTests.ModelTests
{
    [TestFixture]
    public class EmployeeModelTests
    {
        [Test]
        public void Employee_InitializesCollectionsProperly()
        {
            var employee = new Employee();

            Assert.That(employee.Availability, Is.Not.Null);
            Assert.That(employee.HoursWorked, Is.Not.Null);
            Assert.That(employee.Certifications, Is.Not.Null);
            Assert.That(employee.Availability, Is.Empty);
            Assert.That(employee.HoursWorked, Is.Empty);
            Assert.That(employee.Certifications, Is.Empty);
        }

        [Test]
        public void Employee_GET_SET()
        {
            var availability = new List<int> { 1, 2 };
            var hoursWorked = new List<int> { 40 };
            var certifications = new List<string> { "Cert1", "Cert2" };
            var employee = new Employee
            {
                EmployeeId = 1,
                Name = "Test Employee",
                JobTitle = "Developer",
                EmploymentType = "Full-Time",
                PayRate = 60.00m,
                Availability = availability,
                HoursWorked = hoursWorked,
                Certifications = certifications,
                ManagerId = 3,
                OrganizationId = 1
            };

            Assert.That(employee.EmployeeId, Is.EqualTo(1));
            Assert.That(employee.Name, Is.EqualTo("Test Employee"));
            Assert.That(employee.JobTitle, Is.EqualTo("Developer"));
            Assert.That(employee.EmploymentType, Is.EqualTo("Full-Time"));
            Assert.That(employee.PayRate, Is.EqualTo(60.00m));
            CollectionAssert.AreEqual(availability, employee.Availability);
            CollectionAssert.AreEqual(hoursWorked, employee.HoursWorked);
            CollectionAssert.AreEqual(certifications, employee.Certifications);
            Assert.That(employee.ManagerId, Is.EqualTo(3));
            Assert.That(employee.OrganizationId, Is.EqualTo(1));
        }
    }

    [TestFixture]
    public class OrganizationModelTests
    {
        [Test]
        public void Organization_InitializesCollectionsProperly()
        {
            var organization = new Organization();

            Assert.That(organization.Employees, Is.Not.Null);
            Assert.That(organization.Services, Is.Not.Null);
            Assert.That(organization.Employees, Is.Empty);
            Assert.That(organization.Services, Is.Empty);
        }

        [Test]
        public void Organization_CanSetOrganizationId()
        {
            var organization = new Organization
            {
                OrganizationId = 1
            };

            Assert.That(organization.OrganizationId, Is.EqualTo(1));
        }
    }
}
