using Microsoft.AspNetCore.Mvc;

namespace Web.Admin.Controllers
{
    public class ImageUploadController : BaseController
    {
        private readonly IWebHostEnvironment _env;

        public ImageUploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadImage(List<IFormFile> files)
        {
            var file_path = "";
            foreach (var item in Request.Form.Files)
            {
                string serverMapPath = Path.Combine(_env.WebRootPath, "img/CkeditorPost/", item.FileName);
                using (var stream = new FileStream(serverMapPath, FileMode.Create))
                {
                    item.CopyTo(stream);
                }

                file_path = "https://localhost:7100/" + "img/CkeditorPost/" + item.FileName;
            }

            return Json(new { url = file_path });
        }
    }
}
