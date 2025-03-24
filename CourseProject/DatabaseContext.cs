using CourseProject.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseProject
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Resident> Residents { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<EventSchedule> EventSchedules { get; set; }
        public DbSet<Asset> Assets { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee self-referencing relationship
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany(e => e.Subordinates)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Organization-Employee relationship
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Organization)
                .WithMany(o => o.Employees)
                .HasForeignKey(e => e.OrganizationId);

            // Employee-Service many-to-many relationship
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Services)
                .WithMany(s => s.Employees);

            // EventSchedule relationships
            modelBuilder.Entity<EventSchedule>()
                .HasOne(es => es.Employee)
                .WithMany()
                .HasForeignKey(es => es.EmployeeID);

            modelBuilder.Entity<EventSchedule>()
                .HasOne(es => es.Service)
                .WithMany()
                .HasForeignKey(es => es.ServiceID);

            // Resident-Service many-to-many relationship
            modelBuilder.Entity<Resident>()
                .HasMany(r => r.Services)
                .WithMany();

            // Invoice-Resident relationship
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Resident)
                .WithMany()
                .HasForeignKey(i => i.ResidentID);

            //modelBuilder.Entity<Organization>().HasData(
            //    new Organization { OrganizationId = 1 },
            //    new Organization { OrganizationId = 2 }
            //);

            //// Seed Employees
            //modelBuilder.Entity<Employee>().HasData(
            //    new Employee { EmployeeId = 1, Name = "Alice", JobTitle = "Manager", EmploymentType = "Full-Time", PayRate = 60000, OrganizationId = 1 },
            //    new Employee { EmployeeId = 2, Name = "Bob", JobTitle = "Developer", EmploymentType = "Full-Time", PayRate = 50000, OrganizationId = 1, ManagerId = 1 }
            //);

            //// Seed Residents
            //modelBuilder.Entity<Resident>().HasData(
            //    new Resident { ResidentId = 1, Name = "Charlie" },
            //    new Resident { ResidentId = 2, Name = "Diana" }
            //);

            //// Seed Services
            //modelBuilder.Entity<Service>().HasData(
            //    new Service { ServiceID = 1, Type = "Cleaning", Rate = 50 },
            //    new Service { ServiceID = 2, Type = "Security", Rate = 100 }
            //);

            //// Seed Invoices
            //modelBuilder.Entity<Invoice>().HasData(
            //    new Invoice { InvoiceID = 1, ResidentID = 1, Date = DateTime.UtcNow, AmountDue = 200, AmountPaid = 100 }
            //);

            //// Seed EventSchedules
            //modelBuilder.Entity<EventSchedule>().HasData(
            //    new EventSchedule { EventScheduleId = 1, EmployeeID = 1, ServiceID = 1, RangeOfHours = "9AM-5PM" }
            //);
        }
    }
}
