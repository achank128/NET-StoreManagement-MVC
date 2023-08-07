using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;
using System.Reflection.Metadata;

namespace StoreManagement.Controllers
{
    public class ExportStoreController : Controller
    {
        private readonly StoreManagementContext _context;

        public ExportStoreController(StoreManagementContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var exportStores = _context.ExportStores.Include(s => s.Product).ToList().OrderByDescending(s => s.ExporterDate);
            return View(exportStores);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "Id", "ProductName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("ExporterName,ExporterDate,ProductId,Quantity,Total")] ExportStore exportStore)
        {
            if (!ModelState.IsValid)
            {
                return View(exportStore);
            }
            exportStore.Id = Guid.NewGuid();

            var product = await _context.Products.SingleOrDefaultAsync(x => x.Id == exportStore.ProductId);
            product.Number -= exportStore.Quantity;

            if(product.Number < 0)
            {
                ViewData["error"] = "Số lượng xuất quá giới hạn";
                return View(exportStore);
            }

            _context.Update(product);

            exportStore.Total = product.Price * exportStore.Quantity;
            _context.ExportStores.Add(exportStore);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(Guid id)
        {
            var product = _context.ExportStores.Include(s => s.Product).SingleOrDefault(x => x.Id == id);
            if (product == null)
            {
                return View("NotFound");
            }
            return View(product);
        }


        public async Task<IActionResult> Edit(Guid id)
        {
            var importStore = _context.ExportStores.Include(s => s.Product).SingleOrDefault(x => x.Id == id);
            if (importStore == null) return View("NotFound");
            return View(importStore);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ExporterName,ExporterDate,ProductId,Quantity,Total")] ExportStore exportStoreUpdate)
        {
            var exportStore = _context.ExportStores.Include(s => s.Product).SingleOrDefault(x => x.Id == id);
            var product = await _context.Products.SingleOrDefaultAsync(x => x.Id == exportStore.ProductId);

            if (exportStore == null) return View("NotFound");

            if (!ModelState.IsValid)
            {
                return View(exportStore);
            }
            //Update Product
            product.Number += exportStore.Quantity;
            product.Number -= exportStoreUpdate.Quantity;

            if (product.Number < 0)
            {
                ViewData["error"] = "Số lượng xuất quá giới hạn";
                return View(exportStore);
            }

            _context.Products.Update(product);

            //Update Store
            exportStore.ExporterName = exportStoreUpdate.ExporterName;
            exportStore.ExporterDate = exportStoreUpdate.ExporterDate;
            exportStore.Quantity = exportStoreUpdate.Quantity;
            exportStore.Total = product.Price * exportStoreUpdate.Quantity;
            _context.ExportStores.Update(exportStore);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            var importStore = await _context.ExportStores.SingleOrDefaultAsync(x => x.Id == id);
            if (importStore == null) return View("NotFound");
            return View(importStore);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var exportStore = await _context.ExportStores.SingleOrDefaultAsync(x => x.Id == id);
            if (exportStore == null) return View("NotFound");

            var product = await _context.Products.SingleOrDefaultAsync(x => x.Id == exportStore.ProductId);
            product.Number += exportStore.Quantity;
            _context.Products.Update(product);

            _context.ExportStores.Remove(exportStore);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
