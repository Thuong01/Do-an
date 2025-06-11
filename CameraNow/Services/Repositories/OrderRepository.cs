using Microsoft.EntityFrameworkCore;
using Datas.Data;
using Datas.Infrastructures.Cores;
using Datas.Infrastructures.Interfaces;
using Models.Enums;
using Models.Models;
using Services.Interfaces.Repositories;

namespace Services.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private readonly IDbFactory _dbFactory;
        private readonly CameraNowContext _dbContext;

        public OrderRepository(IDbFactory dbFactory, CameraNowContext dbContext) : base(dbFactory)
        {
            _dbFactory = dbFactory;
            _dbContext = dbContext;
        }

        // Hàm lấy dữ liệu transactions từ database
        public async Task<List<List<Guid>>> GetTransactionData()
        {
            var transactions = new List<List<Guid>>();

            var orders = _dbContext.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToList();

            foreach (var order in orders)
            {
                var transaction = order.OrderDetails
                    .Select(od => od.Product_ID)
                    .Distinct() // Chỉ lấy mỗi sản phẩm một lần trong mỗi đơn hàng
                    .ToList();

                if (transaction.Count > 1) // Chỉ xét các đơn hàng có từ 2 sản phẩm trở lên
                {
                    transactions.Add(transaction);
                }
            }

            return transactions;
        }

        public async Task<IEnumerable<Order>> GetOrdersUserAsync(string userId, OrderStatusEnum status, string[] includes = null, string keyword = null)
        {
            var entities = _dbContext.Orders
                                .Include(x => x.OrderDetails)
                                .ThenInclude(od => od.Product)
                                .Where(x => x.User_Id == userId);

            if (status == null)
            {
                entities = entities.Where(x => x.Status == status);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                entities = entities.Where(x => x.OrderNo.Contains(keyword) || x.OrderDetails.Any(od => od.Product.Name.Contains(keyword)));
            }

            return await entities.ToListAsync();
        }
    }
}
