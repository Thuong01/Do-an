using Datas.Infrastructures.Interfaces;
using Datas.ViewModels;
using Models.Models;

namespace Services.Interfaces.Repositories
{
    public interface ICouponRepository : IBaseRepository<Coupon>
    {
        IQueryable<Coupon> ApplyFilter(IQueryable<Coupon> query, BaseSpecification spec);
    }
}
