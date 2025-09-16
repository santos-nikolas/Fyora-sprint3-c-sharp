using Fyora_sprint3.Models;
using Microsoft.EntityFrameworkCore;

namespace Fyora_sprint3.Data
{
    public class FyoraContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<ProgressLog> ProgressLogs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=fyora_admin.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Nickname)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Nickname)
                .IsRequired()
                .HasMaxLength(120);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            // ★ Default no banco para CreatedAt
            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ProgressLog>()
                .Property(p => p.Achievement)
                .HasMaxLength(300);

            modelBuilder.Entity<ProgressLog>()
                .Property(p => p.LogDate)
                .IsRequired();

            modelBuilder.Entity<ProgressLog>()
                .HasOne(p => p.User)
                .WithMany(u => u.ProgressLogs)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
