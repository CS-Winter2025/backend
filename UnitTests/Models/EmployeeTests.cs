using CourseProject.Models;
using NUnit.Framework.Legacy;

namespace UnitTests.ModelTests
{
    // [TestFixture]
    // public class EmployeeModelTests
    // {
    //     [Test]
    //     public void Employee_InitializesCollectionsProperly()
    //     {
    //         var employee = new Employee();

    //         Assert.That(employee.Availability, Is.Not.Null);
    //         Assert.That(employee.HoursWorked, Is.Not.Null);
    //         Assert.That(employee.Certifications, Is.Not.Null);
    //         Assert.That(employee.Availability, Is.Empty);
    //         Assert.That(employee.HoursWorked, Is.Empty);
    //         Assert.That(employee.Certifications, Is.Empty);
    //         Assert.That(employee.Subordinates, Is.Not.Null);
    //         Assert.That(employee.Subordinates, Is.Empty);
    //         Assert.That(employee.Services, Is.Not.Null); 
    //         Assert.That(employee.Services, Is.Empty);
    //     }

    //     [Test]
    //     public void Employee_GET_SET()
    //     {
    //         var availability = new List<int> { 1, 2 };
    //         var hoursWorked = new List<int> { 40 };
    //         var certifications = new List<string> { "Cert1", "Cert2" };
    //         var manager = new Employee { EmployeeId = 2, Name = "Manager" };
    //         var organization = new Organization { OrganizationId = 5 };
    //         var subordinates = new List<Employee>
    //         {
    //             new Employee { EmployeeId = 4, Name = "Subordinate1" },
    //             new Employee { EmployeeId = 5, Name = "Subordinate2" }
    //         };
    //         var services = new List<Service>
    //         {
    //             new Service { ServiceID = 1, Type = "Cleaning", Rate = 25.00m },
    //             new Service { ServiceID = 2, Type = "Maintenance", Rate = 30.00m }
    //         };

    //         var employee = new Employee
    //         {
    //             EmployeeId = 1,
    //             Name = "Test Employee",
    //             JobTitle = "Developer",
    //             EmploymentType = "Full-Time",
    //             PayRate = 60.00m,
    //             Availability = availability,
    //             HoursWorked = hoursWorked,
    //             Certifications = certifications,
    //             ManagerId = 3,
    //             OrganizationId = 1,
    //             Manager = manager,
    //             Organization = organization,
    //             Subordinates = subordinates,
    //             Services = services
    //         };

    //         Assert.That(employee.EmployeeId, Is.EqualTo(1));
    //         Assert.That(employee.Name, Is.EqualTo("Test Employee"));
    //         Assert.That(employee.JobTitle, Is.EqualTo("Developer"));
    //         Assert.That(employee.EmploymentType, Is.EqualTo("Full-Time"));
    //         Assert.That(employee.PayRate, Is.EqualTo(60.00m));
    //         CollectionAssert.AreEqual(availability, employee.Availability);
    //         CollectionAssert.AreEqual(hoursWorked, employee.HoursWorked);
    //         CollectionAssert.AreEqual(certifications, employee.Certifications);
    //         Assert.That(employee.ManagerId, Is.EqualTo(3));

    //         Assert.That(employee.OrganizationId, Is.EqualTo(1));
    //         employee.OrganizationId = 10;
    //         Assert.That(employee.OrganizationId, Is.EqualTo(10));

    //         Assert.That(employee.Manager, Is.Not.Null);
    //         Assert.That(employee.Manager.EmployeeId, Is.EqualTo(2));
    //         Assert.That(employee.Manager.Name, Is.EqualTo("Manager"));

    //         Assert.That(employee.Organization, Is.Not.Null);
    //         Assert.That(employee.Organization.OrganizationId, Is.EqualTo(5));
    //         var newOrganization = new Organization { OrganizationId = 15 };
    //         employee.Organization = newOrganization;
    //         Assert.That(employee.Organization.OrganizationId, Is.EqualTo(15));

