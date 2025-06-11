using Datas.ViewModels.Permissions;
using Models.Models;

namespace Services.Interfaces.Services
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionViewModel>> GetListPermissionAsync();

        Task<PermissionViewModel> GetByIdAsync(Guid id);
    }
}
