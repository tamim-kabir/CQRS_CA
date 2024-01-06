using Application.Abstractions.Messaging.Query;

namespace Application.Products.Queries.GetAllProducts;
public sealed record GetAllProductsQuery():IQuery<List<GetAllProductsResponse>>;
