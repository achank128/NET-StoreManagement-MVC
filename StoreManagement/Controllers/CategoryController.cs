using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;
using StoreManagement.Repositories.CategoryRepository;
using System.Linq;

namespace StoreManagement.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("idUser") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var draw = HttpContext.Request.Query["draw"].ToString();
            var start = HttpContext.Request.Query["start"].ToString();
            var length = HttpContext.Request.Query["length"].ToString();
            var sortColumn = HttpContext.Request.Query["columns[" + HttpContext.Request.Query["order[0][column]"].ToString() + "][name]"].ToString();
            var sortColumnDir = HttpContext.Request.Query["order[0][dir]"].ToString();
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();

            //Paging Size 
            int pageSize = length != null ? Convert.ToInt32(length) : 1;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            // Getting all Customer data    
            IQueryable<Category> categories = _categoryRepository.GetQueryable();

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                categories = categories.Where(m => m.CategoryName.Contains(searchValue));
            }

            //total number of rows count     
            recordsTotal = categories.Count();
            //Paging     
            var data = categories.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] Guid id)
        {
            var category = _categoryRepository.GetById<Guid>(id);
            if (category == null) return NotFound();
            return Json(new { data = category });

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
    }
}
