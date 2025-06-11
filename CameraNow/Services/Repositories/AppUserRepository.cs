using Datas.Infrastructures.Cores;
using Datas.Infrastructures.Interfaces;
using Datas.ViewModels.Auth;
using Datas.ViewModels.Product;
using Models.Models;
using Services.Interfaces.Repositories;

namespace Services.Repositories
{
    public class AppUserRepository : BaseRepository<AppUser>, IAppUserRepository
    {
        public AppUserRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public IQueryable<AppUser> ApplyFilter(IQueryable<AppUser> query, AppUserSpecification spec)
        {
            query = query
                .WhereIf(!string.IsNullOrEmpty(spec.Filter), x => x.UserName.Contains(spec.Filter) || x.FullName.Contains(spec.Filter));

            if (!string.IsNullOrEmpty(spec.Sorting))
            {
                query = spec.Sorting switch
                {
                    "name" => query.OrderBy(x => x.UserName),
                    _ => query.OrderBy(x => x.UserName)
                };
            }
            else
            {
                query = query.OrderBy(x => x.UserName);
            }

            return query;
        }
    }
}
