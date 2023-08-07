using StoreManagement.Models;
using StoreManagement.Repositories.RepositoryBase;

namespace StoreManagement.Repositories.ProductRepository
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        private readonly StoreManagementContext _context;

        public ProductRepository(StoreManagementContext context) : base(context) 
        {
            _context = context;
        }
    }
}
