using Application.Abstractions.Messaging.Query;
using Domain.Repositories.IProduct;
using Domain.Shared;

namespace Application.Products.Queries.GetAllProducts;
internal sealed class GetAllProductsQueryHandler : IQueryHandler<GetAllProductsQuery, List<GetAllProductsResponse>>
{
    private readonly IProductRepository _repository;

    public GetAllProductsQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }
    public async Task<Result<List<GetAllProductsResponse>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        await _repository.GetAllProducts();
        var products = new List<GetAllProductsResponse>
        {
            new GetAllProductsResponse(1, "Phone", "L"),
            new GetAllProductsResponse(1, "Camera", "M"),
            new GetAllProductsResponse(1, "Light", "L"),
            new GetAllProductsResponse(1, "Laptop", "L"),
        };
        if(products.Count <= 0 )
        {
            return Result.Failure<List<GetAllProductsResponse>>(new Error(
                    "Product.NotFound",
                    "There is't any products"));
        }
        return products;
    }
}
