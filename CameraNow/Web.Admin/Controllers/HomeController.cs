using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Datas.Data;
using Datas.Extensions.Responses;
using Datas.ViewModels.Notification;
using Models.Models;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;
using Web.Admin.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace Web.Admin.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _service;
        private readonly INotificationRepository _notificationRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly CameraNowContext _context;
        private readonly IStatsService _statsService;

        public HomeController(ILogger<HomeController> logger, IProductService service, 
                            INotificationRepository notificationRepository, UserManager<AppUser> userManager,
                            CameraNowContext context,
                            IStatsService statsService

            )
        {
            _logger = logger;
            _service = service;
            _notificationRepository = notificationRepository;
            _userManager = userManager;
            _context = context;
            _statsService = statsService;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Product_Count"] = _service.GetCount("SELECT count(*) FROM products", null);
            ViewData["Customer_number"] = _service.GetCount("SELECT count(*) FROM users", null);
            ViewData["Product_Lock_Count"] = (int)_service.GetCount("SELECT count(*) FROM products WHERE status = 0;", null);

            DateTime today = DateTime.Today;
            DateTime firstDayOfMonth = (new DateTime(today.Year, today.Month, 1)).ToUniversalTime();
            DateTime lastDayOfMonth = (new DateTime(today.Year, today.Month,
                DateTime.DaysInMonth(today.Year, today.Month))).ToUniversalTime();

            ViewData["TotalOrder"] = _context.Orders.Count(x => x.Order_Date >= firstDayOfMonth && x.Order_Date <= lastDayOfMonth);
            ViewData["TotalRevenues"] = _context.Orders.Where(x => x.Order_Date >= firstDayOfMonth && x.Order_Date <= lastDayOfMonth).Sum(x => x.Total_Amount);

            var topProduct = await _statsService.GetGeneralStatsAsync(firstDayOfMonth, lastDayOfMonth);
            ViewData["Stats"] = topProduct;

            return View();
        }

        public IActionResult SetCultureCookie(string cltr, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cltr)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        public async Task<ResponseMessage> GetNotification()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var user = await _userManager.FindByIdAsync(userId); // lấy AppUser

                if (user == null)
                    return new ResponseMessage
                    {
                        Success = false
                    };

                var roles = await _userManager.GetRolesAsync(user); // truyền AppUser vào đây
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                var res = (await _notificationRepository.GetAllAsync()).Where(x => x.User_ID == userId && x.Role == "ADMIN");
                return new ResponseMessage
                {
                    Success = true,
                    Result = res.Select(x => new NotificationViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Content = x.Content,
                        CreatedDate = x.CreatedDate,
                        IsRead = x.IsRead,
                        User_ID = x.User_ID,
                        Role = x.Role
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                return new ResponseMessage
                {
                    Success = false
                };
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
