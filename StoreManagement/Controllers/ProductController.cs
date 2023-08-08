using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;
using StoreManagement.Repositories.CategoryRepository;
using StoreManagement.Repositories.ExportStoreRepository;
using StoreManagement.Repositories.ImportStoreRepository;
using StoreManagement.Repositories.ProductRepository;

namespace StoreManagement.Controllers
{
    public class ProductController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IImportStoreRepository _importStoreRepository;
        private readonly IExportStoreRepository _exportStoreRepository;

        public ProductController(
            ICategoryRepository categoryRepository,
            IProductRepository productRepository, 
            IImportStoreRepository importStoreRepository,
            IExportStoreRepository exportStoreRepository
            )
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _importStoreRepository = importStoreRepository;
            _exportStoreRepository = exportStoreRepository;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllProducts();
            return View(products);
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

            //var exportStore = await _exportStoreRepository.GetBy(x => x.ProductId == product.Id).ToListAsync();
            //_exportStoreRepository.DeleteRange(exportStore);

            //var importStore = await _importStoreRepository.GetBy(x => x.ProductId == product.Id).ToListAsync();
            //_importStoreRepository.DeleteRange(importStore);

            _productRepository.Delete(product);
            _productRepository.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
