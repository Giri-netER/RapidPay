using Microsoft.EntityFrameworkCore;
using RapidPay.Models;

namespace RapidPay.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Card> Cards { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Card>()
                .Property(c => c.CardNumber)
                .HasMaxLength(15)
                .IsRequired();

            modelBuilder.Entity<Card>()
                .HasIndex(c => c.CardNumber)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(c => c.Email)
                .IsUnique();

        }
    }


}

