using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;
using StoreManagement.Models.Request;
using StoreManagement.Repositories.CustomerRepository;
using StoreManagement.Repositories.ExportStoreRepository;
using StoreManagement.Repositories.ProductRepository;
using StoreManagement.Repositories.UserRepository;
using X.PagedList;

namespace StoreManagement.Controllers
{
    public class ExportStoreController : Controller
    {
        private readonly IExportStoreRepository _exportStoreRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly INotyfService _notyf;
        public ExportStoreController(
            IExportStoreRepository exportStoreRepository,
            IProductRepository productRepository,
            INotyfService notyf,
            IUserRepository userRepository,
            ICustomerRepository customerRepository)
        {
            _exportStoreRepository = exportStoreRepository;
            _productRepository = productRepository;
            _notyf = _notyf;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
        }
        public IActionResult Index()
        {
            ViewBag.CustomersList = new SelectList(_customerRepository.GetAll().ToList(), "Id", "CustomerName");
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
            IQueryable<ExportStore> exportStores = _exportStoreRepository.GetQueryable().OrderByDescending(s => s.CreatedDate);

            exportStores = exportStores.Include(s => s.Customer).Include(s => s.Exporter);


            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
               // exportStores = exportStores.Where(m => m.ExporterName.Contains(searchValue) || m.Customer.Contains(searchValue));
            }

            //total number of rows count     
            recordsTotal = exportStores.Count();
            //Paging     
            var data = exportStores.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
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
            exportStore.ExporterId = exportStoreRequest.ExporterId;
            exportStore.CustomerId = exportStoreRequest.CustomerId;
            exportStore.ExportDate = exportStoreRequest.ExportDate;
            exportStore.CreatedDate = DateTime.Now;
            exportStore.SubTotal = exportStoreRequest.SubTotal;
            exportStore.Discount = exportStoreRequest.Discount;
            exportStore.Note = exportStoreRequest.Note;
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
            exportStore.ExporterId = exportStoreRequest.ExporterId;
            exportStore.CustomerId = exportStoreRequest.CustomerId;
            exportStore.ExportDate = exportStoreRequest.ExportDate;
            exportStore.CreatedDate = DateTime.Now;
            exportStore.SubTotal = exportStoreRequest.SubTotal;
            exportStore.Discount = exportStoreRequest.Discount;
            exportStore.Note = exportStoreRequest.Note;
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
