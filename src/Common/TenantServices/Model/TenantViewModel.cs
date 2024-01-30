namespace TenantServices.Model;
internal class TenantViewModel
{
    internal string? Id { get; set; }
    internal required string Name { get; set; }
    internal ProviderType Provider { get; set; }
    internal required string ConnectionString { get; set; }
}
