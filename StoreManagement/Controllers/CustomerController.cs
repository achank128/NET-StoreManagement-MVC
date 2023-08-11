using Microsoft.AspNetCore.Mvc;
using StoreManagement.Models;
using StoreManagement.Repositories.CustomerRepository;
using StoreManagement.Repositories.SupplierRepository;

namespace StoreManagement.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
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
            IQueryable<Customer> customers = _customerRepository.GetQueryable();

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                customers = customers.Where(m => m.CustomerName.Contains(searchValue));
            }

            //total number of rows count     
            recordsTotal = customers.Count();
            //Paging     
            var data = customers.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = _customerRepository.GetAll().ToList() });
        }

        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] Guid id)
        {
            var customer = _customerRepository.GetById<Guid>(id);
            if (customer == null) return NotFound();
            return Json(new { data = customer });

        }

        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            customer.Id = Guid.NewGuid();
            _customerRepository.Add(customer);
            return Json(new { isSuccess = _customerRepository.Save() });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Customer customer)
        {
            var customerUpdate = _customerRepository.GetById<Guid>(customer.Id);
            if (customerUpdate == null) return NotFound();

            customerUpdate.CustomerName = customer.CustomerName;
            customerUpdate.Address = customer.Address;
            customerUpdate.Phone = customer.Phone;
            customerUpdate.Description = customer.Description;
            _customerRepository.Update(customerUpdate);
            return Json(new { isSuccess = _customerRepository.Save() });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var customer = _customerRepository.GetById<Guid>(id);
            if (customer == null) return NotFound();
            _customerRepository.Delete(customer);
            return Json(new { isSuccess = _customerRepository.Save() });
        }
    }
}
