using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

            IQueryable<Category> categories = _categoryRepository.GetQueryable();
            if (!String.IsNullOrEmpty(searchString))
            {
                categories = categories.Where(s =>
                s.CategoryName.Contains(searchString)
                );
            }
            categories = categories.OrderByDescending(s => s.CreatedDate);
            return View(categories.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            return Json(new { data = _categoryRepository.GetAll().ToList() });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            category.Id = Guid.NewGuid();  
            _categoryRepository.Add(category);
            _categoryRepository.Save();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var category = _categoryRepository.GetById<Guid>(Guid.Parse(id));
            if (category == null) return View("NotFound");
            category.Id = Guid.NewGuid();
            _categoryRepository.Delete(category);
            _categoryRepository.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
