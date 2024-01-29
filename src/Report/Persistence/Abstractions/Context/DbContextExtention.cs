using Domain.Entities.Core.Audit;
using Domain.Enums.Audit;
using Domain.Premetives;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Configarations;
using Persistence.Contexts;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;

namespace Persistence.Abstractions.Context;
public abstract class DbContextExtention : DbContext
{
    protected internal readonly HttpContext? _httpContext;
    internal DbContextExtention(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        _httpContext = httpContextAccessor.HttpContext;
    }
    public DbSet<AuditTrail> AuditTrails { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        foreach (var property in builder.Model.GetEntityTypes()
       .SelectMany(t => t.GetProperties())
       .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(builder);

    }

    private string GetRemoteIpAddress(HttpRequest? request)
    {
        string? ip = "::1";
        if (request is { HttpContext: not null })
        {
            ip = request.HttpContext.Connection.RemoteIpAddress?.ToString();
        }
        if (ip == "::1")
        {
            ip = GetLocalIpAddress();
        }
        return ip ?? "System";
    }
    private string GetLocalIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "No network adapters with an IPv4 address in the system!";
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        string? userId = _httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId ?? "System";
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    entry.Entity.CreatedIPAddress = GetRemoteIpAddress(_httpContext?.Request);

                    break;
                case EntityState.Modified:
                    if (entry.Entity.Deleted)
                    {
                        entry.Entity.DeletedBy = userId ?? "System";
                        entry.Entity.DeletedTime = DateTime.UtcNow;
                        entry.Entity.DeletedIPAddress = GetRemoteIpAddress(_httpContext?.Request);
                    }
                    else
                    {
                        entry.Entity.LastModifiedBy = userId ?? "System";
                        entry.Entity.LastModifiedDate = DateTime.UtcNow;
                        entry.Entity.ModifiedIPAddress = GetRemoteIpAddress(_httpContext?.Request);
                    }
                    break;
            }
        }
        if (userId is null)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        return await SaveChangesAsync(userId, cancellationToken);
    }
    public virtual async Task<int> SaveChangesAsync(string? userId = null, CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId ?? "System";
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    entry.Entity.CreatedIPAddress = GetRemoteIpAddress(_httpContext?.Request);
                    break;
                case EntityState.Modified:
                    if (entry.Entity.Deleted)
                    {
                        entry.Entity.DeletedBy = userId ?? "System";
                        entry.Entity.DeletedTime = DateTime.UtcNow;
                        entry.Entity.DeletedIPAddress = GetRemoteIpAddress(_httpContext?.Request);
                    }
                    else
                    {
                        entry.Entity.LastModifiedBy = userId ?? "System";
                        entry.Entity.LastModifiedDate = DateTime.UtcNow;
                        entry.Entity.ModifiedIPAddress = GetRemoteIpAddress(_httpContext?.Request);
                    }
                    break;
            }
        }
        var auditEntries = OnBeforeSaveChanges(userId);
        var result = await base.SaveChangesAsync();
        await OnAfterSaveChanges(auditEntries);
        return result;
    }
    private List<AuditEntry> OnBeforeSaveChanges(string? userId)
    {
        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditTrail || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry);
            auditEntry.TableName = entry.Entity.GetType().Name;
            auditEntry.UserId = userId;
            auditEntries.Add(auditEntry);
            foreach (var property in entry.Properties)
            {
                if (property is { CurrentValue: null } || property is { OriginalValue: null })
                {
                    continue;
                }
                if (property.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(property);
                    continue;
                }

                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.AuditType = AuditType.Create;
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        auditEntry.AuditType = AuditType.Delete;
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.ChangedColumns.Add(propertyName);
                            auditEntry.AuditType = AuditType.Update;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }
        }
        foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
        {
            AuditTrails.Add(auditEntry.ToAudit());
        }
        return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
    }
    private Task OnAfterSaveChanges(List<AuditEntry> auditEntries, CancellationToken cancellationToken = default)
    {
        if (auditEntries == null || auditEntries.Count == 0)
            return Task.CompletedTask;

        foreach (var auditEntry in auditEntries)
        {
            foreach (var prop in auditEntry.TemporaryProperties)
            {
                if (prop is { CurrentValue: null })
                {
                    continue;
                }
                if (prop.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                }
                else
                {
                    auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                }
            }
            AuditTrails.Add(auditEntry.ToAudit());
        }
        return SaveChangesAsync();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.LogTo(x =>
            Debug.WriteLine("Api Database --> \n" + x + "\n"),
            LogLevel.Information,
            Microsoft.EntityFrameworkCore.Diagnostics.DbContextLoggerOptions.None);
#endif
    }

}
