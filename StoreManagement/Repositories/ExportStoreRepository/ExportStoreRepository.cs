using StoreManagement.Models;
using StoreManagement.Repositories.RepositoryBase;

namespace StoreManagement.Repositories.ExportStoreRepository
{
    public class ExportStoreRepository : RepositoryBase<ExportStore>, IExportStoreRepository
    {
        private readonly StoreManagementContext _context;

        public ExportStoreRepository(StoreManagementContext context) : base(context)
        {
            _context = context;
        }
    }
}
