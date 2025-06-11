using Datas.Infrastructures.Cores;
using Datas.Infrastructures.Interfaces;
using Datas.ViewModels;
using Models.Models;
using Services.Interfaces.Repositories;

namespace Services.Repositories
{
    public class CouponRepository : BaseRepository<Coupon>, ICouponRepository
    {
        private readonly IDbFactory _dbFactory;

        public CouponRepository(IDbFactory dbFactory) : base(dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public IQueryable<Coupon> ApplyFilter(IQueryable<Coupon> query, BaseSpecification spec)
        {
            query = query
                .WhereIf(!string.IsNullOrWhiteSpace(spec.Filter), x => x.Code.ToLower().Contains(spec.Filter.ToLower()));

            if (!string.IsNullOrEmpty(spec.Sorting))
            {
                query = spec.Sorting switch
                {
                    "code" => query.OrderBy(x => x.Code),
                    _ => query.OrderBy(x => x.StartDate)
                };
            }
            else
            {
                query = query.OrderBy(x => x.StartDate);
            }

            return query;
        }
    }
}
