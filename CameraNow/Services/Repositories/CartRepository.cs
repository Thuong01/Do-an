using Datas.Data;
using Datas.Infrastructures.Cores;
using Datas.Infrastructures.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using Services.Interfaces.Repositories;

namespace Services.Repositories
{
    public class CartRepository : BaseRepository<Cart>, ICartRepository
    {
        private readonly IDbFactory _dbFactory;
        private readonly CameraNowContext _dbContext;

        public CartRepository(IDbFactory dbFactory, CameraNowContext dbContext) : base(dbFactory)
        {
            _dbFactory = dbFactory;
            _dbContext = dbContext;
        }

        public async Task<List<List<Guid>>> GetCartsData()
        {
            var transactions = new List<List<Guid>>();

            var orders = _dbContext.Carts
                .Include(o => o.CartItems)
                .ThenInclude(od => od.Product)
                .ToList();

            foreach (var order in orders)
            {
                var transaction = order.CartItems
                    .Select(od => od.Product_Id)
                    .Distinct() // Chỉ lấy mỗi sản phẩm một lần trong mỗi đơn hàng
                    .ToList();

                if (transaction.Count > 1) // Chỉ xét các đơn hàng có từ 2 sản phẩm trở lên
                {
                    transactions.Add(transaction);
                }
            }

            return transactions;
        }
    }
}
