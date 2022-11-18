using JwtAuthServer.Models;
using JwtAuthServer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthServer.Data;

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
        builder.Entity<User>().Property(u => u.PhoneNumber).HasColumnType("varchar(15)").IsRequired();
        builder.Entity<User>().Property(u => u.Email).HasColumnType("varchar(30)").IsRequired();
        builder.Entity<User>().Property(u => u.Identifier).HasColumnType("varchar(15)").IsRequired();
        builder.Entity<User>().Property(u => u.PasswordHash).HasColumnType("varchar(150)").IsRequired();
        builder.Entity<User>().Property(u => u.IsVerified).HasColumnType("bool").IsRequired();
        builder.Entity<User>().Property(u => u.IsLocked).HasColumnType("bool").IsRequired();
        builder.Entity<User>().Property(u => u.RefreshToken).HasColumnType("varchar(100)").IsRequired();
        builder.Entity<User>().Property(u => u.ValidationCode).HasColumnType("varchar(6)").IsRequired();
        builder.Entity<User>().Property(u => u.RefreshTokenExpiryTime).HasColumnType("timestamp with time zone")
            .HasDefaultValue(DateTime.MinValue.ToUniversalTime()).IsRequired();
        
        builder.Entity<User>().HasIndex(u => u.Identifier).IsUnique();
        builder.Entity<User>().HasIndex(u => u.Email).IsUnique();
    }
}