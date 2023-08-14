using StoreManagement.Models;
using StoreManagement.Repositories.RepositoryBase;

namespace StoreManagement.Repositories.UnitRepository
{
    public class UnitRepository: RepositoryBase<Unit>, IUnitRepository
    {
        private readonly StoreManagementContext _context;

        public UnitRepository(StoreManagementContext context) : base(context)
        {
            _context = context;
        }
    }
}
