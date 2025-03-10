using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using CourseProject.Models;

namespace CourseProject
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        public ApplicationContext() { }

      
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<Resident> Residents { get; set; }
    }
}
