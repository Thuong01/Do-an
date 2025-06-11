using Datas.Data;
using Datas.Infrastructures.Cores;
using Datas.Infrastructures.Interfaces;
using Datas.ViewModels.Product;
using Models.Enums;
using Models.Models;
using Services.Interfaces.Repositories;

namespace Services.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly CameraNowContext _dbContext;

        public ProductRepository(IDbFactory dbFactory, ICategoryRepository categoryRepository, CameraNowContext dbContext) : base (dbFactory)
        {
            _categoryRepository = categoryRepository;
            _dbContext = dbContext;
        }

        public IQueryable<Product> ApplyFilter(IQueryable<Product> query, ProductSpecification spec)
        {
            query = query
                .WhereIf(!string.IsNullOrWhiteSpace(spec.Filter), x => x.Name.ToLower().Contains(spec.Filter.ToLower()))
                .WhereIf(spec.Status != Status.All, x => x.Status == spec.Status);

            if (!Guid.TryParse(spec.Category, out Guid isCategoryId))
            {
                var res = _categoryRepository.GetCategoryByAlias(spec.Category);

                if (res != null)
                {
                    query = query.WhereIf(res.ID != null, x => x.Category_ID == res.ID);
                }
            }
            else
            {
                query = query.WhereIf(!string.IsNullOrEmpty(spec.Category), x => x.Category_ID == Guid.Parse(spec.Category));
            }

            if (!string.IsNullOrEmpty(spec.Sorting))
            {
                query = spec.Sorting switch
                {
                    "name" => query.OrderBy(x => x.Name),
                    "price" => query.OrderBy(x => x.Price), 
                    "date" => query.OrderByDescending(x => x.Creation_Date),
                    _ => query.OrderBy(x => x.Name)
                };
            }
            else
            {
                query = query.OrderBy(x => x.Name);
            }

            return query;
        }
    }
}
