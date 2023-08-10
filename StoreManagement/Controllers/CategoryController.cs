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

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            category.Id = Guid.NewGuid();
            _categoryRepository.Add(category);
            return Json(new { isSuccess = _categoryRepository.Save() });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            var categoryUpdate = _categoryRepository.GetById<Guid>(category.Id);
            if (categoryUpdate == null) return NotFound();

            categoryUpdate.CategoryName = category.CategoryName;
            categoryUpdate.Description = category.Description;
            _categoryRepository.Update(categoryUpdate);
            return Json(new { isSuccess = _categoryRepository.Save() });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var category = _categoryRepository.GetById<Guid>(id);
            if (category == null) return NotFound();
            _categoryRepository.Delete(category);
            return Json(new { isSuccess = _categoryRepository.Save() });
        }

        [HttpPost]
        public async Task<IActionResult> Details([FromRoute] Guid id)
        {
            var category = _categoryRepository.GetById<Guid>(id);
            if (category == null) return NotFound();
            return Json(new { data = category });

        }
    }
}
