using Domain.Entities;

namespace Domain.Repositories.IProduct;
public interface IProductRepository
{
    Task CreateProduct();
    Task<Product> GetProductById(int id, CancellationToken cancellationToken);
    Task<List<Product>> GetAllProducts();
}
