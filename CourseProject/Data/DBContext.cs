using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CourseProject.Models;

namespace CourseProject.Data
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {
        }
        public DBContext() { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CourseProject.Models.Asset>().ToTable("Asset");
            modelBuilder.Entity<CourseProject.Models.Service>().ToTable("Service");
            modelBuilder.Entity<CourseProject.Models.Employee>().ToTable("Employee");
        }
        public DbSet<CourseProject.Models.Asset> Asset { get; set; } = default!;
        public DbSet<CourseProject.Models.Service> Service { get; set; } = default!;
        public DbSet<CourseProject.Models.Employee> Employee { get; set; } = default!;
    }
}