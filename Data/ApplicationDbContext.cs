using CompanyManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CompanyManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<BranchSupplier> BranchesSupplier { get; set; }
        public DbSet<WorksWith> WorksWith { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WorksWith>()
                .HasKey(w => new { w.EmpId, w.ClientId });
            modelBuilder.Entity<WorksWith>()
                .HasOne(w => w.Employee)
                .WithMany()
                .HasForeignKey(w => w.EmpId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorksWith>()
                .HasOne(w => w.Client)
                .WithMany()
                .HasForeignKey(w => w.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Supervisor)
                .WithMany()
                .HasForeignKey(e => e.SupervisorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Branch)
                .WithMany(b => b.Employees)
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<BranchSupplier>()
                .HasKey(bs=> new {bs.BranchId, bs.SupplierName});

            modelBuilder.Entity<BranchSupplier>()
                .HasOne(bs => bs.Branch)
                .WithMany(b => b.BranchSuppliers)
                .HasForeignKey(bs => bs.BranchId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
