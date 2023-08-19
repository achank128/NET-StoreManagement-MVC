using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data.OleDb;
using System.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StoreManagement.Models;
using StoreManagement.Repositories.CategoryRepository;
using StoreManagement.Repositories.ProductRepository;
using StoreManagement.Repositories.UnitRepository;
using System.Globalization;

namespace StoreManagement.Controllers
{
    public class FileUploadController : Controller
    {
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;
        private IConfiguration Configuration;
        private readonly IProductRepository _productRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly ICategoryRepository _categoryRepository;

        public FileUploadController(
            Microsoft.AspNetCore.Hosting.IHostingEnvironment environment,
            IConfiguration configuration,
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            IUnitRepository unitRepository)
        {
            Environment = environment;
            Configuration = configuration;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _unitRepository = unitRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            var filePaths = new List<string>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    // full path to file in temp location
                    var filePath = Path.GetTempFileName(); //we are using Temp file name just for the example. Add your own file path.
                    filePaths.Add(filePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            // process uploaded files
            // Don't rely on or trust the FileName property without validation.
            return Ok(new { count = files.Count, size, filePaths });
        }

        [HttpPost]
        public IActionResult ImportProduct(IFormFile postedFile)
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
                    CultureInfo culture = new CultureInfo("en-US");
                    product.CreatedDate = Convert.ToDateTime(row["CreatedDate"], culture);

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
                _productRepository.Save();
            }

            return RedirectToAction("Index", "Product");
        }
    }
}
