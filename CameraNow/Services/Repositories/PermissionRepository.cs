using Microsoft.EntityFrameworkCore;
using Datas.Infrastructures.Cores;
using Datas.Infrastructures.Interfaces;
using Models.Models;
using Services.Interfaces.Repositories;

namespace Services.Repositories
{
    public class PermissionRepository : BaseRepository<Permissions>, IPermissionRepository
    {
        private readonly IDbFactory _dbFactory;

        public PermissionRepository(IDbFactory dbFactory) : base(dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<IQueryable<Permissions>> GetListPermissionsAsync()
        {
            var permissions = (from pms in _dbFactory.Init().Permissions select pms);

            return permissions;

        }
    }
}
