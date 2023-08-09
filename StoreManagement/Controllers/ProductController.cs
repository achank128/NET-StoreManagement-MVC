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

            return View(products.ToPagedList(pageNumber, pageSize));
        }

        public IActionResult Create()
        {
            ViewBag.CategoriesList = new SelectList(_categoryRepository.GetAll().ToList(), "Id", "CategoryName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create( Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            product.Id = Guid.NewGuid();
            _productRepository.Add(product);
            _productRepository.Save();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(Guid id)
        {
            ViewBag.CategoriesList = new SelectList(_categoryRepository.GetAll().ToList(), "Id", "CategoryName");
            var product = _productRepository.GetById<Guid>(id);
            if (product == null)
            {
                return View("NotFound");
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            ViewBag.CategoriesList = new SelectList(_categoryRepository.GetAll().ToList(), "Id", "CategoryName");
            var product = _productRepository.GetById<Guid>(id);
            if (product == null)
            {
                return View("NotFound");
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            _productRepository.Update(product);
            _productRepository.Save();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var product = _productRepository.GetById<Guid>(id);
            if (product == null)
            {
                return View("NotFound");
            }
            return View(product);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var product = _productRepository.GetById<Guid>(id);
            if (product == null) return View("NotFound");

            var exportStore = await _exportStoreDetailRepository.GetBy(x => x.ProductId == product.Id).ToListAsync();
            _exportStoreDetailRepository.DeleteRange(exportStore);

            var importStore = await _importStoreDetailRepository.GetBy(x => x.ProductId == product.Id).ToListAsync();
            _importStoreDetailRepository.DeleteRange(importStore);

            _productRepository.Delete(product);
            _productRepository.Save();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = _productRepository.GetAll().ToList() });
        }
    }
}
