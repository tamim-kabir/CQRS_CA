using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TenantServices.Model;

namespace TenantServices.ModelConfiguration;
internal class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).HasConversion(
            tenantId => tenantId.Value,
            value => new TenantId(value));
        builder.Property(t => t.Name);
        builder.Property(t => t.Provider);
        builder.Property(t => t.ConnectionString).HasMaxLength(500);
        builder.HasIndex(t => t.Id);
    }
}
