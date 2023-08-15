using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;
using StoreManagement.Models.Request;
using StoreManagement.Repositories.ImportStoreRepository;
using StoreManagement.Repositories.ProductRepository;
using StoreManagement.Repositories.SupplierRepository;
using StoreManagement.Repositories.UserRepository;
using X.PagedList;

namespace StoreManagement.Controllers
{
    public class ImportStoreController : Controller
    {
        private readonly IImportStoreRepository _importStoreRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotyfService _notyf;

        public ImportStoreController(
            IImportStoreRepository importStoreRepository,
            IProductRepository productRepository,
            ISupplierRepository supplierRepository,
            IUserRepository userRepository,
            INotyfService notyf
            )
        {
            _importStoreRepository = importStoreRepository;
            _productRepository = productRepository;
            _supplierRepository = supplierRepository;
            _userRepository = userRepository;
            _notyf = notyf;
        }
        public IActionResult Index()
        {
            ViewBag.SuppliersList = new SelectList(_supplierRepository.GetAll().ToList(), "Id", "SupplierName");
            ViewBag.UsersList = new SelectList(_userRepository.GetAll().ToList(), "Id", "FullName");
            if (HttpContext.Session.GetString("idUser") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var draw = HttpContext.Request.Query["draw"].ToString();
            var start = HttpContext.Request.Query["start"].ToString();
            var length = HttpContext.Request.Query["length"].ToString();
            var sortColumn = HttpContext.Request.Query["columns[" + HttpContext.Request.Query["order[0][column]"].ToString() + "][name]"].ToString();
            var sortColumnDir = HttpContext.Request.Query["order[0][dir]"].ToString();
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();

            //Paging Size 
            int pageSize = length != null ? Convert.ToInt32(length) : 1;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            // Getting all Customer data    
            IQueryable<ImportStore> importStores = _importStoreRepository.GetQueryable().OrderByDescending(s => s.CreatedDate);

            importStores = importStores.Include(s => s.Supplier).Include(s => s.Importer);

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                //importStores = importStores.Where(m => m.ImporterName.Contains(searchValue) || m.Supplier.Contains(searchValue));
            }

            //total number of rows count     
            recordsTotal = importStores.Count();
            //Paging     
            var data = importStores.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
     
        [HttpPost]
        public async Task<IActionResult> CreateImportStore([FromBody] ImportStoreRequest importStoreRequest)
        {
            ImportStore importStore = new ImportStore();
            importStore.Id = Guid.NewGuid();
            importStore.ImporterId = importStoreRequest.ImporterId;
            importStore.SupplierId = importStoreRequest.SupplierId;
            importStore.ImportDate = importStoreRequest.ImportDate;
            importStore.Note = importStoreRequest.Note;
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
            }

            var isSuccess = await _importStoreRepository.CreateImportStore(importStore, importStoreDetails);

            return Json(new { isSuccess });
        }

        [HttpPost]
        public async Task<IActionResult> Details(Guid id)
        {
            var exportStore = _importStoreRepository.GetQueryable()
               .Include(s => s.ImportStoreDetails).ThenInclude(s => s.Product)
               .SingleOrDefault(s => s.Id == id);
            if (exportStore == null) return NotFound();
            return Ok(new { data = exportStore });
        }


        [HttpPost]
        public async Task<IActionResult> EditImportStore([FromBody] ImportStoreRequest importStoreRequest)
        {
            ImportStore importStore = new ImportStore();
            importStore.Id = (Guid)importStoreRequest.Id;
            importStore.ImporterId = importStoreRequest.ImporterId;
            importStore.SupplierId = importStoreRequest.SupplierId;
            importStore.ImportDate = importStoreRequest.ImportDate;
            importStore.Note = importStoreRequest.Note;
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
            var isSuccess = await _importStoreRepository.UpdateImportStore(importStore, importStoreDetails);
            return Ok(new { isSuccess });
        }


        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var importStore = _importStoreRepository.GetById(id);
            if (importStore == null) return NotFound();
            var isSuccess = await _importStoreRepository.DeleteImportStore(importStore);
            return Ok(new { isSuccess });
        }
    }
}
