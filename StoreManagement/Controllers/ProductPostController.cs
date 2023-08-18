using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;
using StoreManagement.Repositories.CategoryRepository;
using StoreManagement.Repositories.ProductPostRepository;
using StoreManagement.Repositories.ProductRepository;
using StoreManagement.Repositories.UserRepository;

namespace StoreManagement.Controllers
{
    public class ProductPostController : Controller
    {
        private readonly IProductPostRepository _productPostRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public ProductPostController(IProductPostRepository productPostRepository, IProductRepository productRepository, IUserRepository userRepository)
        {
            _productPostRepository = productPostRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }
        public IActionResult Index()
        {
            ViewBag.ProductsList = new SelectList(_productRepository.GetAll().ToList(), "Id", "ProductName");
            ViewBag.UsersList = new SelectList(_userRepository.GetAll().ToList(), "Id", "FullName");
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
            IQueryable<ProductPost> productPosts = _productPostRepository.GetQueryable();

            productPosts = productPosts.Include(s => s.Product).Include(s => s.Author);

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                productPosts = productPosts.Where(m => m.Content.Contains(searchValue));
            }

            //total number of rows count     
            recordsTotal = productPosts.Count();
            //Paging     
            var data = productPosts.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = _productPostRepository.GetAll().ToList() });
        }

        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] Guid id)
        {
            var productPost = _productPostRepository.GetById<Guid>(id);
            if (productPost == null) return NotFound();
            return Json(new { data = productPost });

        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductPost productPost)
        {
            productPost.Id = Guid.NewGuid();
            _productPostRepository.Add(productPost);
            return Json(new { isSuccess = _productPostRepository.Save() });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductPost productPost)
        {
            var productPostUpdate = _productPostRepository.GetById<Guid>(productPost.Id);
            if (productPostUpdate == null) return NotFound();

            productPostUpdate.ProductId = productPost.ProductId;
            productPostUpdate.AuthorId = productPost.AuthorId;
            productPostUpdate.Content = productPost.Content;
            productPostUpdate.CoverImg = productPost.CoverImg;
            _productPostRepository.Update(productPostUpdate);
            return Json(new { isSuccess = _productPostRepository.Save() });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var productPost = _productPostRepository.GetById<Guid>(id);
            if (productPost == null) return NotFound();
            _productPostRepository.Delete(productPost);
            return Json(new { isSuccess = _productPostRepository.Save() });
        }
    }
}
