using StoreManagement.Models;
using StoreManagement.Repositories.RepositoryBase;

namespace StoreManagement.Repositories.ImportStoreRepository
{
    public interface IImportStoreRepository : IRepositoryBase<ImportStore>
    {
        Task<bool> CreateImportStore(ImportStore importStore, List<ImportStoreDetail> importStoreDetails);
        Task<bool> DeleteImportStore(ImportStore importStore);
        Task<bool> UpdateImportStore(ImportStore importStore, List<ImportStoreDetail> importStoreDetails);
    }
}
