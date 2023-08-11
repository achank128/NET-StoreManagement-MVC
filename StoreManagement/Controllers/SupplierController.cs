using Microsoft.AspNetCore.Mvc;
using StoreManagement.Models;
using StoreManagement.Repositories.SupplierRepository;

namespace StoreManagement.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierController(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public IActionResult Index()
        {
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
            IQueryable<Supplier> suppliers = _supplierRepository.GetQueryable();

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                suppliers = suppliers.Where(m => m.SupplierName.Contains(searchValue));
            }

            //total number of rows count     
            recordsTotal = suppliers.Count();
            //Paging     
            var data = suppliers.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = _supplierRepository.GetAll().ToList() });
        }

        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] Guid id)
        {
            var supplier = _supplierRepository.GetById<Guid>(id);
            if (supplier == null) return NotFound();
            return Json(new { data = supplier });

        }

        [HttpPost]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            supplier.Id = Guid.NewGuid();
            _supplierRepository.Add(supplier);
            return Json(new { isSuccess = _supplierRepository.Save() });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Supplier supplier)
        {
            var supplierUpdate = _supplierRepository.GetById<Guid>(supplier.Id);
            if (supplierUpdate == null) return NotFound();

            supplierUpdate.SupplierName = supplier.SupplierName;
            supplierUpdate.Address = supplier.Address;
            supplierUpdate.Phone = supplier.Phone;
            supplierUpdate.Description = supplier.Description;
            _supplierRepository.Update(supplierUpdate);
            return Json(new { isSuccess = _supplierRepository.Save() });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var supplier = _supplierRepository.GetById<Guid>(id);
            if (supplier == null) return NotFound();
            _supplierRepository.Delete(supplier);
            return Json(new { isSuccess = _supplierRepository.Save() });
        }
    }
}
