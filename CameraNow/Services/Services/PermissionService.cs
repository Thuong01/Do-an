using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Datas.ViewModels.Permissions;
using Models.Models;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;

namespace Services.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        public PermissionService(IPermissionRepository permissionRepository, IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        public async Task<PermissionViewModel> GetByIdAsync(Guid id)
        {
            var permission = _permissionRepository.GetSingleById(id);

            return _mapper.Map<PermissionViewModel>(permission);
        }

        public async Task<IEnumerable<PermissionViewModel>> GetListPermissionAsync()
        {
            var repo = await _permissionRepository.GetAllAsync();

            var permissions = _mapper.Map<List<Permissions>, List<PermissionViewModel>>(repo.ToList());

            // Tìm các quyền gốc
            var roots = permissions.Where(x => string.IsNullOrEmpty(x.ParentName)).ToList();

            // Tạo một từ điển để dễ dàng tìm quyền con
            var permissionLookup = permissions.ToLookup(x => x.ParentName);

            // Duyệt qua danh sách các quyền gốc và thêm quyền con
            foreach (var root in roots)
            {
                root.PermissionsChild = permissionLookup[root.Name].ToList();
            }


            return roots;
        }
    }
}
