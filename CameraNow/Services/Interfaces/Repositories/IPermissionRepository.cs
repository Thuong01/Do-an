using Datas.Infrastructures.Interfaces;
using Models.Models;

namespace Services.Interfaces.Repositories
{
    public interface IPermissionRepository : IBaseRepository<Permissions>
    {
        Task<IQueryable<Permissions>> GetListPermissionsAsync();
    }
}
