using Microsoft.AspNetCore.Mvc;
using System.Data.OleDb;
using System.Data;
using StoreManagement.Models;
using StoreManagement.Repositories.CategoryRepository;
using StoreManagement.Repositories.ProductRepository;
using StoreManagement.Repositories.UnitRepository;
using AspNetCoreHero.ToastNotification.Abstractions;
using StoreManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Drawing;


namespace StoreManagement.Controllers
{
    public class FileUploadController : Controller
    {
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;
        private IConfiguration Configuration;
        private readonly IProductRepository _productRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotyfService _notyf;
        private readonly LanguageService _localization;

        public FileUploadController(
            Microsoft.AspNetCore.Hosting.IHostingEnvironment environment,
            IConfiguration configuration,
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            IUnitRepository unitRepository,
            INotyfService notyf,
            LanguageService localization)
        {
            Environment = environment;
            Configuration = configuration;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _unitRepository = unitRepository;
            _notyf = notyf;
            _localization = localization;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Uploads(List<IFormFile> postedFiles)
        {
            long size = postedFiles.Sum(f => f.Length);

            var filePaths = new List<string>();
            foreach (var file in postedFiles)
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
                    filePaths.Add(filePath);
                }
            }
            return Ok(new { count = postedFiles.Count, size, filePaths });
        }

