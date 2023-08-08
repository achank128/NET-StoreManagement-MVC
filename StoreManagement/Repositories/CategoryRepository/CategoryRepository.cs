using StoreManagement.Models;
using StoreManagement.Repositories.ProductRepository;
using StoreManagement.Repositories.RepositoryBase;

namespace StoreManagement.Repositories.CategoryRepository
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        private readonly StoreManagementDbContext _context;

        public CategoryRepository(StoreManagementDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
