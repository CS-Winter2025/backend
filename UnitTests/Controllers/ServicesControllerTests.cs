using CourseProject.Areas.Services.Controllers;
using CourseProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseProject;

namespace UnitTests.ControllerTests
{
    [TestFixture]
    public class ServicesControllerTests
    {
        private ServicesController _controller;
        private DatabaseContext _context;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            _context = new DatabaseContext(options);
            _context.Services.AddRange(
                new Service { ServiceID = 1, Type = "Cleaning", Rate = 50 },
                new Service { ServiceID = 2, Type = "Security", Rate = 100 }
            );
            _context.SaveChanges();

            _controller = new ServicesController(_context);
        }

        [Test]
        public async Task Index_ReturnsViewResult_WithListOfServices()
        {
            var result = await _controller.Index();

            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult!.Model, Is.TypeOf<List<Service>>());
            Assert.That(((List<Service>)viewResult.Model!).Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Details_NullId_ReturnsNotFound()
        {
            var result = await _controller.Details(null);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Details_ValidId_ReturnsService()
        {
            var result = await _controller.Details(1);

            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = result as ViewResult;
            var model = viewResult!.Model as Service;

            Assert.That(model, Is.Not.Null);
            Assert.That(model!.ServiceID, Is.EqualTo(1));
            Assert.That(model.Type, Is.EqualTo("Cleaning"));
        }

        [Test]
        public void Create_Get_ReturnsViewResult()
        {
            var result = _controller.Create();
            Assert.That(result, Is.TypeOf<ViewResult>());
        }

        [Test]
        public async Task Create_Post_ValidService_RedirectsToIndex()
        {
            var service = new Service { Type = "Gardening", Rate = 75 };
            var result = await _controller.Create(service);

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());
            Assert.That(((RedirectToActionResult)result).ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task Edit_Get_ValidId_ReturnsServiceView()
        {
            var result = await _controller.Edit(1);

            Assert.That(result, Is.TypeOf<ViewResult>());
            var model = ((ViewResult)result).Model as Service;
            Assert.That(model, Is.Not.Null);
            Assert.That(model!.ServiceID, Is.EqualTo(1));
        }

        [Test]
        public async Task Edit_Post_IdMismatch_ReturnsNotFound()
        {
            var service = new Service { ServiceID = 2, Type = "Mismatch", Rate = 20 };
            var result = await _controller.Edit(1, service);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Delete_Get_ValidId_ReturnsService()
        {
            var result = await _controller.Delete(1);

            Assert.That(result, Is.TypeOf<ViewResult>());
            var model = ((ViewResult)result).Model as Service;
            Assert.That(model, Is.Not.Null);
            Assert.That(model!.ServiceID, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteConfirmed_ValidId_RemovesServiceAndRedirects()
        {
            var result = await _controller.DeleteConfirmed(1);

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());
            Assert.That(((RedirectToActionResult)result).ActionName, Is.EqualTo("Index"));

            var deleted = await _context.Services.FindAsync(1);
            Assert.That(deleted, Is.Null);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _controller.Dispose();
        }
    }
}
