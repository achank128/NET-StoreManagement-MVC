using StoreManagement.Models;
using StoreManagement.Repositories.RepositoryBase;

namespace StoreManagement.Repositories.ProductPostRepository
{
    public class ProductPostRepository : RepositoryBase<ProductPost>, IProductPostRepository
    {
        public ProductPostRepository(StoreManagementContext context) : base(context)
        {
        }
    }
}
