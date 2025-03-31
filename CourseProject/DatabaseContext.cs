using CourseProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNet.Identity;

namespace CourseProject
{

    public class DatabaseContext : IdentityDbContext<IdentityUser>

    {
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Resident> Residents { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<EventSchedule> EventSchedules { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<ResidentAsset> ResidentAssets { get; set; }
        public DbSet<ResidentAssetRequest> ResidentAssetRequests { get; set; }


        public DbSet<User> Users { get; set; }


        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>().OwnsOne(e => e.Name);
            modelBuilder.Entity<Resident>().OwnsOne(r => r.Name);

            modelBuilder.Entity<Employee>().OwnsOne(e => e.Address);
            modelBuilder.Entity<Resident>().OwnsOne(r => r.Address);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany(e => e.Subordinates)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Organization)
                .WithMany(o => o.Employees)
                .HasForeignKey(e => e.OrganizationId);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Services)
                .WithMany(s => s.Employees);

            modelBuilder.Entity<EventSchedule>()
                .HasMany(es => es.Employees)
                .WithMany(e => e.EventSchedules)
                .UsingEntity<Dictionary<string, object>>(
                    "EventScheduleEmployee", // Junction table name
                    r => r.HasOne<Employee>().WithMany().HasForeignKey("EmployeeId"), // Foreign key for Employee
                    r => r.HasOne<EventSchedule>().WithMany().HasForeignKey("EventScheduleId"), // Foreign key for EventSchedule
                    r =>
                    {
                        // Seed data for the junction table
                        r.HasData(
                            new { EventScheduleId = 1, EmployeeId = 1 },
                            new { EventScheduleId = 1, EmployeeId = 2 }
                        );
                    }
                );

            modelBuilder.Entity<EventSchedule>()
                .HasOne(es => es.Service)
                .WithMany()
                .HasForeignKey(es => es.ServiceID);

            modelBuilder.Entity<EventSchedule>()
                .HasOne(es => es.Resident)
                .WithMany(r => r.EventSchedules)
                .HasForeignKey(es => es.ResidentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ResidentAsset>()
                .HasOne(ra => ra.Resident)
                .WithMany()
                .HasForeignKey(ra => ra.ResidentId);

            modelBuilder.Entity<ResidentAsset>()
                .HasOne(ra => ra.Asset)
                .WithMany()
                .HasForeignKey(ra => ra.AssetID);

            modelBuilder.Entity<ResidentAssetRequest>()
                .HasOne(r => r.Resident)
                .WithMany()
                .HasForeignKey(r => r.ResidentId);

            modelBuilder.Entity<ResidentAssetRequest>()
                .HasOne(r => r.Asset)
                .WithMany()
                .HasForeignKey(r => r.AssetID);

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion(
                    r => r.ToString(),
                    r => Enum.Parse<UserRole>(r)
                );

            // Resident-Service many-to-many relationship
            modelBuilder.Entity<Resident>().ToTable("Resident")
                .HasMany(r => r.Services)
                .WithMany(s => s.Residents) 
                .UsingEntity(j => j.HasData(
                    new { ResidentsResidentId = 1, ServicesServiceID = 1 },
                    new { ResidentsResidentId = 2, ServicesServiceID = 2 }
                ));

            modelBuilder.Entity<Asset>().ToTable("Asset");


            // Invoice-Resident relationship
            modelBuilder.Entity<Invoice>().ToTable("Invoice")
                .HasOne(i => i.Resident)
                .WithMany()
                .HasForeignKey(i => i.ResidentID)
                .IsRequired();

            modelBuilder.Entity<Organization>().ToTable("Organization").HasData(
                new Organization { OrganizationId = 1 },
                new Organization { OrganizationId = 2 }
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    EmployeeId = 1,
                    Name = new FullName { FirstName = "Alice", LastName = "Smith" },
                    JobTitle = "Manager",
                    EmploymentType = "Full-Time",
                    PayRate = 60000,
                    OrganizationId = 1,
                    Address = new FullAddress { Street = "123 Main St", City = "New York", State = "NY", ZipCode = "10001" },
                    ProfilePicture = null
                },
                new Employee
                {
                    EmployeeId = 2,
                    Name = new FullName { FirstName = "Bob", LastName = "Johnson" },
                    JobTitle = "Developer",
                    EmploymentType = "Full-Time",
                    PayRate = 50000,
                    OrganizationId = 1,
                    ManagerId = 1,
                    Address = new FullAddress { Street = "456 Park Ave", City = "New York", State = "NY", ZipCode = "10002" },
                    ProfilePicture = null
                }
            );

            modelBuilder.Entity<Resident>().HasData(
                new Resident
                {
                    ResidentId = 1,
                    Name = new FullName { FirstName = "Charlie", LastName = "Brown" },
                    Address = new FullAddress { Street = "789 Oak St", City = "Los Angeles", State = "CA", ZipCode = "90001" }
                },
                new Resident
                {
                    ResidentId = 2,
                    Name = new FullName { FirstName = "Diana", LastName = "Prince" },
                    Address = new FullAddress { Street = "101 Pine St", City = "San Francisco", State = "CA", ZipCode = "94101" }
                }
            );
            modelBuilder.Entity<Service>().HasData(
                new Service { ServiceID = 1, Type = "Cleaning", Rate = 50 },
                new Service { ServiceID = 2, Type = "Security", Rate = 100 }
            );

            modelBuilder.Entity<Invoice>().HasData(
                new Invoice { InvoiceID = 1, ResidentID = 1, Date = DateTime.UtcNow, AmountDue = 200, AmountPaid = 100 }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1, Username = "admin", Role = UserRole.ADMIN,
                    Password = new PasswordHasher().HashPassword("123")
                },
                new User
                {
                    Id = 2, Username = "resident", Role = UserRole.RESIDENT,
                    Password = new PasswordHasher().HashPassword("123")
                },
                new User
                {
                    Id = 3, Username = "housing", Role = UserRole.HOUSING_MANAGER,
                    Password = new PasswordHasher().HashPassword("123")
                },
                new User
                {
                    Id = 4, Username = "employee", Role = UserRole.EMPLOYEE,
                    Password = new PasswordHasher().HashPassword("123")
                },
                new User
                {
                    Id = 5, Username = "service", Role = UserRole.SERVICE_MANAGER,
                    Password = new PasswordHasher().HashPassword("123")
                },
                new User
                {
                    Id = 6, Username = "hr", Role = UserRole.HR_MANAGER,
                    Password = new PasswordHasher().HashPassword("123")
                },
                new User
                {
                    Id = 7, Username = "hiring", Role = UserRole.HIRING_MANAGER,
                    Password = new PasswordHasher().HashPassword("123")
                }
            );

            modelBuilder.Entity<EventSchedule>().HasData(
                new EventSchedule { EventScheduleId = 1, ServiceID = 1 }
            );
        }

    }
}
