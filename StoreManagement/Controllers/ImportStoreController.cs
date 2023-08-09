using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;
using StoreManagement.Models.Request;
using StoreManagement.Repositories.ImportStoreRepository;
using StoreManagement.Repositories.ProductRepository;


namespace StoreManagement.Controllers
{
    public class ImportStoreController : Controller
    {
        private readonly IImportStoreRepository _importStoreRepository;
        private readonly IProductRepository _productRepository;

        public ImportStoreController(
            IImportStoreRepository importStoreRepository,
            IProductRepository productRepository
            )
        {
            _importStoreRepository = importStoreRepository;
            _productRepository = productRepository;
        }
        public IActionResult Index()
        {
            var importStores = _importStoreRepository.GetQueryable()
                .ToList()
                .OrderByDescending(s => s.CreatedDate);
            return View(importStores);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateImportStore([FromBody] ImportStoreRequest importStoreRequest)
        {
            ImportStore importStore = new ImportStore();
            importStore.Id = Guid.NewGuid();
            importStore.ImporterName = importStoreRequest.ImporterName;
            importStore.Supplier = importStoreRequest.Supplier;
            importStore.ImportDate = importStoreRequest.ImportDate;
            importStore.CreatedDate = DateTime.Now;
            importStore.Total = importStoreRequest.Total;

            List<ImportStoreDetail> importStoreDetails = new List<ImportStoreDetail>();

            foreach (ProductItem item in importStoreRequest.ListProducts)
            {
                importStoreDetails.Add(new ImportStoreDetail
                {
                    Id = Guid.NewGuid(),
                    ImportStoreId = importStore.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    ImportPrice = item.Price,
                });

                var product = _productRepository.GetById(item.ProductId);
                product.Number += item.Quantity;
                _productRepository.Update(product);
            }

            await _importStoreRepository.CreateImportStore(importStore, importStoreDetails);
            return Json(new { importStore });
        }

        public IActionResult Details(Guid id)
        {
            var importStore = _importStoreRepository.GetQueryable()
                .Include(s => s.ImportStoreDetails).ThenInclude(s => s.Product)
                .SingleOrDefault(s => s.Id == id);
            if (importStore == null) return View("NotFound");
            return View(importStore);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var exportStore = _importStoreRepository.GetQueryable()
               .Include(s => s.ImportStoreDetails).ThenInclude(s => s.Product)
               .SingleOrDefault(s => s.Id == id);

            ViewBag.ProductsList = new SelectList(_productRepository.GetAll().ToList(), "Id", "ProductName");
            if (exportStore == null) return View("NotFound");
            return View(exportStore);
        }
        [HttpPost]
        public async Task<IActionResult> EditImportStore([FromBody] ImportStoreRequest importStoreRequest)
        {
            ImportStore importStore = new ImportStore();
            importStore.Id = (Guid)importStoreRequest.Id;
            importStore.ImporterName = importStoreRequest.ImporterName;
            importStore.Supplier = importStoreRequest.Supplier;
            importStore.ImportDate = importStoreRequest.ImportDate;
            importStore.CreatedDate = DateTime.Now;
            importStore.Total = importStoreRequest.Total;

            List<ImportStoreDetail> importStoreDetails = new List<ImportStoreDetail>();

            foreach (ProductItem item in importStoreRequest.ListProducts)
            {
                importStoreDetails.Add(new ImportStoreDetail
                {
                    Id = (Guid)(item.Id == null ? Guid.NewGuid() : item.Id),
                    ImportStoreId = importStore.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    ImportPrice = item.Price,
                });
            }

            await _importStoreRepository.UpdateImportStore(importStore, importStoreDetails);

            return Json(new { importStore, importStoreDetails });
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            var importStore = _importStoreRepository.GetById(id);
            if (importStore == null) return View("NotFound");
            return View(importStore);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var importStore = _importStoreRepository.GetById(id);
            if (importStore == null) return View("NotFound");

            _importStoreRepository.DeleteImportStore(importStore);

            _importStoreRepository.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
