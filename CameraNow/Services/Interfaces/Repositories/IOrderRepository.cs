using Datas.Infrastructures.Interfaces;
using Models.Enums;
using Models.Models;

namespace Services.Interfaces.Repositories
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersUserAsync(string userId, OrderStatusEnum status, string[] includes = null, string keyword = null);
        Task<List<List<Guid>>> GetTransactionData();
    }
}
