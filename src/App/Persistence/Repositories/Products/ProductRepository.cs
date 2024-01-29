using Domain.Entities;
using Domain.Repositories.IProduct;

namespace Persistence.Repositories.Products;
internal sealed class ProductRepository : IProductRepository
{
    public Task CreateProduct()
    {
        return Task.CompletedTask;
    }
    public Task<Product> GetProductById(int id, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Product
        {
            Name = "Phone",
            Id = id,
            Size = 4.8M,
            Quentity = 5
        });
    }
    public Task<List<Product>> GetAllProducts()
    {
        return Task.FromResult(new List<Product>());
    }
}
