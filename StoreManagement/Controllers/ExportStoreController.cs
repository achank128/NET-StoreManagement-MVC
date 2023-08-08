using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;
using StoreManagement.Repositories.ExportStoreRepository;
using StoreManagement.Repositories.ImportStoreRepository;
using StoreManagement.Repositories.ProductRepository;
using System.Reflection.Metadata;

namespace StoreManagement.Controllers
{
    public class ExportStoreController : Controller
    {
        private readonly IExportStoreRepository _exportStoreRepository;
        private readonly IProductRepository _productRepository;

        public ExportStoreController(
            IExportStoreRepository exportStoreRepository,
            IProductRepository productRepository
            )
        {
            _exportStoreRepository = exportStoreRepository;
            _productRepository = productRepository;
        }
        public IActionResult Index()
        {
            var exportStores = _exportStoreRepository.GetQueryable()
                .ToList()
                .OrderByDescending(s => s.CreatedDate);
            return View(exportStores);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Products = new SelectList(_productRepository.GetAll().ToList(), "Id", "ProductName");
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Create([Bind("ExporterName,ExporterDate,ProductId,Quantity,Total")] ExportStore exportStore)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(exportStore);
        //    }

        //    var product = _productRepository.GetById(exportStore.ProductId);
        //    product.Number -= exportStore.Quantity;
        //    if(product.Number < 0)
        //    {
        //        ViewData["error"] = "Số lượng xuất quá giới hạn";
        //        return View(exportStore);
        //    }
        //    _productRepository.Update(product);


        //    exportStore.Id = Guid.NewGuid();
        //    exportStore.Total = product.Price * exportStore.Quantity;
        //    _exportStoreRepository.Add(exportStore);

        //    _exportStoreRepository.Save();
        //    return RedirectToAction(nameof(Index));
        //}

        public IActionResult Details(Guid id)
        {
            var exportStore = _exportStoreRepository.GetQueryable().SingleOrDefault(s => s.Id == id);
            if (exportStore == null) return View("NotFound");

            return View(exportStore);
        }


        //public async Task<IActionResult> Edit(Guid id)
        //{
        //    var exportStore = _exportStoreRepository.GetQueryable().Include(s => s.Product).SingleOrDefault(s => s.Id == id);
        //    if (exportStore == null) return View("NotFound");
        //    return View(exportStore);
        //}
        //[HttpPost]
        //public async Task<IActionResult> Edit(Guid id, [Bind("Id,ExporterName,ExporterDate,ProductId,Quantity,Total")] ExportStore exportStoreUpdate)
        //{
        //    var exportStore = _exportStoreRepository.GetById(id);
        //    var product = _productRepository.GetById(exportStore.ProductId);

        //    if (exportStore == null) return View("NotFound");

        //    if (!ModelState.IsValid)
        //    {
        //        return View(exportStore);
        //    }

        //    //Update Product
        //    product.Number += exportStore.Quantity;
        //    product.Number -= exportStoreUpdate.Quantity;
        //    if (product.Number < 0)
        //    {
        //        ViewData["error"] = "Số lượng xuất quá giới hạn";
        //        return View(exportStore);
        //    }
        //    _productRepository.Update(product);

        //    //Update Store
        //    exportStore.ExporterName = exportStoreUpdate.ExporterName;
        //    exportStore.ExporterDate = exportStoreUpdate.ExporterDate;
        //    exportStore.Quantity = exportStoreUpdate.Quantity;
        //    exportStore.Total = product.Price * exportStoreUpdate.Quantity;
        //    _exportStoreRepository.Update(exportStore);

        //    _exportStoreRepository.Save();
        //    return RedirectToAction(nameof(Index));
        //}


        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var importStore = _exportStoreRepository.GetById(id);
        //    if (importStore == null) return View("NotFound");
        //    return View(importStore);
        //}
        //[HttpPost, ActionName("Delete")]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    var exportStore = _exportStoreRepository.GetById(id);
        //    if (exportStore == null) return View("NotFound");

        //    var product = _productRepository.GetById(exportStore.ProductId);
        //    product.Number += exportStore.Quantity;
        //    _productRepository.Update(product);

        //    _exportStoreRepository.Delete(exportStore);

        //    _exportStoreRepository.Save();
        //    return RedirectToAction(nameof(Index));
        //}
    }
}
