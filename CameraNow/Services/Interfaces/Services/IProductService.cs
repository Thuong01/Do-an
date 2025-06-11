using Microsoft.AspNetCore.Http;
using Datas.ViewModels;
using Datas.ViewModels.Product;

using Datas.Extensions;

namespace Services.Interfaces.Services
{
    public interface IProductService : IBaseService<ProductViewModel, CreateProductViewModel, UpdateProductViewModel>
    {
        Task<IEnumerable<ProductViewModel>> GetAllAsync(string[] includes = null);
        Task<PaginationSet<ProductViewModel>> GetListAsync(ProductSpecification spec, PaginatedParams pageParams, string[] includes = null);
        int GetCount(string query, params object[] parameter);
        Task<ProductViewModel> Update(Guid id, UpdateProductViewModel input, IFormFile? img);
        Task<double> UpdateAverageRating (Guid productId, double rating);
        Task UpdateBuyCount(Guid productId);
        Task UpdateQuantity(Guid productId, int quantity);
    }
}
