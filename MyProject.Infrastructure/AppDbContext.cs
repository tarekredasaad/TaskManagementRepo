using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MyProject.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.HasIndex(x => x.Email).IsUnique();
                builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
                builder.Property(x => x.Email).HasMaxLength(255).IsRequired();
                builder.Property(x => x.PasswordHash).IsRequired();
                builder.Property(x => x.Role).HasMaxLength(20).IsRequired();
                builder.HasQueryFilter(x => !x.IsDeleted);
            });

            modelBuilder.Entity<TaskItem>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
                builder.Property(x => x.Description).HasMaxLength(1500);
                builder.HasOne(x => x.User)
                    .WithMany(x => x.Tasks)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
