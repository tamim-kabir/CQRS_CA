using Microsoft.AspNetCore.Mvc;
using TenantServices.Context;
using TenantServices.Model;

namespace TenantServices.Endpoint;

public static class TenantEndpoint
{
    internal static void TenantMap(this IEndpointRouteBuilder app)
    {
        app.MapGroup("api/Tenant");
        app.MapPost("/Create", async (TenantViewModel model, TenantContext _db, CancellationToken cancellationToken) =>
        {
            var tenent = Tenant.CreateTenant(model);
            await _db.AddAsync(tenent!);
            await _db.SaveChangesAsync();

            return TypedResults.Ok();
        });
    }
}
