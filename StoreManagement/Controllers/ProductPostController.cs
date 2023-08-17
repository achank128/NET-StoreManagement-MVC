using Microsoft.AspNetCore.Mvc;

namespace StoreManagement.Controllers
{
    public class ProductPostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
