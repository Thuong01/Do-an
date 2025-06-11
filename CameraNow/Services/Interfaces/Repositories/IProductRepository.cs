using Datas.Infrastructures.Interfaces;
using Datas.ViewModels;
using Datas.ViewModels.Product;
using Models.Models;

namespace Services.Interfaces.Repositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        IQueryable<Product> ApplyFilter(IQueryable<Product> query, ProductSpecification spec);
    }
}
