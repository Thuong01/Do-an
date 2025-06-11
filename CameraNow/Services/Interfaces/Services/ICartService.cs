using Microsoft.AspNetCore.Http.HttpResults;
using Datas.ViewModels.Cart;

namespace Services.Interfaces.Services
{
    public interface ICartService
    {
        Task<int> CreateCartAsync(string userId);
        Task<CartViewModel> GetCarts(string userId);
        Task<int> CreateAsync(CartItemCreateViewModel create, string userId);
        Task<int> UpdateAsync(Guid cartId, Guid productId, int quantity);
        Task<int> DeleteAsync(string userId, Guid cartId, Guid productId);
        Task<Guid> GetCartId(string userId);
    }
}
