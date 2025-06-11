using Microsoft.EntityFrameworkCore;
using Datas.Data;
using Datas.ViewModels.Statistic;
using Services.Interfaces.Services;

namespace Services.Services
{
    public class StatsService : IStatsService
    {
        private readonly CameraNowContext _context;

        public StatsService(CameraNowContext context)
        {
            _context = context;
        }

        public async Task<StatsViewModel> GetGeneralStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Orders.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(o => o.Order_Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.Order_Date <= endDate.Value);

            var hasOrders = await query.AnyAsync();

            var stats = new StatsViewModel
            {
                TotalOrders = await query.CountAsync(),
                TotalRevenue = hasOrders ? await query.SumAsync(o => o.Total_Amount) : 0,
                AverageOrderValue = hasOrders ? await query.AverageAsync(o => o.Total_Amount) : 0,
                OrdersByStatus = hasOrders
                    ? await query
                        .GroupBy(o => o.Status)
                        .Select(g => new { Status = g.Key, Count = g.Count() })
                        .ToDictionaryAsync(x => x.Status.ToString(), x => x.Count)
                    : new Dictionary<string, int>(),
                RevenueByDay = await GetRevenueByDayAsync(),
                TopCustomers = [],
                TopSellingProducts = await GetTopSellingProductsAsync(),
                RecentOrders = await GetRecentOrdersAsync(10)

            };

            return stats;
        }

        public async Task<Dictionary<string, decimal>> GetRevenueByDayAsync(int days = 30)
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-days);

            return await _context.Orders
                .Where(o => o.Order_Date >= startDate && o.Order_Date <= endDate)
                .GroupBy(o => o.Order_Date.Date)
                .OrderBy(g => g.Key)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(o => o.Total_Amount) })
                .ToDictionaryAsync(x => x.Date.ToString("dd/MM"), x => x.Revenue);
        }

        public async Task<List<TopProduct>> GetTopSellingProductsAsync(int top = 5)
        {
            return await _context.OrderDetails
                .Include(od => od.Product)
                .GroupBy(od => new
                {
                    od.Product_ID,
                    od.Product.Name,
                    od.Product.Image,
                    od.Product.Price
                })
                .OrderByDescending(g => g.Sum(od => od.Quantity))
                .Take(top)
                .Select(g => new TopProduct
                {
                    ProductId = g.Key.Product_ID,
                    ProductName = g.Key.Name,
                    ProductImage = g.Key.Image,
                    ProductPrice = g.Key.Price,
                    QuantitySold = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.Quantity * g.Key.Price)
                })
                .ToListAsync();
        }

        public async Task<List<RecentOrderDto>> GetRecentOrdersAsync(int count = 10)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.Order_Date)
                .Take(count)
                .Select(o => new RecentOrderDto
                {
                    OrderId = o.OrderNo,
                    CustomerName = o.User.UserName,
                    ProductName = o.OrderDetails
                        .Select(od => od.Product.Name)
                        .FirstOrDefault() ?? "N/A",
                    Price = o.Total_Amount,
                    Status = o.Status.ToString()
                })
                .ToListAsync();
        }
    }
}
