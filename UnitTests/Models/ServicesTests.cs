using CourseProject.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace UnitTests.ModelTests
{
    [TestFixture]
    public class ServicesTests
    {
        [Test]
        public void Service_DefaultConstructor_SetsDefaultValues()
        {
            var service = new Service();

            Assert.That(service.ServiceID, Is.EqualTo(0));
            Assert.That(service.Type, Is.Null);
            Assert.That(service.Rate, Is.EqualTo(0));
            Assert.That(service.Requirements, Is.Not.Null.And.Empty);
            Assert.That(service.Employees, Is.Not.Null.And.Empty);
            Assert.That(service.Residents, Is.Not.Null.And.Empty);
        }

        [Test]
        public void Service_AssignsValuesCorrectly()
        {
            var requirements = new List<string> { "License", "Experience" };

            var service = new Service
            {
                ServiceID = 5,
                Type = "Maintenance",
                Rate = 75.50m,
                Requirements = requirements
            };

            Assert.That(service.ServiceID, Is.EqualTo(5));
            Assert.That(service.Type, Is.EqualTo("Maintenance"));
            Assert.That(service.Rate, Is.EqualTo(75.50m));
            Assert.That(service.Requirements, Is.EqualTo(requirements));
        }

        [Test]
        public void Service_CanAddEmployeesAndResidents()
        {
            var service = new Service();

            service.Employees.Add(new Employee { EmployeeId = 1 });
            service.Residents.Add(new Resident { ResidentId = 1 });

            Assert.That(service.Employees.Count, Is.EqualTo(1));
            Assert.That(service.Residents.Count, Is.EqualTo(1));
        }
    }
}