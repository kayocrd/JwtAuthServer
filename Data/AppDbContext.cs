using JwtAuthServerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthServerApi.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<User>().HasKey(u => u.Guid);
        builder.Entity<User>().Property(u => u.Guid).ValueGeneratedOnAdd();
        builder.Entity<User>().Property(u => u.FullName).HasColumnType("varchar(55)").IsRequired();
        builder.Entity<User>().Property(u => u.Email).HasColumnType("varchar(30)").IsRequired();
        builder.Entity<User>().Property(u => u.Username).HasColumnType("varchar(15)").IsRequired();
        builder.Entity<User>().Property(u => u.PasswordHash).HasColumnType("varchar(150)").IsRequired();
        builder.Entity<User>().Property(u => u.IsLocked).HasColumnType("bool").IsRequired();
        builder.Entity<User>().Property(u => u.RefreshToken).HasColumnType("varchar(100)").IsRequired();
        builder.Entity<User>().Property(u => u.RefreshTokenExpiryTime).HasColumnType("timestamp with time zone")
            .HasDefaultValue(DateTime.MinValue.ToUniversalTime()).IsRequired();
        
        builder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        builder.Entity<User>().HasIndex(u => u.Email).IsUnique();
    }
}