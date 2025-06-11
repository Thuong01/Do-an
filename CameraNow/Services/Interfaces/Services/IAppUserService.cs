using Datas.ViewModels.Auth;
using Datas.Extensions;

namespace Services.Interfaces.Services
{
    public interface IAppUserService
    {
        Task<PaginationSet<AppUserViewModel>> GetListAsync(AppUserSpecification spec, PaginatedParams pageParams);
        Task<AppUserViewModel> GetUserByIdAsync(string Id);
    }
}
