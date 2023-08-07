using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;

namespace StoreManagement.Controllers
{
    public class ProductController : Controller
    {
        private readonly StoreManagementContext _context;

        public ProductController(StoreManagementContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("ProductName,Manufacturer,Description,Price,Number")] Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            product.Id = Guid.NewGuid();
            _context.Products.Add(product);
           await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var product = await _context.Products.SingleOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return View("NotFound");
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var product = await _context.Products.SingleOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return View("NotFound");
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ProductName,Manufacturer,Description,Price,Number")] Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _context.Products.SingleOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return View("NotFound");
            }
            return View(product);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var product = await _context.Products.SingleOrDefaultAsync(x => x.Id == id);
            if (product == null) return View("NotFound");

            var exportStore = await _context.ExportStores.Where(x => x.ProductId == product.Id).ToListAsync();
            _context.ExportStores.RemoveRange(exportStore);

            var importStore = await _context.ImportStores.Where(x => x.ProductId == product.Id).ToListAsync();
            _context.ImportStores.RemoveRange(importStore);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
