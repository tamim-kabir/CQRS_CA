using Application.Abstractions.Messaging.Query;

namespace Application.Products.Queries.GetProductById;
public sealed record GetProductByIdQuery(int Id) : IQuery<GetProductByIdResponse>;
