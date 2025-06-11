using Datas.ViewModels.Order;
using Models.Enums;
using Datas.Extensions;

namespace Services.Interfaces.Services
{
    public interface IOrderService
    {
        Task<PaginationSet<OrderViewModel>> GetAllOrdersAsync(PaginatedParams pageParams, string[] includes = null);
        Task<PaginationSet<OrderViewModel>> GetOrdersUserAsync(string userId, PaginatedParams pageParams, OrderStatusEnum status, string[] includes = null, string keyword = null);
        Task<OrderViewModel> CreateAsync(OrderCreateViewModel create);
        Task<int> CancelledOrder(Guid orderId, string userId);
        Task<OrderViewModel> GetOrderById(Guid orderId, string userId, string[] includes = null);
        Task<int> UpdateOrderStatus(Guid orderId, string userId, OrderStatusEnum status);
        List<List<Guid>> GetTransactionData();
        Task<int> UpdateOrderIsCommented(Guid orderId, string userId);
        Task<int> IsPaymentOrder(Guid orderId, string userId, string transactionNo);
    }
}
