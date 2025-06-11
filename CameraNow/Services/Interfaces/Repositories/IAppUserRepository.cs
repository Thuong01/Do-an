using Datas.Infrastructures.Interfaces;
using Datas.ViewModels.Auth;
using Datas.ViewModels.Product;
using Models.Models;

namespace Services.Interfaces.Repositories
{
    public interface IAppUserRepository : IBaseRepository<AppUser>
    {
        IQueryable<AppUser> ApplyFilter(IQueryable<AppUser> query, AppUserSpecification spec);
    }
}
