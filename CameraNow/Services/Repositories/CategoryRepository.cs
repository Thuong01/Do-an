using Datas.Infrastructures.Cores;
using Datas.Infrastructures.Interfaces;
using Datas.ViewModels;
using Models.Enums;
using Models.Models;
using Services.Interfaces.Repositories;

namespace Services.Repositories
{
    public class CategoryRepository : BaseRepository<ProductCategory>, ICategoryRepository
    {
        private readonly IDbFactory _dbFactory;

        public CategoryRepository(IDbFactory dbFactory) : base(dbFactory) 
        {
            _dbFactory = dbFactory;
        }

        public IQueryable<ProductCategory> ApplyFilter(IQueryable<ProductCategory> query, BaseSpecification spec)
        {
            query = query
                .WhereIf(!string.IsNullOrWhiteSpace(spec.Filter), x => x.Name.ToLower().Contains(spec.Filter.ToLower()))
                .WhereIf(spec.Status != Status.All, x => x.Status == spec.Status);

            if (!string.IsNullOrEmpty(spec.Sorting))
            {
                query = spec.Sorting switch
                {
                    "name" => query.OrderBy(x => x.Name),
                    "date" => query.OrderBy(x => x.Creation_Date),
                    _ => query.OrderBy(x => x.Name)
                };
            }
            else
            {
                query = query.OrderBy(x => x.Name);
            }

            return query;

        }

        public ProductCategory GetCategoryByAlias(string alias)
        {
           return _dbFactory.Init().ProductCategories.Where(x =>  x.Alias == alias).FirstOrDefault();            
        }
    }
}
