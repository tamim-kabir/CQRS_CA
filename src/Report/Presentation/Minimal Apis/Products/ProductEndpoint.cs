using Application.Products.Queries.GetProductById;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Presentation.Minimal_Apis.Products;
public class ProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/endpoint");
        group.MapPost("/GetProduct", async (int id, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetProductByIdQuery(id);
            var result = await sender.Send(query, cancellationToken);

            return TypedResults.Ok(result);
        });
    }
}
