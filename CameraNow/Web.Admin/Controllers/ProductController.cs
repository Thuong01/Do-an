using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Datas.ViewModels;
using Datas.ViewModels.Product;
using Models.Enums;
using Datas.Extensions;
using Services.Interfaces.Services;
using System.Net;
using Microsoft.IdentityModel.Tokens;

namespace Web.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService _service;
        private readonly IMapper _mapper;
        private readonly INotyfService _notif;
        private readonly ICategoryService _categories;
        private readonly IFeedbackService _feedbackService;
        private readonly ILogger<ProductCategoryController> _logger;

        public ProductController(IProductService service,
            IMapper mapper,
            ICategoryService categories,
            IFeedbackService feedbackService,
            ILogger<ProductCategoryController> logger,
            INotyfService notif)
        {
            _service = service;
            _mapper = mapper;
            _notif = notif;
            _categories = categories;
            _feedbackService = feedbackService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string filter_text, [Bind(Prefix = "page")] int page, Guid? advSlcCateg, DateTime? advSlcCreatedDate, Status advSlcStatus = Status.All,  string sorting = "name")
        {
            ViewData["current_fillter"] = filter_text;
            ViewData["current_advSlcCateg"] = advSlcCateg;
            ViewData["current_advSlcStatus"] = advSlcStatus;
            ViewData["current_advSlcCreatedDate"] = advSlcCreatedDate;

            if (page <= 0) page = 1;
            var spec = new ProductSpecification(filter_text, advSlcStatus, sorting, advSlcCateg.ToString(), advSlcCreatedDate);
            var pageParams = new PaginatedParams(page, 48);
            var res = await _service.GetListAsync(spec, pageParams);

            ViewBag.Page = page;
            ViewBag.TotalPages = res.TotalPage;
            ViewBag.TotalCount = res.TotalCount;

            ViewBag.ProductCategories = await _categories.GetListParentAsync(new BaseSpecification(null, Status.All, "name"));

            if (TempData["Message_Status"] != null && !string.IsNullOrEmpty(TempData["Message_Status"].ToString()) && TempData["Message_Status"].ToString() == "OK")
                _notif.Success(TempData["Message_Info"].ToString());
            else if (TempData["Message_Status"] != null && !string.IsNullOrEmpty(TempData["Message_Status"].ToString()) && TempData["Message_Status"].ToString() == "NG")
            {
                _notif.Success(TempData["Message_Info"].ToString());
            }
            TempData["Message_Info"] = null;
            TempData["Message_Status"] = null;

            return View(res.Data);
        }

        public async Task<IActionResult> Create()
        {
            var input_parent = new BaseSpecification(null, Status.Active, "name");
            ViewBag.ProductCategoryParent = await _categories.GetListParentAsync(input_parent);
            //ViewBag.Brands = await _brand.GetBrands(input_parent);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductViewModel input)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var res = await _service.CreateAsync(input);

                    if (res != 0)
                    {
                        TempData["Message_Status"] = "OK";
                        TempData["Message_Info"] = "Thêm mới thành công";
                        return new JsonResult(new
                        {
                            success = true
                        });
                    }

                    TempData["Message_Status"] = "NG";
                    // TempData["Message_Info"] = "Thêm mới thất bại";
                    return new JsonResult(new
                    {
                        success = true
                    });
                }

                return new JsonResult(new
                {
                    success = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return new JsonResult(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        public async Task<IActionResult> Update(Guid id)
        {
            try
            {
                var entity = await _service.GetByIdAsync(id);
                var spec = new BaseSpecification(null, Status.Active, "name");
                ViewBag.ProductCategoryParent = await _categories.GetListParentAsync(spec);
                //ViewBag.Brands = await _brand.GetBrands(spec);

                var parse = (_mapper.Map<UpdateProductViewModel>(entity));

                return View(parse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return View(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Guid id, UpdateProductViewModel input, IFormFile? img)
        {
            try
            {
                var res = await _service.Update(id, input, img);

                if (res != null)
                {
                    TempData["Message_Status"] = "OK";
                    TempData["Message_Info"] = "Cập nhật thành công";
                    return new JsonResult(new
                    {
                        success = true,
                        highlightId = id,
                    });
                }

                TempData["Message_Status"] = "NG";
                TempData["Message_Success"] = "Cập nhật thất bại";
                return new JsonResult(new
                {
                    success = false,                    
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new JsonResult(new
                {
                    success = false,
                    error = ex.Message,
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var res = await _service.DeleteAsync(_mapper.Map<Guid>(id));

                if (res != 0)
                {
                    TempData["Message_Status"] = "OK";
                    TempData["Message_Info"] = "Xóa thành công";
                    return Json(new
                    {
                        success = true,
                        error = false,
                    });
                }

                return Json(new
                {
                    success = false,
                    error = true,
                });
            }
            catch (Exception ex)
            {
                TempData["Message_Status"] = "NG";
                TempData["Message_Info"] = "Xóa không thành công";

                return Json(new
                {
                    success = false,
                    error = new
                    {
                        code = HttpStatusCode.BadRequest,
                        message = ex.Message,
                        stacktrace = ex.StackTrace,
                    },
                });
            }

        }

        [HttpPost]
        public async Task<IActionResult> DeleteRange(List<string> ids)
        {
            try
            {
                var res = await _service.DeleteRangeAsync(_mapper.Map<List<Guid>>(ids));

                if (res != 0)
                {
                    TempData["Message_Status"] = "OK";
                    TempData["Message_Info"] = $"Xóa {ids.Count} sản phẩm thành công";
                    return Json(new
                    {
                        success = true,
                        error = false,
                    });
                }

                TempData["Message_Info"] = $"Không có sản phẩm nào được chọn";
                return Json(new
                {
                    success = false,
                    error = true,
                });
            }
            catch (Exception ex)
            {
                TempData["Message_Status"] = "NG";
                TempData["Message_Info"] = "Xóa không thành công";

                return Json(new
                {
                    success = false,
                    error = new
                    {
                        code = HttpStatusCode.BadRequest,
                        message = ex.Message,
                        stacktrace = ex.StackTrace,
                    },
                });
            }

        }

        // GET: Product/Details/id
        public async Task<IActionResult> Details(Guid id, PaginatedParams FeedbackPageParams)
        {
            try
            {
                if (id != null)
                {
                    var res = await _service.GetByIdAsync(id);

                    ViewBag.Feedback = await _feedbackService.GetFeedbacks(id, FeedbackPageParams);

                    return View(res);
                }

                return View(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Message_Info"] = "Có lỗi xảy ra!";
                _logger.LogError(ex.ToString());
                return View(nameof(Index));
            }
        }

    }
}
