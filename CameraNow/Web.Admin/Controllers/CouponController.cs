using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Datas.ViewModels;
using Datas.ViewModels.Coupon;
using Models.Enums;
using Datas.Extensions;
using Services.Interfaces.Services;

namespace Web.Admin.Controllers
{
    public class CouponController : BaseController
    {
        private readonly ICouponService _couponService;
        private readonly IMapper _mapper;
        private readonly ILogger<CouponController> _logger;
        private readonly INotyfService _notify;

        public CouponController(ICouponService couponService, IMapper mapper, ILogger<CouponController> logger, INotyfService notify)
        {
            _couponService = couponService;
            _mapper = mapper;
            _logger = logger;
            _notify = notify;
        }

        public async Task<IActionResult> Index(string? filter, [Bind(Prefix = "page")] int page, string sort = "name")
        {
            ViewData["Current_Filter"] = filter;

            if (TempData["Notify"] != null)
            {
                _notify.Success(TempData["Notify"].ToString());
            }
            TempData["Notify"] = null;

            if (page <= 0) page = 1;
            var spec = new BaseSpecification(filter, Status.All, sort);
            var pageParams = new PaginatedParams(page, 20);
            var res = await _couponService.GetListAsync(spec, pageParams);

            ViewBag.PageNumber = page;
            ViewBag.TotalPages = res.TotalPage;
            ViewBag.TotalCount = res.TotalCount;

            return View(res.Data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CouponCreateViewModel input)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var res = await _couponService.CreateAsync(input);

                    if (res == 0)
                    {
                        ModelState.AddModelError("", "Tạo mã giảm giá không thành công");
                        return View();
                    }

                    TempData["Notify"] = "Tạo mã giảm giá thành công";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Dữ liệu không hợp lệ");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating coupon");
                return BadRequest(ex);
            }
        }

        public async Task<IActionResult> Update(int id)
        {
            try
            {
                var res = await _couponService.GetByIdAsync(id);

                var vm = _mapper.Map<CouponUpdateViewModel>(res);

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating coupon");
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, CouponUpdateViewModel input)
        {
            try
            {
                var res = await _couponService.UpdateAsync(id, input);

                if (res == 0)
                    return View(input);

                TempData["Notify"] = "Cập nhật mã giảm giá thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating coupon");
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var res = await _couponService.DeleteAsync(id);

                if (res == 0)
                    return BadRequest();

                TempData["Notify"] = "Xóa mã giảm giá thành công";
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting coupon");
                return new JsonResult(new { success = false });
            }
        }
    }
}
