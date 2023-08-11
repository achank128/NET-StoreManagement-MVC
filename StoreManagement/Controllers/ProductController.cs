using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;
using StoreManagement.Repositories.CategoryRepository;
using StoreManagement.Repositories.ProductRepository;
using StoreManagement.Repositories.UnitRepository;
using X.PagedList;

namespace StoreManagement.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(
            IProductRepository productRepository,
            IUnitRepository unitRepository,
            ICategoryRepository categoryRepository
            )
        {
            _productRepository = productRepository;
            _unitRepository = unitRepository;
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.UnitsList = new SelectList(_unitRepository.GetAll().ToList(), "Id", "UnitName");
            ViewBag.CategoriesList = new SelectList(_categoryRepository.GetAll().ToList(), "Id", "CategoryName");
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
            IQueryable<Product> products = _productRepository.GetQueryable().Include(p => p.Category).Include(p => p.UnitNavigation);

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                products = products.Where(m => m.ProductName.Contains(searchValue));
            }

            //total number of rows count     
            recordsTotal = products.Count();
            //Paging     
            var data = products.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = _productRepository.GetQueryable().Where(s => s.Status == true).ToList() });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            product.Id = Guid.NewGuid();
            product.Status = true;
            _productRepository.Add(product);
            return Json(new { isSuccess = _productRepository.Save() });
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var product = _productRepository.GetById<Guid>(id);
            if (product == null) return NotFound();
            return Ok(new { data = product });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            var productUpdate = _productRepository.GetById<Guid>(product.Id);
            if (product == null) return NotFound();

            productUpdate.ProductCode = product.ProductCode;
            productUpdate.ProductName = product.ProductName;
            productUpdate.Manufacturer = product.Manufacturer;
            productUpdate.CategoryId = product.CategoryId;
            productUpdate.Description = product.Description;
            productUpdate.Unit = product.Unit;
            productUpdate.Price = product.Price;
            productUpdate.Number = product.Number;
            _productRepository.Update(productUpdate);
            return Ok(new { isSuccess = _productRepository.Save() });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute]Guid id, [FromQuery] string status )
        {
            var product = _productRepository.GetById<Guid>(id);
            if (product == null) return NotFound();
            product.Status = status == "1" ? true : false;
            _productRepository.Update(product);
            return Ok(new { isSuccess = _productRepository.Save(), status });

        }


       
    }
}
