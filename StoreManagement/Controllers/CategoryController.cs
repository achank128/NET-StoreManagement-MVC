using Microsoft.AspNetCore.Mvc;
using StoreManagement.Models;
using StoreManagement.Repositories.CategoryRepository;
using X.PagedList;

namespace StoreManagement.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public IActionResult Index(int? page, string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            if (page == null) page = 1;
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            IQueryable<Category> exportStores = _categoryRepository.GetQueryable();
            if (!String.IsNullOrEmpty(searchString))
            {
                exportStores = exportStores.Where(s =>
                s.CategoryName.Contains(searchString)
                );
            }
            exportStores = exportStores.OrderByDescending(s => s.CreatedDate);
            return View(exportStores.ToPagedList(pageNumber, pageSize));
            return View();
        }
    }
}
