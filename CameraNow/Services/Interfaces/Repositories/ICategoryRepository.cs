using Datas.Infrastructures.Interfaces;
using Datas.ViewModels;
using Models.Models;

namespace Services.Interfaces.Repositories
{
    public interface ICategoryRepository : IBaseRepository<ProductCategory>
    {
        ProductCategory GetCategoryByAlias(string alias);

        IQueryable<ProductCategory> ApplyFilter(IQueryable<ProductCategory> query, BaseSpecification spec);
    }
}
