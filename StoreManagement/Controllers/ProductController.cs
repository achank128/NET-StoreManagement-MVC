using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;
using StoreManagement.Repositories.CategoryRepository;
using StoreManagement.Repositories.ProductRepository;
using X.PagedList;

namespace StoreManagement.Controllers
{
    public class ProductController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        public ProductController(
            ICategoryRepository categoryRepository,
            IProductRepository productRepository
            )
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }
        public async Task<IActionResult> Index()
        {
            return View();
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
            IQueryable<Product> products = _productRepository.GetQueryable().Include(p => p.Category);

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
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = _productRepository.GetById<Guid>(id);
            if (product == null) return NotFound();
            product.Status = false;
            _productRepository.Update(product);
            return Ok(new { isSuccess = _productRepository.Save() });

        }


       
    }
}
