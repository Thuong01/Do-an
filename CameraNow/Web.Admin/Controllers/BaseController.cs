using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Web.Admin.Models;

namespace Web.Admin.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var menuItems = new List<MenuItem>
            {
                new MenuItem {Name="Dashboard", Controller = "Home", Action="Index"},
                new MenuItem
                {
                    Name = "Product_Categories",
                    Controller="ProductCategory",
                    Action="Index",
                    Icon = "",
                },
                new MenuItem
                {
                    Name = "Products",
                    Controller="Product",
                    Icon = "<i class=\"fa-brands fa-product-hunt\"></i>",
                    Action="Index"
                },
                new MenuItem
                {
                    Name = "Coupons",
                    Controller="Coupon",
                    Icon = "<i class=\"fa-solid fa-ticket-simple\"></i>",
                    Action="Index"
                },
                new MenuItem
                {
                    Name = "Orders",
                    Controller="Order",
                    Icon = "<i class=\"fa-solid fa-receipt\"></i>",
                    Action="Index"
                },
                new MenuItem
                {
                    Name = "Accounts",
                    Controller="Account",
                    Icon = "<i class=\"fa-solid fa-users\"></i>",
                    Action="Index"
                },
                new MenuItem
                {
                    Name = "Statistic",
                    Controller="Stats",
                    Action="Index",
                    Icon = "<i class=\"fa-solid fa-chart-simple\"></i>",
                },
            };

            ViewBag.MenuItems = menuItems;

            base.OnActionExecuting(context);
        }
    }
}