    //         Assert.That(employee.Subordinates, Is.Not.Null);
    //         Assert.That(employee.Subordinates.Count, Is.EqualTo(2));
    //         Assert.That(employee.Subordinates.First().EmployeeId, Is.EqualTo(4));
    //         Assert.That(employee.Subordinates.First().Name, Is.EqualTo("Subordinate1"));

    //         Assert.That(employee.Services, Is.Not.Null);
    //         Assert.That(employee.Services.Count, Is.EqualTo(2));
    //         Assert.That(employee.Services.First().ServiceID, Is.EqualTo(1));
    //         Assert.That(employee.Services.First().Type, Is.EqualTo("Cleaning"));
    //         Assert.That(employee.Services.ElementAt(1).ServiceID, Is.EqualTo(2));
    //         Assert.That(employee.Services.ElementAt(1).Type, Is.EqualTo("Maintenance"));
    //     }
    // }

    // [TestFixture]
    // public class OrganizationModelTests
    // {
    //     [Test]
    //     public void Organization_InitializesCollectionsProperly()
    //     {
    //         var organization = new Organization();

    //         Assert.That(organization.Employees, Is.Not.Null);
    //         Assert.That(organization.Services, Is.Not.Null);
    //         Assert.That(organization.Employees, Is.Empty);
    //         Assert.That(organization.Services, Is.Empty);
    //     }

    //     [Test]
    //     public void Organization_CanSetOrganizationId()
    //     {
    //         var organization = new Organization
    //         {
    //             OrganizationId = 1
    //         };

    //         Assert.That(organization.OrganizationId, Is.EqualTo(1));
    //     }

    //     [Test]
    //     public void Organization_Services_GET_SET()
    //     {
    //         var services = new List<Service>
    //         {
    //             new Service 
    //             { 
    //                 ServiceID = 1, 
    //                 Type = "Cleaning", 
    //                 Rate = 25.00m, 
    //                 Requirements = new List<string> { "Requirement1", "Requirement2" },
    //                 Employees = new List<Employee> { new Employee { EmployeeId = 1, Name = "Employee1" } },
    //                 Residents = new List<Resident> { new Resident { ResidentId = 1, Name = "Resident1" } }
    //             },
    //             new Service 
    //             { 
    //                 ServiceID = 2, 
    //                 Type = "Maintenance", 
    //                 Rate = 30.00m, 
    //                 Requirements = new List<string> { "Requirement3" },
    //                 Employees = new List<Employee> { new Employee { EmployeeId = 2, Name = "Employee2" } },
    //                 Residents = new List<Resident> { new Resident { ResidentId = 2, Name = "Resident2" } }
    //             }
    //         };

    //         var organization = new Organization
    //         {
    //             Services = services
    //         };

    //         Assert.That(organization.Services, Is.Not.Null);
    //         Assert.That(organization.Services.Count, Is.EqualTo(2));

    //         var firstService = organization.Services.First();
    //         Assert.That(firstService.ServiceID, Is.EqualTo(1));
    //         Assert.That(firstService.Type, Is.EqualTo("Cleaning"));
    //         Assert.That(firstService.Rate, Is.EqualTo(25.00m));
    //         Assert.That(firstService.Requirements, Has.Count.EqualTo(2));
    //         Assert.That(firstService.Requirements[0], Is.EqualTo("Requirement1"));
    //         Assert.That(firstService.Employees, Has.Count.EqualTo(1));
    //         Assert.That(firstService.Employees.First().EmployeeId, Is.EqualTo(1));
    //         Assert.That(firstService.Residents, Has.Count.EqualTo(1));
    //         Assert.That(firstService.Residents.First().ResidentId, Is.EqualTo(1));

    //         var secondService = organization.Services.ElementAt(1);
    //         Assert.That(secondService.ServiceID, Is.EqualTo(2));
    //         Assert.That(secondService.Type, Is.EqualTo("Maintenance"));
    //     }
    // }
}