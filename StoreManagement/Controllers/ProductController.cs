using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;
using StoreManagement.Repositories.CategoryRepository;
using StoreManagement.Repositories.ProductRepository;
using StoreManagement.Repositories.RepositoryBase;
using X.PagedList;

namespace StoreManagement.Controllers
{
    public class ProductController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IRepositoryBase<ImportStoreDetail> _importStoreDetailRepository;
        private readonly IRepositoryBase<ExportStoreDetail> _exportStoreDetailRepository;

        public ProductController(
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            IRepositoryBase<ImportStoreDetail> importStoreDetailRepository,
            IRepositoryBase<ExportStoreDetail> exportStoreDetailRepository
            )
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _importStoreDetailRepository = importStoreDetailRepository;
            _exportStoreDetailRepository = exportStoreDetailRepository;
        }
        public async Task<IActionResult> Index(int? page, string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            if (page == null) page = 1;
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            IQueryable<Product> products = _productRepository.GetQueryable().Include(p => p.Category);

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s =>
                s.ProductName.Contains(searchString)
                || s.ProductCode.Contains(searchString)
                || s.Manufacturer.Contains(searchString)
                || s.Category.CategoryName.Contains(searchString)
                );
            }

            ViewBag.CategoriesList = new SelectList(_categoryRepository.GetAll().ToList(), "Id", "CategoryName");
            return View(products.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            product.Id = Guid.NewGuid();
            product.Status = true;
            _productRepository.Add(product);
            return Json(new { isSuccess = _productRepository.Save() });
        }

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
            product.Status = !product.Status;
            _productRepository.Update(product);
            return Ok(new { isSuccess = _productRepository.Save() });

        }


        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var products = _productRepository.GetQueryable().Include(s => s.Category).ToList();
            return Json(new { data = products });
        }
    }
}
