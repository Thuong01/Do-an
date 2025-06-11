using AutoMapper;
using Datas.Infrastructures.Interfaces;
using Datas.ViewModels;
using Datas.Extensions;
using Services.Interfaces.Services;

namespace Services.Services
{
    public class BaseService<TViewModel, TEntity, TCreate, TUpdate> 
        : IBaseService<TViewModel, TCreate, TUpdate> 
            where TViewModel : class where TEntity : class where TCreate : class where TUpdate : class
    {
        private readonly IBaseRepository<TEntity> _baseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BaseService(IBaseRepository<TEntity> baseRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        protected virtual TViewModel ConvertToViewModel(TEntity entity) 
            => throw new NotImplementedException();

        protected virtual TEntity ConvertToEntity(TViewModel dto) 
            => throw new NotImplementedException();

        protected virtual TEntity ConvertToEntity(TCreate create)
            => throw new NotImplementedException();

        protected virtual TEntity ConvertToEntity(TUpdate update)
            => throw new NotImplementedException();

        public virtual async Task<int> DeleteAsync(Guid id)
        {
            await _baseRepository.DeleteAsync(id);
            return await _unitOfWork.CommitAsync();
        }

        public virtual Task<int> DeleteRangeAsync(List<Guid> ids) => throw new NotImplementedException();

        public virtual async Task<TViewModel> GetByIdAsync(Guid id, string[] includes = null)
        {
            var enitty = await _baseRepository.GetSingleByIdAsync(id);

            return ConvertToViewModel(enitty);
        }

        public virtual async Task<int> CreateAsync(TCreate create)
        {
            _baseRepository.Add(ConvertToEntity(create));
            return await _unitOfWork.CommitAsync();
        }

        public virtual async Task<int> UpdateAsync(Guid id, TUpdate update)
        {
            var queryable = await _baseRepository.GetSingleByIdAsync(id);
            _mapper.Map(update, queryable);

            var updated = _baseRepository.Update(queryable);
            return await _unitOfWork.CommitAsync();
        }
    }
}
