using Application.Products.Commands.CreateProduct;
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
        group.MapPost("/createProduct", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateProductCommand("Phone", 10, "str");
            var result = await sender.Send(command, cancellationToken);

            return TypedResults.Ok(result);
        });
    }
}
