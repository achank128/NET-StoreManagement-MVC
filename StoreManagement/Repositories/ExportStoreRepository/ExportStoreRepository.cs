using StoreManagement.Models;
using StoreManagement.Models.Request;
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

        public async Task<bool> CreateExportStore(ExportStore exportStore, List<ExportStoreDetail> exportStoreDetails)
        {
            _context.ExportStores.Add(exportStore);
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

            foreach (var item in exportStoreDetails)
            {
                var detail = _context.ExportStoreDetails.Find(item.Id);
                if(detail == null)
                {
                    _context.ExportStoreDetails.Add(item);
                    var product = _context.Products.Find(item.ProductId);
                    product.Number -= item.Quantity;
                    _context.Products.Update(product);
                }
                else
                {
                    var productOld = _context.Products.Find(detail.ProductId);
                    productOld.Number += detail.Quantity;
                    _context.Products.Update(productOld);

                    var productNew = _context.Products.Find(item.ProductId);
                    productNew.Number -= item.Quantity;
                    _context.Products.Update(productNew);

                    detail.ProductId = item.ProductId;
                    detail.Quantity = item.Quantity;
                    detail.ExportPrice= item.ExportPrice;
                    _context.ExportStoreDetails.Update(detail);
                }
            }

            var isSave = this.Save();
            return isSave;
        }
    }
}
