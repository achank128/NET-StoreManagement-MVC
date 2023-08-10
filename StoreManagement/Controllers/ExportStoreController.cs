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
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var importStore = _exportStoreRepository.GetQueryable().OrderByDescending(s => s.CreatedDate).ToList();
            return Ok(new { data = importStore });
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

            var isSuccess = await _exportStoreRepository.CreateExportStore(exportStore, exportStoreDetails);
            return Json(new { isSuccess });
        }

        public IActionResult Details(Guid id)
        {
            var exportStore = _exportStoreRepository.GetQueryable()
                .Include(s => s.ExportStoreDetails).ThenInclude(s => s.Product)
                .SingleOrDefault(s => s.Id == id);
            if (exportStore == null) return NotFound();
            return Ok(new { data = exportStore });
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

            var isSuccess = await _exportStoreRepository.UpdateExportStore(exportStore, exportStoreDetails);

            return Ok(new { isSuccess });
        }


        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var exportStore = _exportStoreRepository.GetById(id);
            if (exportStore == null) return NotFound();
            var isSuccess = await _exportStoreRepository.DeleteExportStore(exportStore);
            return Ok(new { isSuccess });
        }
    }
}
