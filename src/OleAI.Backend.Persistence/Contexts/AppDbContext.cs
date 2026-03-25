namespace OleAI.Backend.Persistence.Contexts;

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Domain.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<OperationClaim> OperationClaims { get; set; }
    public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // çalışan Assembly (proje) içindeki tüm IEntityTypeConfiguration arayüzünü uygulayan
        // sınıfları bulmasını ve otomatik olarak Apply etmesini söylüyoruz.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);

    }
}
