using AutoMapper;
using Datas.ViewModels.Auth;
using Models.Models;
using Datas.Extensions;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;
using Datas.Extentions;

namespace Services.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly IAppUserRepository _repository;
        private readonly IMapper _mapper;

        public AppUserService(IAppUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PaginationSet<AppUserViewModel>> GetListAsync(AppUserSpecification spec, PaginatedParams pageParams)
        {
            var users = _repository.ApplyFilter(_repository.GetAll(), spec);

            var userVM = PaginatedList<AppUser>.Create(users, pageParams.PageNumber, pageParams.PageSize);

            var userPaging = _mapper.Map<PaginatedList<AppUserViewModel>>(userVM);

            return new PaginationSet<AppUserViewModel>(userPaging.PageIndex, userPaging.TotalPages, userPaging.TotalCount, userPaging);
        }

        public Task<AppUserViewModel> GetUserByIdAsync(string Id)
        {
            throw new NotImplementedException();
        }
    }
}
