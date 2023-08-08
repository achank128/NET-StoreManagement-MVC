using StoreManagement.Models;
using StoreManagement.Repositories.RepositoryBase;

namespace StoreManagement.Repositories.ExportStoreRepository
{
    public class ExportStoreRepository : RepositoryBase<ExportStore>, IExportStoreRepository
    {
        private readonly StoreManagementDbContext _context;

        public ExportStoreRepository(StoreManagementDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
