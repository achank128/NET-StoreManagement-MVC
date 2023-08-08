using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;
using StoreManagement.Repositories.ImportStoreRepository;
using StoreManagement.Repositories.ProductRepository;

namespace StoreManagement.Controllers
{
    public class ImportStoreController : Controller
    {
        private readonly IImportStoreRepository _importStoreRepository;
        private readonly IProductRepository _productRepository;

        public ImportStoreController(
            IImportStoreRepository importStoreRepository,
            IProductRepository productRepository
            )
        {
            _importStoreRepository = importStoreRepository;
            _productRepository = productRepository;
        }
        public IActionResult Index()
        {
            var importStores = _importStoreRepository.GetQueryable()
                .ToList()
                .OrderByDescending(s => s.CreatedDate);
            return View(importStores);
        }

        //public async Task<IActionResult> Create()
        //{
        //    ViewBag.Products = new SelectList(_productRepository.GetAll().ToList(), "Id", "ProductName");
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create([Bind("ImporterName,ImportDate,ProductId,Quantity,Total")] ImportStore importStore)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(importStore);
        //    }

        //    var product = _productRepository.GetById(importStore.ProductId);
        //    product.Number += importStore.Quantity;
        //    _productRepository.Update(product);

        //    importStore.Id = Guid.NewGuid();
        //    importStore.Total = product.Price * importStore.Quantity;
        //    _importStoreRepository.Add(importStore);

        //    _importStoreRepository.Save();
        //    return RedirectToAction(nameof(Index));
        //}

        //public IActionResult Details(Guid id)
        //{
        //    var importStore = _importStoreRepository.GetQueryable().Include(s => s.Product).SingleOrDefault(s => s.Id == id);
        //    if (importStore == null) return View("NotFound");
        //    return View(importStore);
        //}

        //public async Task<IActionResult> Edit(Guid id)
        //{
        //    var importStore = _importStoreRepository.GetQueryable().Include(s => s.Product).SingleOrDefault(s => s.Id == id);
        //    if (importStore == null) return View("NotFound");
        //    return View(importStore);
        //}
        //[HttpPost]
        //public async Task<IActionResult> Edit(Guid id, [Bind("Id,ImporterName,ImportDate,ProductId,Quantity,Total")] ImportStore importStoreUpdate)
        //{
        //    var importStore = _importStoreRepository.GetById(id);
        //    var product = _productRepository.GetById(importStore.ProductId);

        //    if (importStore == null) return View("NotFound");

        //    if (!ModelState.IsValid)
        //    {
        //        return View(importStore);
        //    }
        //    //Update Product
        //    product.Number -= importStore.Quantity;
        //    product.Number += importStoreUpdate.Quantity;
        //    if (product.Number < 0)
        //    {
        //        ViewData["error"] = "Số lượng nhập quá giới hạn";
        //        return View(importStore);
        //    }
        //    _productRepository.Update(product);

        //    //Update Store
        //    importStore.ImporterName = importStoreUpdate.ImporterName;
        //    importStore.ImportDate = importStoreUpdate.ImportDate;
        //    importStore.Quantity = importStoreUpdate.Quantity;
        //    importStore.Total = product.Price * importStoreUpdate.Quantity;
        //    _importStoreRepository.Update(importStore);

        //    _importStoreRepository.Save();
        //    return RedirectToAction(nameof(Index));
        //}


        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var importStore = _importStoreRepository.GetById(id);
        //    if (importStore == null) return View("NotFound");
        //    return View(importStore);
        //}
        //[HttpPost, ActionName("Delete")]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    var importStore = _importStoreRepository.GetById(id);
        //    var product = _productRepository.GetById(importStore.ProductId);

        //    if (importStore == null) return View("NotFound");

        //    product.Number -= importStore.Quantity;
        //    if (product.Number < 0)
        //    {
        //        ViewData["error"] = "Số lượng nhập quá giới hạn";
        //        return View(importStore);
        //    }
        //    _productRepository.Update(product);

        //    _importStoreRepository.Delete(importStore);
        //    _importStoreRepository.Save();

        //    return RedirectToAction(nameof(Index));
        //}
    }
}
