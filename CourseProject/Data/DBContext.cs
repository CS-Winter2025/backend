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
        public DBContext (DbContextOptions<DBContext> options)
            : base(options)
        {
        }

        //public DbSet<CourseProject.Models.Asset> Asset { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Resident>().ToTable("Resident");
        }
        //public DbSet<Resident> Resident { get; set; }
        public DbSet<Asset> Asset { get; set; }
        //public DbSet<Invoice> Invoice { get; set; }
        //public DbSet<Service> Service { get; set; }
    }
}
