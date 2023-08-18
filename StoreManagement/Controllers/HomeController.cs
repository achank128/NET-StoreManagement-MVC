using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using StoreManagement.Models;
using StoreManagement.Services;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace StoreManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INotyfService _notyf;
        private readonly StoreManagementContext _context;
        private LanguageService _localization;

        public HomeController(ILogger<HomeController> logger, INotyfService notyf, StoreManagementContext context, LanguageService localization)
        {
            _logger = logger;
            _notyf = notyf;
            _context = context;
            _localization = localization;
        }

        public IActionResult Index()
        {
            ViewBag.WelcomeMessage = _localization.Getkey("common_welcome");
            if (HttpContext.Session.GetString("idUser") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public IActionResult ChangeLanguage(string culture)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)), new CookieOptions()
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult Register()
        {
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                var check = _context.Users.FirstOrDefault(s => s.Email == user.Email);
                if (check == null)
                {
                    user.Id = Guid.NewGuid();
                    user.Password = GetMD5(user.Password);
                    //_context.Configuration.ValidateOnSaveEnabled = false;
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    _notyf.Success("Đăng ký tài khoản thành công.");
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Email đã tồn tại.";
                    _notyf.Error("Đăng nhập thất bại vui lòng kiểm tra lại thông tin");
                    return View();
                }


            }
            return View();


        }

        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var f_password = GetMD5(password);
                var data = _context.Users.Where(s => s.Email.Equals(email) && s.Password.Equals(f_password)).ToList();
                if (data.Count() > 0)
                {
                    HttpContext.Session.SetString("FullName", data.FirstOrDefault().FullName);
                    HttpContext.Session.SetString("Email", data.FirstOrDefault().Email);
                    HttpContext.Session.SetString("idUser", data.FirstOrDefault().Id.ToString());
                    _notyf.Success("Đăng nhập thành công.");
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Vui lòng kiểm tra lại thông tin đăng nhập.";
                    _notyf.Error("Đăng nhập thất bại vui lòng kiểm tra lại thông tin");
                    return RedirectToAction("Login");
                }
            }
            return View();
        }


        //Logout
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();//remove session
            return RedirectToAction("Login");
        }



        //create a string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }

    
}
}