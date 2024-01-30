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

        builder.HasIndex(t => t.Id);
    }
}
