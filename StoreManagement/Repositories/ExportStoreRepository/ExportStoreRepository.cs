using AspNetCoreHero.ToastNotification.Abstractions;
using StoreManagement.Models;
using StoreManagement.Repositories.RepositoryBase;

namespace StoreManagement.Repositories.ExportStoreRepository
{
    public class ExportStoreRepository : RepositoryBase<ExportStore>, IExportStoreRepository
    {
        private readonly StoreManagementContext _context;
        private readonly INotyfService _notyf;

        public ExportStoreRepository(StoreManagementContext context, INotyfService notyf) : base(context)
        {
            _context = context;
            _notyf = notyf;
        }

        public async Task<bool> CreateExportStore(ExportStore exportStore, List<ExportStoreDetail> exportStoreDetails)
        {
            _context.ExportStores.Add(exportStore);

            foreach (var item in exportStoreDetails)
            {
                var product = _context.Products.Find(item.ProductId);
                product.Number -= item.Quantity;
                if (product.Number < 0)
                {
                    _notyf.Error("Không thể thêm! Số lượng sản phẩm: " + product.ProductName + " đạt giới hạn.");
                    return false;
                }
                _context.Products.Update(product);
            }
            _context.ExportStoreDetails.AddRange(exportStoreDetails);

            var isSave = this.Save();
            return isSave;
        }

        public async Task<bool> DeleteExportStore(ExportStore exportStore)
        {
            var exportStoreDetails = _context.ExportStoreDetails.Where(x => x.ExportStoreId == exportStore.Id).ToList();
            foreach (var item in exportStoreDetails)
            {
                var product = _context.Products.Find(item.ProductId);
                product.Number += item.Quantity;
                _context.Products.Update(product);
            }
            _context.ExportStoreDetails.RemoveRange(exportStoreDetails);
            _context.ExportStores.Remove(exportStore);

            var isSave = this.Save();
            return isSave;
        }

        public async Task<bool> UpdateExportStore(ExportStore exportStore, List<ExportStoreDetail> exportStoreDetails)
        {
            _context.ExportStores.Update(exportStore);

            var exportStoreDetailsOld = _context.ExportStoreDetails.Where(x => x.ExportStoreId == exportStore.Id).ToList();
            foreach (var item in exportStoreDetailsOld)
            {
                var product = _context.Products.Find(item.ProductId);
                product.Number += item.Quantity;
                _context.Products.Update(product);
            }
            _context.ExportStoreDetails.RemoveRange(exportStoreDetailsOld);

            foreach (var item in exportStoreDetails)
            {
                var product = _context.Products.Find(item.ProductId);
                product.Number -= item.Quantity;
                if (product.Number < 0)
                {
                    _notyf.Error("Không thể cập nhật! Số lượng sản phẩm: " + product.ProductName + " đạt giới hạn.");
                    return false;
                }
                _context.Products.Update(product);
            }
            _context.ExportStoreDetails.AddRange(exportStoreDetails);

            var isSave = this.Save();
            return isSave;
        }
    }
}
