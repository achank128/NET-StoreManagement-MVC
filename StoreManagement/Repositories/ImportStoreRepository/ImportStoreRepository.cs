using StoreManagement.Models;
using StoreManagement.Repositories.RepositoryBase;

namespace StoreManagement.Repositories.ImportStoreRepository
{
    public class ImportStoreRepository : RepositoryBase<ImportStore>, IImportStoreRepository
    {
        private readonly StoreManagementContext _context;

        public ImportStoreRepository(StoreManagementContext context) : base(context)
        {
            _context = context;
        }
    }
}
