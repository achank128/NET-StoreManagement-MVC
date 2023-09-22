using StoreManagement.Models;
using StoreManagement.Repositories.RepositoryBase;

namespace StoreManagement.Repositories.ExportStoreRepository
{
    public interface IExportStoreRepository : IRepositoryBase<ExportStore>
    {
        Task<bool> CreateExportStore(ExportStore exportStore, List<ExportStoreDetail> exportStoreDetails); 
        Task<bool> DeleteExportStore(ExportStore exportStore);
        Task<bool> UpdateExportStore(ExportStore exportStore, List<ExportStoreDetail> exportStoreDetails);
    }
}
