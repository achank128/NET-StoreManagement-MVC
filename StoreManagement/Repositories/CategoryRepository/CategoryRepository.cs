using StoreManagement.Models;
using StoreManagement.Repositories.ProductRepository;
using StoreManagement.Repositories.RepositoryBase;

namespace StoreManagement.Repositories.CategoryRepository
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        private readonly StoreManagementContext _context;

        public CategoryRepository(StoreManagementContext context) : base(context)
        {
            _context = context;
        }
    }
}
