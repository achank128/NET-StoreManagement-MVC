using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;
using StoreManagement.Models.Request;
using StoreManagement.Repositories.ExportStoreRepository;
using StoreManagement.Repositories.ProductRepository;
using X.PagedList;

namespace StoreManagement.Controllers
{
    public class ExportStoreController : Controller
    {
        private readonly IExportStoreRepository _exportStoreRepository;
        private readonly IProductRepository _productRepository;
        private readonly INotyfService _notyf;
        public ExportStoreController(
            IExportStoreRepository exportStoreRepository,
            IProductRepository productRepository,
            INotyfService notyf
            )
        {
            _exportStoreRepository = exportStoreRepository;
            _productRepository = productRepository;
            _notyf = _notyf;
        }
        public IActionResult Index(int? page, string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            if (page == null) page = 1;
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            IQueryable<ExportStore> exportStores = _exportStoreRepository.GetQueryable();
            if (!String.IsNullOrEmpty(searchString))
            {
                exportStores = exportStores.Where(s =>
                s.ExporterName.Contains(searchString)
                || s.Customer.Contains(searchString)
                );
            }
            exportStores = exportStores.OrderByDescending(s => s.CreatedDate);
            return View(exportStores.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateExportStore([FromBody] ExportStoreRequest exportStoreRequest)
        {
            ExportStore exportStore = new ExportStore();
            exportStore.Id = Guid.NewGuid();
            exportStore.ExporterName = exportStoreRequest.ExporterName;
            exportStore.Customer = exportStoreRequest.Customer;
            exportStore.ExportDate = exportStoreRequest.ExportDate;
            exportStore.CreatedDate = DateTime.Now;
            exportStore.Total = exportStoreRequest.Total;

            List<ExportStoreDetail> exportStoreDetails = new List<ExportStoreDetail>();

            foreach (ProductItem item in exportStoreRequest.ListProducts)
            {
                exportStoreDetails.Add(new ExportStoreDetail
                {
                    Id = Guid.NewGuid(),
                    ExportStoreId = exportStore.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    ExportPrice = item.Price,
                });
            }

            await _exportStoreRepository.CreateExportStore(exportStore, exportStoreDetails);
            return Json(new { data = exportStore });
        }

        public IActionResult Details(Guid id)
        {
            var exportStore = _exportStoreRepository.GetQueryable()
                .Include(s => s.ExportStoreDetails).ThenInclude(s => s.Product)
                .SingleOrDefault(s => s.Id == id);
            if (exportStore == null) return View("NotFound");

            return View(exportStore);
        }


        public async Task<IActionResult> Edit(Guid id)
        {
            var exportStore = _exportStoreRepository.GetQueryable()
               .Include(s => s.ExportStoreDetails).ThenInclude(s => s.Product)
               .SingleOrDefault(s => s.Id == id);

            ViewBag.ProductsList = new SelectList(_productRepository.GetAll().ToList(), "Id", "ProductName");
            if (exportStore == null) return View("NotFound");
            return View(exportStore);
        }
        [HttpPost]
        public async Task<IActionResult> EditExportStore([FromBody] ExportStoreRequest exportStoreRequest)
        {
            ExportStore exportStore = new ExportStore();
            exportStore.Id = (Guid)exportStoreRequest.Id;
            exportStore.ExporterName = exportStoreRequest.ExporterName;
            exportStore.Customer = exportStoreRequest.Customer;
            exportStore.ExportDate = exportStoreRequest.ExportDate;
            exportStore.CreatedDate = DateTime.Now;
            exportStore.Total = exportStoreRequest.Total;

            List<ExportStoreDetail> exportStoreDetails = new List<ExportStoreDetail>();

            foreach (ProductItem item in exportStoreRequest.ListProducts)
            {
                exportStoreDetails.Add(new ExportStoreDetail
                {
                    Id = (Guid)(item.Id == null ? Guid.NewGuid() : item.Id),
                    ExportStoreId = exportStore.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    ExportPrice = item.Price,
                });
            }

            await _exportStoreRepository.UpdateExportStore(exportStore, exportStoreDetails);

            return Json(new { data = exportStore, exportStoreDetails });
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            var exportStore = _exportStoreRepository.GetById(id);
            if (exportStore == null) return View("NotFound");
            return View(exportStore);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var exportStore = _exportStoreRepository.GetById(id);
            if (exportStore == null) return View("NotFound");

            _exportStoreRepository.DeleteExportStore(exportStore);

            _exportStoreRepository.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
