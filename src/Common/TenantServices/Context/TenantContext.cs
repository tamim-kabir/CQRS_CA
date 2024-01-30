using Microsoft.EntityFrameworkCore;
using TenantServices.Model;

namespace TenantServices.Context;
internal class TenantContext(DbContextOptions<TenantContext> options) : DbContext(options)
{
    internal DbSet<Tenant> Tenants { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = "Server=localhost; Database=Tenant; User=sa; Password=sa123_secret_@#TYuiop; MultipleActiveResultSets=True; TrustServerCertificate=True;";
        optionsBuilder.UseSqlServer(connectionString,
             e => e.MigrationsAssembly(typeof(TenantContext).Assembly.FullName));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(TenantContext).Assembly);
    }
}
