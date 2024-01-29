using Application.Abstractions.Messaging.Query;
using Domain.Repositories.IProduct;
using Domain.Shared;

namespace Application.Products.Queries.GetProductById;
internal sealed class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, GetProductByIdResponse>
{
    private readonly IProductRepository _repository;

    public GetProductByIdQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<GetProductByIdResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetProductById(request.Id, cancellationToken);
        if (product is null)
        {
            return Result.Failure<GetProductByIdResponse>(new Error(
                "Product.NotFound",
                $"The Product with Id {request.Id} was not found"));
        }
        var response = new GetProductByIdResponse(product.Id, product.Name);
        return response;
    }
}
