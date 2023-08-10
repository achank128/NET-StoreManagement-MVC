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
            var products = _productRepository.GetQueryable().Include(s => s.Category).ToList();
            return Ok(new { data = products });
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
