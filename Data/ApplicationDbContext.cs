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
            modelBuilder.Entity<Employee>()
        .HasOne(e => e.Supervisor)
        .WithMany()
        .HasForeignKey(e => e.SupervisorId);
        }
    }
}
