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

        public async Task<bool> CreateImportStore(ImportStore importStore, List<ImportStoreDetail> importStoreDetails)
        {
            _context.ImportStores.Add(importStore);
            _context.ImportStoreDetails.AddRange(importStoreDetails);

            var isSave = this.Save();
            return isSave;
        }

        public async Task<bool> DeleteImportStore(ImportStore importStore)
        {
            var importStoreDetails = _context.ImportStoreDetails.Where(x => x.ImportStoreId == importStore.Id).ToList();

            foreach (var item in importStoreDetails)
            {
                var product = _context.Products.Find(item.ProductId);
                product.Number -= item.Quantity;
                _context.Products.Update(product);
            }

            _context.ImportStoreDetails.RemoveRange(importStoreDetails);
            _context.ImportStores.Remove(importStore);

            var isSave = this.Save();
            return isSave;
        }

        public async Task<bool> UpdateImportStore(ImportStore importStore, List<ImportStoreDetail> importStoreDetails)
        {
            _context.ImportStores.Update(importStore);

            foreach (var item in importStoreDetails)
            {
                var detail = _context.ImportStoreDetails.Find(item.Id);
                if (detail == null)
                {
                    _context.ImportStoreDetails.Add(item);
                    var product = _context.Products.Find(item.ProductId);
                    product.Number += item.Quantity;
                    _context.Products.Update(product);
                }
                else
                {
                    var productOld = _context.Products.Find(detail.ProductId);
                    productOld.Number -= detail.Quantity;
                    _context.Products.Update(productOld);

                    var productNew = _context.Products.Find(item.ProductId);
                    productNew.Number += item.Quantity;
                    _context.Products.Update(productNew);

                    detail.ProductId = item.ProductId;
                    detail.Quantity = item.Quantity;
                    detail.ImportPrice = item.ImportPrice;
                    _context.ImportStoreDetails.Update(detail);
                }
            }

            var isSave = this.Save();
            return isSave;
        }
    }
}
