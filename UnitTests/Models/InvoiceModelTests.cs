using System;
using NUnit.Framework;
using CourseProject.Models;
using CourseProject;

namespace UnitTests.Models
{
    [TestFixture]
    public class InvoiceModelTests
    {
        [Test]
        public void Invoice_InitializesProperly()
        {
            // Arrange & Act
            var invoice = new Invoice();

            // Assert
            Assert.That(invoice, Is.Not.Null);
            // Add more assertions if Invoice initializes any collections or default values.
        }

        [Test]
        public void Invoice_CanSetAndGetProperties()
        {
            // Arrange
            var testDate = new DateTime(2023, 1, 1);
            var invoice = new Invoice
            {
                InvoiceID = 1,
                ResidentID = 5,
                Date = testDate,
                AmountDue = 100.00m,
                AmountPaid = 50.00m
            };

            // Assert
            Assert.That(invoice.InvoiceID, Is.EqualTo(1));
            Assert.That(invoice.ResidentID, Is.EqualTo(5));
            Assert.That(invoice.Date, Is.EqualTo(testDate));
            Assert.That(invoice.AmountDue, Is.EqualTo(100.00m));
            Assert.That(invoice.AmountPaid, Is.EqualTo(50.00m));
        }
    }

    [TestFixture]
    public class ResidentModelTests
    {
        [Test]
        public void Resident_CanSetAndGetProperties()
        {
            // Arrange
            var resident = new Resident
            {
                ResidentId = 1,
                // Set additional properties if defined.
            };

            // Assert
            Assert.That(resident.ResidentId, Is.EqualTo(1));
        }
    }
}