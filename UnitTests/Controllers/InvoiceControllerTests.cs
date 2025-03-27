using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseProject;
using CourseProject.Areas.Charges.Controllers;
using CourseProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace UnitTests.Controllers
{
    [TestFixture]
    public class InvoicesControllerTests
    {
        private DatabaseContext _context;
        private InvoicesController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "CourseProject_" + Guid.NewGuid().ToString())
                .Options;

            _context = new DatabaseContext(options);
            _controller = new InvoicesController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
            (_controller as IDisposable)?.Dispose();
        }

        #region Index & Details

        [Test]
        public async Task Index_ReturnsViewResult_WithListOfInvoices()
        {
            // Arrange
            var resident = new Resident { ResidentId = 1 , Name = "John"};
            _context.Residents.Add(resident);

            var invoice = new Invoice
            {
                InvoiceID = 1,
                ResidentID = resident.ResidentId,
                Date = new DateTime(2023, 1, 1),
                AmountDue = 100.00m,
                AmountPaid = 50.00m,
                Resident = resident
            };
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as List<Invoice>;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count, Is.EqualTo(1));
            Assert.That(model.First().Resident, Is.Not.Null);
        }

        [Test]
        public async Task Details_ValidId_ReturnsViewResult_WithInvoice()
        {
            // Arrange
            var resident = new Resident { ResidentId = 1 , Name = "John"};
            _context.Residents.Add(resident);

            var invoice = new Invoice
            {
                InvoiceID = 1,
                ResidentID = resident.ResidentId,
                Date = new DateTime(2023, 1, 1),
                AmountDue = 100.00m,
                AmountPaid = 50.00m,
                Resident = resident
            };
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as Invoice;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.InvoiceID, Is.EqualTo(1));
        }

        [Test]
        public async Task Details_NullId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Details(null);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        #endregion

        #region Create Tests

        [Test]
        public void Create_Get_ReturnsViewResult_WithResidentSelectList()
        {
            // Act
            var result = _controller.Create();

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult?.ViewData["ResidentID"], Is.InstanceOf<SelectList>());
        }

        [Test]
        public async Task Create_Post_ValidModel_RedirectsToIndex_AndAddsInvoice()
        {
            // Arrange
            var resident = new Resident { ResidentId = 1, Name = "John" };
            _context.Residents.Add(resident);
            await _context.SaveChangesAsync();

            var invoice = new Invoice
            {
                InvoiceID = 1,
                ResidentID = resident.ResidentId,
                Date = new DateTime(2023, 1, 1),
                AmountDue = 100.00m,
                AmountPaid = 50.00m,
            };

            // Act
            var result = await _controller.Create(invoice);

            // Assert
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult?.ActionName, Is.EqualTo("Index"));

            // Verify that the invoice was added
            var savedInvoice = await _context.Invoices.FindAsync(1);
            Assert.That(savedInvoice, Is.Not.Null);
            Assert.That(savedInvoice.AmountDue, Is.EqualTo(100.00m));
        }

        [Test]
        public async Task Create_Post_InvalidModel_ReturnsView_WithInvoice()
        {
            var invoice = new Invoice
            {
                InvoiceID = 1,
                ResidentID = 99, // invalid resident ID
                Date = new DateTime(2023, 1, 1),
                AmountDue = 100.00m,
                AmountPaid = 50.00m,
            };
            
            var result = await _controller.Create(invoice);
            
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as Invoice;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.InvoiceID, Is.EqualTo(1));
        }

        #endregion

        #region Edit Tests

        [Test]
        public async Task Edit_Get_ValidId_ReturnsViewResult_WithInvoice()
        {
            // Arrange
            var resident = new Resident { ResidentId = 1 , Name = "John"};
            _context.Residents.Add(resident);
            var invoice = new Invoice
            {
                InvoiceID = 1,
                ResidentID = resident.ResidentId,
                Date = new DateTime(2023, 1, 1),
                AmountDue = 100.00m,
                AmountPaid = 50.00m,
            };
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            
            var result = await _controller.Edit(1);
            
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as Invoice;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.InvoiceID, Is.EqualTo(1));
        }

        [Test]
        public async Task Edit_Get_NullId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Edit(null);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Edit_Post_ValidModel_RedirectsToIndex_AndUpdatesInvoice()
        {
            // Arrange
            var resident = new Resident { ResidentId = 1 , Name = "John"};
            _context.Residents.Add(resident);
            var invoice = new Invoice
            {
                InvoiceID = 1,
                ResidentID = resident.ResidentId,
                Date = new DateTime(2023, 1, 1),
                AmountDue = 100.00m,
                AmountPaid = 50.00m,
            };
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // Modify invoice data
            invoice.AmountPaid = 75.00m;

            // Act
            var result = await _controller.Edit(invoice.InvoiceID, invoice);

            // Assert
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult?.ActionName, Is.EqualTo("Index"));

            // Verify update
            var updatedInvoice = await _context.Invoices.FindAsync(1);
            Assert.That(updatedInvoice.AmountPaid, Is.EqualTo(75.00m));
        }

        [Test]
        public async Task Edit_Post_InvalidModel_ReturnsView_WithInvoice()
        {
            // Arrange
            var resident = new Resident { ResidentId = 1, Name = "John" };
            _context.Residents.Add(resident);
            var invoice = new Invoice
            {
                InvoiceID = 1,
                ResidentID = resident.ResidentId,
                Date = new DateTime(2023, 1, 1),
                AmountDue = 100.00m,
                AmountPaid = 50.00m,
            };
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // Force a ModelState error
            _controller.ModelState.AddModelError("AmountDue", "AmountDue error");

            // Act
            var result = await _controller.Edit(invoice.InvoiceID, invoice);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as Invoice;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.InvoiceID, Is.EqualTo(1));

            // Verify invoice not updated
            var unchangedInvoice = await _context.Invoices.FindAsync(1);
            Assert.That(unchangedInvoice.AmountPaid, Is.EqualTo(50.00m));
        }

        #endregion

        #region Delete Tests

        [Test]
        public async Task Delete_Get_ValidId_ReturnsViewResult_WithInvoice()
        {
            // Arrange
            var resident = new Resident { ResidentId = 1 , Name = "John"};
            _context.Residents.Add(resident);
            var invoice = new Invoice
            {
                InvoiceID = 1,
                ResidentID = resident.ResidentId,
                Date = new DateTime(2023, 1, 1),
                AmountDue = 100.00m,
                AmountPaid = 50.00m,
                Resident = resident
            };
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as Invoice;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.InvoiceID, Is.EqualTo(1));
        }

        [Test]
        public async Task Delete_Get_NullId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Delete(null);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Delete_Post_DeletesInvoice_AndRedirects()
        {
            // Arrange
            var resident = new Resident { ResidentId = 1 , Name = "John"};
            _context.Residents.Add(resident);
            var invoice = new Invoice
            {
                InvoiceID = 1,
                ResidentID = resident.ResidentId,
                Date = new DateTime(2023, 1, 1),
                AmountDue = 100.00m,
                AmountPaid = 50.00m,
                Resident = resident
            };
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteConfirmed(invoice.InvoiceID);

            // Assert
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult?.ActionName, Is.EqualTo("Index"));

            // Verify removal
            var deletedInvoice = await _context.Invoices.FindAsync(invoice.InvoiceID);
            Assert.That(deletedInvoice, Is.Null);
        }

        #endregion
    }
}