        [HttpPost]
        public async Task<IActionResult> UploadImg(IFormFile postedFile)
        {
            string fileName = Path.GetFileNameWithoutExtension(postedFile.FileName);
            string extension = Path.GetExtension(postedFile.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

            string path = Path.Combine(this.Environment.WebRootPath, "Images");
            string filePath = Path.Combine(path, fileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                postedFile.CopyTo(fileStream);
            }

            return Json(new
            {
                fileName,
                filePath
            });
        }

        [HttpPost]
        public async Task<IActionResult> Image(List<IFormFile> upload)
        {
            try
            {
                var fileNames = new List<string>();
                foreach (var file in upload)
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
                        fileNames.Add(fileName);
                    }
                }
                var baseUri = $"{Request.Scheme}://{Request.Host}/Images";
                return Ok(new { url = Path.Combine(baseUri, fileNames.First()) });
            }
            catch (Exception exception)
            {
                return Ok(new { error = new { message = exception.Message }});
            }
        }

        [HttpPost]
        public IActionResult ImportCategory(IFormFile postedFile)
        {
            try
            {
                if (postedFile != null)
                {
                    //Create a Folder.
                    string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    //Save the uploaded Excel file.
                    string fileName = Path.GetFileName(postedFile.FileName);
                    string filePath = Path.Combine(path, fileName);
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                    }

                    //Read the connection string for the Excel file.
                    string conString = this.Configuration.GetConnectionString("ExcelConString");
                    DataTable dt = new DataTable();
                    conString = string.Format(conString, filePath);

                    using (OleDbConnection connExcel = new OleDbConnection(conString))
                    {
                        using (OleDbCommand cmdExcel = new OleDbCommand())
                        {
                            using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            {
                                cmdExcel.Connection = connExcel;

                                //Get the name of First Sheet.
                                connExcel.Open();
                                DataTable dtExcelSchema;
                                dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                connExcel.Close();

                                //Read Data from First Sheet.
                                connExcel.Open();
                                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(dt);
                                connExcel.Close();
                            }
                        }
                    }

                    List<Category> categories = new List<Category>();

                    foreach (DataRow row in dt.Rows)
                    {
                        Category category = new Category();
                        category.Id = Guid.NewGuid();
                        category.CategoryCode = row["CategoryCode"].ToString();
                        category.CategoryName = row["CategoryName"].ToString();
                        category.Description = row["Description"].ToString();
                        categories.Add(category);
                    }
                    _categoryRepository.AddRange(categories);
                }

                if (_categoryRepository.Save())
                {
                    _notyf.Success(_localization.Getkey("common_notyf_update_success"));
                }
                else
                {
                    _notyf.Error(_localization.Getkey("common_notyf_error"));
                }
            }
            catch (Exception)
            {
                _notyf.Error(_localization.Getkey("common_notyf_error"));
            }

            return RedirectToAction("Index", "Category");
        }

        [HttpPost]
        public IActionResult ImportUnit(IFormFile postedFile)
        {
            try
            {
                if (postedFile != null)
                {
                    //Create a Folder.
                    string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    //Save the uploaded Excel file.
                    string fileName = Path.GetFileName(postedFile.FileName);
                    string filePath = Path.Combine(path, fileName);
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                    }

                    //Read the connection string for the Excel file.
                    string conString = this.Configuration.GetConnectionString("ExcelConString");
                    DataTable dt = new DataTable();
                    conString = string.Format(conString, filePath);

                    using (OleDbConnection connExcel = new OleDbConnection(conString))
                    {
                        using (OleDbCommand cmdExcel = new OleDbCommand())
                        {
                            using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            {
                                cmdExcel.Connection = connExcel;

                                //Get the name of First Sheet.
                                connExcel.Open();
                                DataTable dtExcelSchema;
                                dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                connExcel.Close();

                                //Read Data from First Sheet.
                                connExcel.Open();
                                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(dt);
                                connExcel.Close();
                            }
                        }
                    }

                    List<Unit> units = new List<Unit>();

                    foreach (DataRow row in dt.Rows)
                    {
                        Unit unit = new Unit();
                        unit.Id = Guid.NewGuid();
                        unit.UnitCode = row["UnitCode"].ToString();
                        unit.UnitName = row["UnitName"].ToString();
                        unit.Description = row["Description"].ToString();
                        units.Add(unit);
                    }
                    _unitRepository.AddRange(units);
                }

                if (_unitRepository.Save())
                {
                    _notyf.Success(_localization.Getkey("common_notyf_update_success"));
                }
                else
                {
                    _notyf.Error(_localization.Getkey("common_notyf_error"));
                }
            }
            catch (Exception)
            {
                _notyf.Error(_localization.Getkey("common_notyf_error"));
            }

            return RedirectToAction("Index", "Unit");
        }

        [HttpPost]
        public IActionResult ImportProduct(IFormFile postedFile)
        {
            try
            {
                if (postedFile != null)
                {
                    //Create a Folder.
                    string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    //Save the uploaded Excel file.
                    string fileName = Path.GetFileName(postedFile.FileName);
                    string filePath = Path.Combine(path, fileName);
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                    }

                    //Read the connection string for the Excel file.
                    string conString = this.Configuration.GetConnectionString("ExcelConString");
                    DataTable dt = new DataTable();
                    conString = string.Format(conString, filePath);

                    using (OleDbConnection connExcel = new OleDbConnection(conString))
                    {
                        using (OleDbCommand cmdExcel = new OleDbCommand())
                        {
                            using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            {
                                cmdExcel.Connection = connExcel;

                                //Get the name of First Sheet.
                                connExcel.Open();
                                DataTable dtExcelSchema;
                                dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                connExcel.Close();

                                //Read Data from First Sheet.
                                connExcel.Open();
                                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(dt);
                                connExcel.Close();
                            }
                        }
                    }

                    List<Product> products = new List<Product>();

                    foreach (DataRow row in dt.Rows)
                    {
                        Product product = new Product();
                        product.Id = Guid.NewGuid();
                        product.ProductCode = row["ProductCode"].ToString();
                        product.ProductName = row["ProductName"].ToString();
                        product.Manufacturer = row["Manufacturer"].ToString();
                        product.Description = row["Description"].ToString();
                        product.ImportPrice = Convert.ToDecimal(row["ImportPrice"]);
                        product.Price = Convert.ToDecimal(row["Price"]);
                        product.Number = Convert.ToInt32(row["Number"]);
                        product.Status = true;
                        //CultureInfo culture = new CultureInfo("en-US");
                        //product.CreatedDate = Convert.ToDateTime(row["CreatedDate"], culture);

                        //Add category
                        var category = _categoryRepository.GetBy(s => s.CategoryName.Contains(row["Category"].ToString())).FirstOrDefault();
                        if (category == null)
                        {
                            Category newCategory = new Category();
                            newCategory.Id = Guid.NewGuid();
                            newCategory.CategoryCode = newCategory.Id.ToString();
                            newCategory.CategoryName = row["Category"].ToString();
                            _categoryRepository.Add(newCategory);
                            _categoryRepository.Save();
                            product.CategoryId = newCategory.Id;
                        }
                        else
                        {
                            product.CategoryId = category.Id;
                        }

                        //Add Unit
                        var unit = _unitRepository.GetBy(s => s.UnitName.Contains(row["Unit"].ToString())).FirstOrDefault();
                        if (unit == null)
                        {
                            Unit newUnit = new Unit();
                            newUnit.Id = Guid.NewGuid();
                            newUnit.UnitCode = newUnit.Id.ToString();
                            newUnit.UnitName = row["Unit"].ToString();
                            _unitRepository.Add(newUnit);
                            _unitRepository.Save();
                            product.UnitId = newUnit.Id;
                        }
                        else
                        {
                            product.UnitId = unit.Id;
                        }

                        //Add to products list
                        products.Add(product);
                    }

                    _productRepository.AddRange(products);
                }

                if (_productRepository.Save())
                {
                    _notyf.Success(_localization.Getkey("common_notyf_update_success"));
                }
                else
                {
                    _notyf.Error(_localization.Getkey("common_notyf_error"));
                }
            }
            catch (Exception)
            {
                _notyf.Error(_localization.Getkey("common_notyf_error"));
            }

            return RedirectToAction("Index", "Product");
        }
    }
}
