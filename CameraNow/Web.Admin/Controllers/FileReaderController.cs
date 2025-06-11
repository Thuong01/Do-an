using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Datas;
using Datas.ViewModels;
using Models.Enums;
using Datas.Extensions;
using Services.Interfaces.Services;
using System.ComponentModel;
using System.Data;
using Datas.ViewModels.Product;

namespace Web.Admin.Controllers
{
    public class FileReaderController : BaseController
    {
        private readonly HttpClient _httpClient;
        private readonly IProductService _product;

        public FileReaderController(HttpClient httpClient, IProductService product)
        {
            _httpClient = httpClient;
            _product = product;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ReadImage(string path)
        {
            //if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
            //{
            //}
            //else 
            
            if (System.IO.File.Exists(path)) {
                var bytes = System.IO.File.ReadAllBytes(path);

                return File(bytes, "image/jpg");
            }


            if (!string.IsNullOrEmpty(path))
            {
                var bytes = await _httpClient.GetByteArrayAsync(path);

                return File(bytes, "image/jpg");
            }

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> Preview()
        {
            var dataTable = await GetDataFromDatabase();
            var stream = new MemoryStream();
            byte[] fileContents;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells.LoadFromDataTable(dataTable, true);
                fileContents = package.GetAsByteArray();
            }

            stream = new MemoryStream(fileContents);

            stream.Position = 0;
            var fileName = $"Export-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";
            // "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(stream, contentType, fileName);
        }

        private async Task<DataTable> GetDataFromDatabase()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Name");
            dataTable.Columns.Add("Price");

            var spec = new ProductSpecification(null, Status.All, null, null, null);
            var pageParams = new PaginatedParams(1, 20);

            var entities = await _product.GetListAsync(spec, pageParams);

            foreach (var item in entities.Data)
            {
                var row = dataTable.NewRow();
                row["Name"] = item.Name;
                row["Price"] = item.Price;  
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}
