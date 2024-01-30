using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenantServices.Model;
[Table("Tenant", Schema = "Settings")]
internal class Tenant
{
    [Key]
    internal TenantId Id { get; private set; }
    [MaxLength(50)]
    internal string? Name { get; private set; }
    internal ProviderType Provider { get; private set; }
    [MaxLength(500)]
    internal string? ConnectionString { get; private set; }

    internal Tenant CreateTenant(TenantViewModel model)
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
            //Id = new TenantId(model.Id),
            Name = model.Name,
            ConnectionString = model.ConnectionString,
            Provider = model.Provider,
        };
    }
}
internal record TenantId(Guid Value);
