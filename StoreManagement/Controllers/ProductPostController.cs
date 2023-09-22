using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;
using StoreManagement.Repositories.ProductPostRepository;
using StoreManagement.Repositories.ProductRepository;
using StoreManagement.Repositories.UserRepository;

namespace StoreManagement.Controllers
{
    public class ProductPostController : Controller
    {
        private IWebHostEnvironment Environment;
        private readonly IProductPostRepository _productPostRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public ProductPostController(
            IWebHostEnvironment environment,
            IProductPostRepository productPostRepository,
            IProductRepository productRepository,
            IUserRepository userRepository
            )
        {
            Environment = environment;
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

        public IActionResult Upsert(Guid? id)
        {
            ViewBag.ProductsList = new SelectList(_productRepository.GetAll().ToList(), "Id", "ProductName");
            ViewBag.UsersList = new SelectList(_userRepository.GetAll().ToList(), "Id", "FullName");
            if (HttpContext.Session.GetString("idUser") != null)
            {
                if (id == null)
                {
                    return View();
                }
                var productPost = _productPostRepository.GetById(id);
                if (productPost == null)
                {
                    return NotFound();
                }
                return View(productPost);
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
        public async Task<IActionResult> Create(ProductPostRequest productPostRequest)
        {
            var fileNames = new List<string>();
            if (productPostRequest.UploadFiles != null)
            {
                foreach (var file in productPostRequest.UploadFiles)
                {
                    if (file.Length > 0)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        string extension = Path.GetExtension(file.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                        string path = Path.Combine(this.Environment.WebRootPath, "Images");
                        string filePath = Path.Combine(path, fileName);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        //filePaths.Add(filePath);
                        fileNames.Add(fileName);
                    }
                }
            }

            string CoverImg = fileNames.FirstOrDefault();

            ProductPost productPost = new ProductPost
            {
                Id = Guid.NewGuid(),
                ProductId = productPostRequest.ProductId,
                AuthorId = productPostRequest.AuthorId,
                CoverImg = CoverImg != null ? CoverImg : productPostRequest.CoverImg,
                Content = productPostRequest.Content,
            };
            _productPostRepository.Add(productPost);
            return Ok(new { isSuccess = _productPostRepository.Save() });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductPostRequest productPostRequest)
        {
            var productPostUpdate = _productPostRepository.GetById<Guid>(productPostRequest.Id);
            if (productPostUpdate == null) return NotFound();

            var fileNames = new List<string>();

            if (productPostRequest.UploadFiles != null)
            {
                foreach (var file in productPostRequest.UploadFiles)
                {
                    if (file.Length > 0)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        string extension = Path.GetExtension(file.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                        string path = Path.Combine(this.Environment.WebRootPath, "Images");
                        string filePath = Path.Combine(path, fileName);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        //filePaths.Add(filePath);
                        fileNames.Add(fileName);
                    }
                }
            }

            string CoverImg = fileNames.FirstOrDefault();

            productPostUpdate.ProductId = productPostRequest.ProductId;
            productPostUpdate.AuthorId = productPostRequest.AuthorId;
            productPostUpdate.Content = productPostRequest.Content;
            productPostUpdate.CoverImg = CoverImg != null ? CoverImg : productPostRequest.CoverImg;
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
