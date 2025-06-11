using Datas.ViewModels;
using Datas.ViewModels.Coupon;
using Models.Models;
using Datas.Extensions;

namespace Services.Interfaces.Services
{
    public interface ICouponService : IBaseService<CouponViewModel, CouponCreateViewModel, CouponUpdateViewModel>
    {
        Task<PaginationSet<CouponViewModel>> GetListAsync(BaseSpecification spec, PaginatedParams pageParams, string[] includes = null);
        Task<CouponViewModel> GetByIdAsync(int id);
        Task<CouponViewModel> GetByIdAsync(string code);
        Task<int> DeleteAsync(int id);
        Task<int> UpdateAsync(int id, CouponUpdateViewModel model);
    }
}
