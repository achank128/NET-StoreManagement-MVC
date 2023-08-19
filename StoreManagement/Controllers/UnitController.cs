using Microsoft.AspNetCore.Mvc;
using StoreManagement.Models;
using StoreManagement.Repositories.UnitRepository;

namespace StoreManagement.Controllers
{
    public class UnitController : Controller
    {
        private readonly IUnitRepository _unitRepository;

        public UnitController(IUnitRepository unitRepository)
        {
            _unitRepository = unitRepository;
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
            IQueryable<Unit> units = _unitRepository.GetQueryable();

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                units = units.Where(m => m.UnitName.Contains(searchValue));
            }

            //total number of rows count     
            recordsTotal = units.Count();
            //Paging     
            var data = units.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = _unitRepository.GetAll().ToList() });
        }

        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] string id)
        {
            var unit = _unitRepository.GetById<string>(id);
            if (unit == null) return NotFound();
            return Json(new { data = unit });

        }

        [HttpPost]
        public async Task<IActionResult> Create(Unit unit)
        {
            _unitRepository.Add(unit);
            return Json(new { isSuccess = _unitRepository.Save() });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Unit unit)
        {
            var unitUpdate = _unitRepository.GetById<Guid>(unit.Id);
            if (unitUpdate == null) return NotFound();

            unitUpdate.UnitName = unit.UnitName;
            unitUpdate.Description = unit.Description;
            _unitRepository.Update(unitUpdate);
            return Json(new { isSuccess = _unitRepository.Save() });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var unit = _unitRepository.GetById<string>(id);
            if (unit == null) return NotFound();
            _unitRepository.Delete(unit);
            return Json(new { isSuccess = _unitRepository.Save() });
        }
    }
}
