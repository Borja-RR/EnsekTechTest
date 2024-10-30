using EnsekTechTest.Repository.Dto;
using Microsoft.EntityFrameworkCore;

namespace EnsekTechTest.Repository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Meters> Meters { get; set; }
        public DbSet<Accounts> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Primary keys
            modelBuilder.Entity<Accounts>().HasKey(a => a.AccountId);
            modelBuilder.Entity<Meters>().HasKey(a => a.AccountId);

            // One-to-many relationship between Accounts and Meters
            modelBuilder.Entity<Meters>()
                .HasOne(m => m.Account)
                .WithMany(a => a.Meters)
                .HasForeignKey(m => m.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}

