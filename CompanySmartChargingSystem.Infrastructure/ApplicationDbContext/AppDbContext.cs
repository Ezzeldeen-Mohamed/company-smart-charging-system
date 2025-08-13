using CompanySmartChargingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
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

            // rename Identity tables

            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");

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

            // decimal precision
            modelBuilder.Entity<ChargeTransaction>()
                .Property(ct => ct.AmountPaid)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ChargeTransaction>()
                .Property(ct => ct.FeesValue)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ChargeTransaction>()
                .Property(ct => ct.NetValue)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Customer>()
                .Property(c => c.AmountPaid)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Customer>()
                .Property(c => c.NetPaid)
                .HasColumnType("decimal(18,2)");

            // Soft Delete Filters
            modelBuilder.Entity<Contract>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Customer>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Meter>().HasQueryFilter(m => !m.IsDeleted);
            modelBuilder.Entity<ChargeTransaction>().HasQueryFilter(ct => !ct.IsDeleted);
        }
    }

}
