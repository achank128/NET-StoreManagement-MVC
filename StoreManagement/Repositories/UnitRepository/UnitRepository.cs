using StoreManagement.Models;
using StoreManagement.Repositories.RepositoryBase;

namespace StoreManagement.Repositories.UnitRepository
{
    public class UnitRepository: RepositoryBase<Unit>, IUnitRepository
    {
        private readonly StoreManagementDbContext _context;

        public UnitRepository(StoreManagementDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
