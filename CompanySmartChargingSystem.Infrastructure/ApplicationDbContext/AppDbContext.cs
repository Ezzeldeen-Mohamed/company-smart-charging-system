using CompanySmartChargingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace CompanySmartChargingSystem.Domain.ApplicationDbContext  
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options){}

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Entities.Meter> Meters { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ChargeTransaction> ChargeTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // Unique Constraints
            modelBuilder.Entity<Contract>()
                .HasIndex(c => c.CustomerCode)
                .IsUnique();

            modelBuilder.Entity<Contract>()
                .HasIndex(c => c.UniqueCode)
                .IsUnique();

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.NationalId)
                .IsUnique();

            modelBuilder.Entity<Meter>()
                .HasIndex(m => m.Serial)
                .IsUnique();

            // Soft Delete Filters
            modelBuilder.Entity<Contract>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Customer>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Meter>().HasQueryFilter(m => !m.IsDeleted);
            modelBuilder.Entity<ChargeTransaction>().HasQueryFilter(ct => !ct.IsDeleted);
        }
    }

}
