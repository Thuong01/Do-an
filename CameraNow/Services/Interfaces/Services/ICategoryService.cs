using Microsoft.AspNetCore.Http;
using Datas.ViewModels;
using Datas.Extensions;

namespace Services.Interfaces.Services
{
    public interface ICategoryService 
        : IBaseService<ProductCategoryViewModel, CreateProductCategoryViewModel, UpdateProductCategoryViewModel>
    {
        Task<PaginationSet<ProductCategoryViewModel>> GetListAsync(BaseSpecification spec, PaginatedParams pageParams, string[] includes = null);
        Task<IEnumerable<ProductCategoryViewModel>> GetListParentAsync(BaseSpecification spec);
        Task<ProductCategoryViewModel> Update(Guid id, UpdateProductCategoryViewModel input, IFormFile img = null);
    }
}
