using StoreManagement.Models;
using StoreManagement.Repositories.RepositoryBase;

namespace StoreManagement.Repositories.ProductRepository
{
    public interface IProductRepository : IRepositoryBase<Product>
    {
        Task<List<Product>> GetAllProducts();
    }
}
