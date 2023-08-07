using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;

namespace StoreManagement.Controllers
{
    public class ImportStoreController : Controller
    {
        private readonly StoreManagementContext _context;

        public ImportStoreController(StoreManagementContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var importStores = _context.ImportStores.Include(s => s.Product).ToList().OrderByDescending(s => s.ImportDate);
            return View(importStores);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "Id", "ProductName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("ImporterName,ImportDate,ProductId,Quantity,Total")] ImportStore importStore)
        {
            if (!ModelState.IsValid)
            {
                return View(importStore);
            }
            importStore.Id = Guid.NewGuid();

            var product = await _context.Products.SingleOrDefaultAsync(x => x.Id == importStore.ProductId);
            product.Number += importStore.Quantity;
            _context.Update(product);

            importStore.Total = product.Price * importStore.Quantity;
            _context.ImportStores.Add(importStore);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(Guid id)
        {
            var importStore = _context.ImportStores.Include(s => s.Product).SingleOrDefault(x => x.Id == id);
            if (importStore == null) return View("NotFound");
            return View(importStore);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var importStore = _context.ImportStores.Include(s => s.Product).SingleOrDefault(x => x.Id == id);
            if (importStore == null) return View("NotFound");
            return View(importStore);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ImporterName,ImportDate,ProductId,Quantity,Total")] ImportStore importStoreUpdate)
        {
            var importStore = _context.ImportStores.Include(s => s.Product).SingleOrDefault(x => x.Id == id);
            var product = await _context.Products.SingleOrDefaultAsync(x => x.Id == importStore.ProductId);

            if (importStore == null) return View("NotFound");
            
            if (!ModelState.IsValid)
            {
                return View(importStore);
            }
            //Update Product
            product.Number -= importStore.Quantity;
            product.Number += importStoreUpdate.Quantity;
            if (product.Number < 0)
            {
                ViewData["error"] = "Số lượng nhập quá giới hạn";
                return View(importStore);
            }

            _context.Products.Update(product);

            //Update Store
            importStore.ImporterName = importStoreUpdate.ImporterName;
            importStore.ImportDate = importStoreUpdate.ImportDate;
            importStore.Quantity = importStoreUpdate.Quantity;
            importStore.Total = product.Price * importStoreUpdate.Quantity;
            _context.ImportStores.Update(importStore);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            var importStore = await _context.ImportStores.SingleOrDefaultAsync(x => x.Id == id);
            if (importStore == null) return View("NotFound");
            return View(importStore);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var importStore = await _context.ImportStores.SingleOrDefaultAsync(x => x.Id == id);
            if (importStore == null) return View("NotFound");

            var product = await _context.Products.SingleOrDefaultAsync(x => x.Id == importStore.ProductId);
            product.Number -= importStore.Quantity;
            if (product.Number < 0)
            {
                ViewData["error"] = "Số lượng nhập quá giới hạn";
                return View(importStore);
            }
            _context.Products.Update(product);

            _context.ImportStores.Remove(importStore);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
