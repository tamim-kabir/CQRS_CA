using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories.IProduct;
public interface IProductRepository
{
    Task CreateProduct();
    Task<Product> GetProductById(int id, CancellationToken cancellationToken);
    Task<List<Product>> GetAllProducts();
}
