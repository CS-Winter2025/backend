using CourseProject.Common;
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

            modelBuilder.Entity<ScheduleBase>().UseTphMappingStrategy();

            modelBuilder.Entity<Models.File>()
            .HasOne(f => f.ScheduleBase)
            .WithMany(s => s.Attachments)
            .HasForeignKey(f => f.ScheduleBaseID);

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
                .HasForeignKey(i => i.ResidentId)
                .IsRequired();

            modelBuilder.Entity<Organization>().ToTable("Organization").HasData(
                new Organization { OrganizationId = 1 },
                new Organization { OrganizationId = 2 }
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    EmployeeId = 1,
                    JobTitle = "Manager",
                    EmploymentType = "Full-Time",
                    PayRate = 60000,
                    OrganizationId = 1,
                    ProfilePicture = SamplePictures.Sample1
                },
                new Employee
                {
                    EmployeeId = 2,
                    JobTitle = "Developer",
                    EmploymentType = "Full-Time",
                    PayRate = 50000,
                    OrganizationId = 1,
                    ManagerId = 1,
                    ProfilePicture = SamplePictures.Sample4
                },
                new Employee
                {
                    EmployeeId = 3,
                    JobTitle = "Technician",
                    EmploymentType = "Part-Time",
                    PayRate = 40000,
                    OrganizationId = 2,
                    ProfilePicture = null
                },
                new Employee
                {
                    EmployeeId = 4,
                    JobTitle = "HR Specialist",
                    EmploymentType = "Full-Time",
                    PayRate = 55000,
                    OrganizationId = 2,
                    ProfilePicture = SamplePictures.Sample3
                },
                new Employee
                {
                    EmployeeId = 5,
                    JobTitle = "Recruiter",
                    EmploymentType = "Full-Time",
                    PayRate = 53000,
                    OrganizationId = 2,
                    ProfilePicture = SamplePictures.Sample2
                },
                new Employee
                {
                    EmployeeId = 6,
                    JobTitle = "Housing Manager",
                    EmploymentType = "Full-Time",
                    PayRate = 54500,
                    OrganizationId = 2,
                    ProfilePicture = null
                }
            );

            modelBuilder.Entity<Service>().HasData(
                new Service { ServiceID = 1, Type = "Cleaning", Rate = 50 },
                new Service { ServiceID = 2, Type = "Security", Rate = 100 },
                new Service { ServiceID = 3, Type = "Dining", Rate = 69 },
                new Service { ServiceID = 4, Type = "Health Care", Rate = 75 },
                new Service { ServiceID = 5, Type = "Transportation", Rate = 25 }
            );

            modelBuilder.Entity<Employee>().OwnsOne(e => e.Name).HasData(
                new { EmployeeId = 1, FirstName = "Alice", LastName = "Smith" },
                new { EmployeeId = 2, FirstName = "Bob", LastName = "Johnson" },
                new { EmployeeId = 3, FirstName = "Cathy", LastName = "Lee" },
                new { EmployeeId = 4, FirstName = "James", LastName = "Taylor" },
                new { EmployeeId = 5, FirstName = "Nina", LastName = "Martinez" },
                new { EmployeeId = 6, FirstName = "Joe", LastName = "Lewis" }
            );

            modelBuilder.Entity<Employee>().OwnsOne(e => e.Address).HasData(
                new { EmployeeId = 1, Street = "123 Main St", City = "New York", State = "NY", Country = "USA", ZipCode = "10001" },
                new { EmployeeId = 2, Street = "456 Park Ave", City = "New York", State = "NY", Country = "USA", ZipCode = "10002" },
                new { EmployeeId = 3, Street = "321 Oak St", City = "Dallas", State = "TX", Country = "USA", ZipCode = "75201" },
                new { EmployeeId = 4, Street = "456 Pine St", City = "Houston", State = "TX", Country = "USA", ZipCode = "77002" },
                new { EmployeeId = 5, Street = "789 Cedar St", City = "Austin", State = "TX", Country = "USA", ZipCode = "73301" },
                new { EmployeeId = 6, Street = "210 Alder St", City = "Houston", State = "TX", Country = "USA", ZipCode = "77042" }
            );

            _ = modelBuilder.Entity<Resident>().HasData(
               new Resident
               {
                   ResidentId = 1,
                   IsCurrentlyLiving = true,
                   DetailsJson = @"{""Emergency Contact"": ""Bob"", ""Age"": 69, ""Email"": ""Bob@example.com""}",
                   ProfilePicture = SamplePictures.Sample2,
                   ServiceSubscriptionIds = [3, 4]
               },
               new Resident
               {
                   ResidentId = 2,
                   IsCurrentlyLiving = true,
                   DetailsJson = @"{""Phone Number"": ""778-555-6942"", ""Age"": 30, ""Email"": ""Test@example.com""}",
                   ProfilePicture = SamplePictures.Sample3,
                   ServiceSubscriptionIds = [2, 4]
               },
               new Resident
               {
                   ResidentId = 3,
                   IsCurrentlyLiving = true,
                   DetailsJson = @"{""Mobile Number"": ""778-555-2025"", ""Age"": 42, ""Email"": ""Meh@example.com"", ""Phone Number"": ""604-555-2021""}",
                   ProfilePicture = SamplePictures.Sample4,
                   ServiceSubscriptionIds = [1, 2]
               },
               new Resident
               {
                   ResidentId = 4,
                   IsCurrentlyLiving = false,
                   DetailsJson = @"{""Age"": 29}"
               },
               new Resident
               {
                   ResidentId = 5,
                   IsCurrentlyLiving = true,
                   DetailsJson = @"{""Age"": 32, ""Email"": ""Amira@example.com""}",
                   ProfilePicture = SamplePictures.Sample1,
                   ServiceSubscriptionIds = [1, 3, 4, 5]
               }
           );

            modelBuilder.Entity<Resident>().OwnsOne(r => r.Name).HasData(
                new { ResidentId = 1, FirstName = "Charlie", LastName = "Brown" },
                new { ResidentId = 2, FirstName = "Diana", LastName = "Prince" },
                new { ResidentId = 3, FirstName = "Alison", LastName = "Smythe" },
                new { ResidentId = 4, FirstName = "Leo", LastName = "Johnson" },
                new { ResidentId = 5, FirstName = "Amira", LastName = "Williams" }
            );

            modelBuilder.Entity<Resident>().OwnsOne(r => r.Address).HasData(
                new { ResidentId = 1, Street = "789 Oak St", City = "Los Angeles", State = "CA", Country = "USA", ZipCode = "90001" },
                new { ResidentId = 2, Street = "101 Pine St", City = "San Francisco", State = "CA", Country = "USA", ZipCode = "94101" },
                new { ResidentId = 3, Street = "456 Elm St", City = "Chicago", State = "IL", Country = "USA", ZipCode = "60601" },
                new { ResidentId = 4, Street = "789 Maple Ave", City = "Austin", State = "TX", Country = "USA", ZipCode = "73301" },
                new { ResidentId = 5, Street = "123 Cedar Rd", City = "Seattle", State = "WA", Country = "USA", ZipCode = "98101" }
            );

            modelBuilder.Entity<Invoice>().HasData(
                new Invoice { InvoiceID = 1, ResidentId = 1, Date = DateTime.UtcNow, AmountDue = 200, AmountPaid = 100 },
                new Invoice { InvoiceID = 2, ResidentId = 3, Date = DateTime.UtcNow, AmountDue = 420, AmountPaid = 69 }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Role = UserRole.ADMIN,
                    Password = new PasswordHasher().HashPassword("123"),
                    EmployeeId = 1,
                },
                new User
                {
                    Id = 2,
                    Username = "resident",
                    Role = UserRole.RESIDENT,
                    Password = new PasswordHasher().HashPassword("123"),
                    ResidentId = 1,
                },
                new User
                {
                    Id = 3,
                    Username = "housing",
                    Role = UserRole.HOUSING_MANAGER,
                    Password = new PasswordHasher().HashPassword("123"),
                    EmployeeId = 6,
                },
                new User
                {
                    Id = 4,
                    Username = "employee",
                    Role = UserRole.EMPLOYEE,
                    Password = new PasswordHasher().HashPassword("123"),
                    EmployeeId = 2,
                },
                new User
                {
                    Id = 5,
                    Username = "hr",
                    Role = UserRole.HR_MANAGER,
                    Password = new PasswordHasher().HashPassword("123"),
                    EmployeeId = 4,
                }
            );

            modelBuilder.Entity<EventSchedule>().HasData(
                new EventSchedule { EventScheduleId = 1, ServiceID = 1 }
            );
        }

    }
}