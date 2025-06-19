
using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Datas;
using Datas.Extensions.Responses;
using Datas.ViewModels;
using Datas.ViewModels.Errors;
using Models.Enums;
using Datas.Extensions;
using Services.Interfaces.Services;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Web.Admin.Controllers
{
    public class ProductCategoryController : BaseController
    {
        private readonly ICategoryService _service;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly ILogger<ProductCategoryController> _logger;
        private readonly INotyfService _notif;

        public ProductCategoryController(ICategoryService service,
                                        IMapper mapper,
                                        IImageService imageService,
                                        ILogger<ProductCategoryController> logger,
                                        INotyfService notif)
        {
            _service = service;
            _mapper = mapper;
            _imageService = imageService;
            _logger = logger;
            _notif = notif;
        }

        public async Task<IActionResult> Index(string? filter_text, string? name, [Bind(Prefix = "page")] int pageNumber)
        {
            ViewData["current_fillter"] = filter_text;
            ViewBag.Status = Enum.GetValues(typeof(Status)) as IEnumerable<Status>;

            if (pageNumber == 0) pageNumber = 1;
            int pageSize = 20;

            var spec = new BaseSpecification(filter_text, Status.All, "name");
            var pageParams = new PaginatedParams(pageNumber, pageSize);
            var view_model = await _service.GetListAsync(spec, pageParams);

            ViewBag.pageNumber = pageNumber;
            ViewBag.totalPages = view_model.TotalPage;
            ViewBag.TotalCount = view_model.TotalCount;
            ViewBag.PageSize = pageSize > view_model.TotalCount ? view_model.TotalCount : pageSize;

            if (TempData["Message_Status"] != null && !string.IsNullOrEmpty(TempData["Message_Status"].ToString()) && TempData["Message_Status"].ToString() == "OK")
                _notif.Success(TempData["Message_Info"].ToString());
            else if (TempData["Message_Status"] != null && !string.IsNullOrEmpty(TempData["Message_Status"].ToString()) && TempData["Message_Status"].ToString() == "NG")
            {
                _notif.Success(TempData["Message_Info"].ToString());
            }
            TempData["Message_Info"] = null;
            TempData["Message_Status"] = null;

            return View(view_model.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var spec = new BaseSpecification(null, Status.Active, null);
            ViewBag.ProductCategoryParent = await _service.GetListParentAsync(spec);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductCategoryViewModel input)
        {
            if (ModelState.IsValid)
            {
                var res = await _service.CreateAsync(input);

                if (res != 0)
                {
                    TempData["Message_Status"] = "OK";
                    TempData["Message_Info"] = "Thêm mới thành công.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Message_Status"] = "NG";
                TempData["Message_Info"] = "Thêm mới thất bại.";
                return View(input);
            }

            var spec = new BaseSpecification(null, Status.Active, null);
            ViewBag.ProductCategoryParent = await _service.GetListParentAsync(spec);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _service.GetByIdAsync(id);

            var entity_mapped = _mapper.Map<ProductCategoryViewModel, UpdateProductCategoryViewModel>(entity);
            ViewBag.Status = Enum.GetValues(typeof(Status)) as IEnumerable<Status>;
            var input_parent = new BaseSpecification(null, Status.Active, null);
            ViewBag.ProductCategoryParent = await _service.GetListParentAsync(input_parent);

            return View(entity_mapped);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateProductCategoryViewModel input, IFormFile? img)
        {
            if (ModelState.IsValid)
            {
                var res = await _service.Update(id, input, img);

                if (res != null)
                {
                    TempData["Message_Status"] = "OK";
                    TempData["Message_Info"] = "Chỉnh sửa thành công";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Message_Status"] = "NG";
                TempData["Message_Info"] = "Chỉnh sửa thất bại";
                return View(input);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var entity = await _service.GetByIdAsync(_mapper.Map<Guid>(id));
                    var res = await _service.DeleteAsync(_mapper.Map<Guid>(id));
                    if (res != 0)
                    {
                        _imageService.DeleteImage(entity.Image);
                        //_notif.Success("Xóa thành công");
                        TempData["Message_Status"] = "OK";
                        TempData["Message_Info"] = "Xóa thành công";
                        return Json(new { success = true });
                    }
                    TempData["Message_Status"] = "NG";
                    TempData["Message_Info"] = "Xóa không thành công";
                    return Json(new { success = false });
                }
                return View(nameof(Index));
            }
            catch (FormatException)
            {
                _logger.LogError("Invalid data format" + id);
                return View(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error" + id);
                _logger.LogError(ex.ToString());
                return View(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRangeAsync(List<string> ids)
        {
            try
            {
                if (ids != null && ids.Count > 0)
                {
                    var res = await _service.DeleteRangeAsync(_mapper.Map<List<Guid>>(ids));

                    if (res != 0)
                    {
                        TempData["Message_Info"] = $"Xóa thành công {ids.Count} danh mục sản phẩm!";
                        return new JsonResult(new ResponseMessage(true, $"Xóa thành công {ids.Count} danh mục sản phẩm!"));
                    }
                }

                return new JsonResult(new ExceptionResponse(StatusCodes.Status400BadRequest, $"Xóa {ids.Count} Product category không thành công"));
            }
            catch (FormatException fex)
            {
                _logger.LogError("Invalid data format" + fex.Message);
                TempData["Message_Info"] = $"Xóa không thành công {ids.Count} danh mục sản phẩm!";
                return new JsonResult(new ExceptionResponse(StatusCodes.Status400BadRequest, $"Xóa {ids.Count} Product category không thành công"));

            }
            catch (Exception ex)
            {
                _logger.LogError("Error" + ex.Message);
                _logger.LogError(ex.ToString());
                TempData["Message_Info"] = $"Xóa không thành công {ids.Count} danh mục sản phẩm!";
                return new JsonResult(new ExceptionResponse(StatusCodes.Status400BadRequest, $"Xóa {ids.Count} Product category không thành công"));
            }

        }

    }
}
