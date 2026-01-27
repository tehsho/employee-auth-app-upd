using EmployeeAuth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAuth.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<User>();

        user.HasKey(x => x.Id);

        user.HasIndex(x => x.UserName)
            .IsUnique();

        user.HasIndex(x => x.Email)
            .IsUnique();

        user.Property(x => x.UserName)
            .IsRequired()
            .HasMaxLength(50);

        user.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        user.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);

        user.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);
    }
}
