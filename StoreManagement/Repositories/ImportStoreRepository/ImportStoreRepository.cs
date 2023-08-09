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

            foreach (var item in importStoreDetails)
            {
                var product = _context.Products.Find(item.ProductId);
                product.Number += item.Quantity;
                _context.Products.Update(product);
            }
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

            var importStoreDetailsOld = _context.ImportStoreDetails.Where(s => s.ImportStoreId == importStore.Id).ToList();
            foreach (var item in importStoreDetailsOld)
            {
                var productOld = _context.Products.Find(item.ProductId);
                productOld.Number -= item.Quantity;
                _context.Products.Update(productOld);
            }
            _context.ImportStoreDetails.RemoveRange(importStoreDetailsOld);

            foreach (var item in importStoreDetails)
            {
                var product = _context.Products.Find(item.ProductId);
                product.Number += item.Quantity;
                _context.Products.Update(product);
            }
            _context.ImportStoreDetails.AddRange(importStoreDetails);

            var isSave = this.Save();
            return isSave;
        }
    }
}
