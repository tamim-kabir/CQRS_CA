using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenantServices.Model;
[Table("Tenant", Schema = "Settings")]
internal class Tenant
{
    [Key]
    public TenantId? Id { get; private set; }
    [MaxLength(50)]
    public string? Name { get; private set; }
    public ProviderType Provider { get; private set; }
    [MaxLength(500)]
    public string? ConnectionString { get; private set; }

    internal static Tenant? CreateTenant(TenantViewModel model)
    {
        return new Tenant
        {
            Id = new TenantId(Guid.NewGuid()),
            Name = model.Name,
            ConnectionString = model.ConnectionString,
            Provider = model.Provider,
        };
    }
    internal Tenant UpdateTenant(TenantViewModel model)
    {
        return new Tenant
        {
            Id = new TenantId(Guid.Parse(model.Id!)),
            Name = model.Name,
            ConnectionString = model.ConnectionString,
            Provider = model.Provider,
        };
    }
}
internal record TenantId(Guid Value);
