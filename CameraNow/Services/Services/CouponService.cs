using AutoMapper;
using Datas.Infrastructures.Interfaces;
using Datas.ViewModels;
using Datas.ViewModels.Coupon;
using Models.Models;
using Datas.Extensions;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;
using Datas.Extentions;

namespace Services.Services
{
    public class CouponService 
        : BaseService<CouponViewModel, Coupon, CouponCreateViewModel, CouponUpdateViewModel>, ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CouponService(ICouponRepository couponRepository, IUnitOfWork unitOfWork, IMapper mapper) : base(couponRepository, unitOfWork, mapper)
        {
            _couponRepository = couponRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        protected override Coupon ConvertToEntity(CouponCreateViewModel create)
        {
            return _mapper.Map<Coupon>(create);
        }

        protected override Coupon ConvertToEntity(CouponUpdateViewModel update)
        {
            return _mapper.Map<Coupon>(update);
        }

        protected override Coupon ConvertToEntity(CouponViewModel dto)
        {
            return _mapper.Map<Coupon>(dto);
        }

        protected override CouponViewModel ConvertToViewModel(Coupon entity)
        {
            return _mapper.Map<CouponViewModel>(entity);
        }

        public async Task<PaginationSet<CouponViewModel>> GetListAsync(BaseSpecification spec, PaginatedParams pageParams, string[] includes = null)
        {
            var entities = _couponRepository.ApplyFilter(_couponRepository.GetAll(), spec);
            var pagingList = PaginatedList<Coupon>.Create(entities, pageParams.PageNumber, pageParams.PageSize);
            var pagingListVM = _mapper.Map<PaginatedList<CouponViewModel>>(pagingList);
            return new PaginationSet<CouponViewModel>(pagingListVM.PageIndex, pagingListVM.TotalPages, pagingListVM.TotalCount, pagingListVM);
        }

        public override async Task<int> CreateAsync(CouponCreateViewModel create)
        {
            var entity = ConvertToEntity(create);

            if (entity.StartDate.HasValue)
                entity.StartDate = DateTime.SpecifyKind(entity.StartDate.Value, DateTimeKind.Utc);

            if (entity.EndDate.HasValue)
                entity.EndDate = DateTime.SpecifyKind(entity.EndDate.Value, DateTimeKind.Utc);

            var res = _couponRepository.Add(entity);

            if (res == null) return 0;

            return await _unitOfWork.CommitAsync();
        }

        public async Task<CouponViewModel> GetByIdAsync(int id)
        {
            return ConvertToViewModel(await _couponRepository.GetSingleByIdAsync(id));
        }

        public async Task<CouponViewModel> GetByIdAsync(string code)
        {
            return ConvertToViewModel(_couponRepository.GetSingleByCondition(x => x.Code.Equals(code)));
        }

        public async Task<int> UpdateAsync(int id, CouponUpdateViewModel model)
        {
            var queryable = await _couponRepository.GetSingleByIdAsync(id);
            _mapper.Map(model, queryable);

            if (queryable.StartDate.HasValue)
                queryable.StartDate = DateTime.SpecifyKind(queryable.StartDate.Value, DateTimeKind.Utc);

            if (queryable.EndDate.HasValue)
                queryable.EndDate = DateTime.SpecifyKind(queryable.EndDate.Value, DateTimeKind.Utc);

            return await _unitOfWork.CommitAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var entity = await _couponRepository.GetSingleByIdAsync(id);

            var res = _couponRepository.Delete(entity);

            return await _unitOfWork.CommitAsync();
        }
    }
}
