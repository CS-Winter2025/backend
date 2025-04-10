using CourseProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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

            modelBuilder.Entity<User>()
                .HasOne(u => u.Resident)
                .WithOne(r => r.User)
                .HasForeignKey<User>(u => u.ResidentId)
                .IsRequired(false);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Employee)
                .WithOne(r => r.User)
                .HasForeignKey<User>(u => u.EmployeeId)
                .IsRequired(false);

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
                new Employee { EmployeeId = 1, Name = "Alice", JobTitle = "Manager", EmploymentType = "Full-Time", PayRate = 60000, OrganizationId = 1 },
                new Employee { EmployeeId = 2, Name = "Bob", JobTitle = "Developer", EmploymentType = "Full-Time", PayRate = 50000, OrganizationId = 1, ManagerId = 1 }
            );

            modelBuilder.Entity<Resident>().HasData(
                new Resident
                {
                    ResidentId = 1,
                    Name = "Charlie",
                    IsCurrentlyLiving = true,
                    DetailsJson = "{\"name\": \"Charlie\", \"age\": 45, \"email\": \"charlie@example.com\", \"is_member\": false}"
                },
                new Resident
                {
                    ResidentId = 2,
                    Name = "Diana",
                    IsCurrentlyLiving = true,
                    DetailsJson = "{\"name\": \"Diana\", \"age\": 38, \"email\": \"diana@example.com\", \"is_member\": true}"
                },
                new Resident
                {
                    ResidentId = 3,
                    Name = "Alice",
                    IsCurrentlyLiving = true,
                    DetailsJson = "{\"name\": \"Alice\", \"age\": 30, \"email\": \"alice@example.com\", \"is_member\": true}"
                },
                new Resident
                {
                    ResidentId = 4,
                    Name = "Leo",
                    IsCurrentlyLiving = false,
                    DetailsJson = "{\"name\": \"Leo\", \"age\": 29, \"email\": \"leo@example.com\", \"is_member\": false}"
                },
                new Resident
                {
                    ResidentId = 5,
                    Name = "Amira",
                    IsCurrentlyLiving = true,
                    DetailsJson = "{\"name\": \"Amira\", \"age\": 32, \"email\": \"amira@example.com\", \"is_member\": true}"
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
                    Id = 1,
                    Username = "admin",
                    Role = UserRole.ADMIN,
                    Password = new PasswordHasher().HashPassword("123")
                },
                new User
                {
                    Id = 2,
                    Username = "resident",
                    Role = UserRole.RESIDENT,
                    Password = new PasswordHasher().HashPassword("123"),
                    ResidentId = 1
                },
                new User
                {
                    Id = 3,
                    Username = "housing",
                    Role = UserRole.HOUSING_MANAGER,
                    Password = new PasswordHasher().HashPassword("123")
                },
                new User
                {
                    Id = 4,
                    Username = "employee",
                    Role = UserRole.EMPLOYEE,
                    Password = new PasswordHasher().HashPassword("123"),
                    EmployeeId = 2
                },
                new User
                {
                    Id = 5,
                    Username = "hr",
                    Role = UserRole.HR_MANAGER,
                    Password = new PasswordHasher().HashPassword("123")
                }
            );

            modelBuilder.Entity<EventSchedule>().HasData(
                new EventSchedule { EventScheduleId = 1, ServiceID = 1 }
            );
        }

    }
}
