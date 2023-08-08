using StoreManagement.Models;
using StoreManagement.Repositories.RepositoryBase;

namespace StoreManagement.Repositories.ImportStoreRepository
{
    public class ImportStoreRepository : RepositoryBase<ImportStore>, IImportStoreRepository
    {
        private readonly StoreManagementDbContext _context;

        public ImportStoreRepository(StoreManagementDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
