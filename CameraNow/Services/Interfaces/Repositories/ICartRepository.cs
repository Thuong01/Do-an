using Datas.Infrastructures.Interfaces;
using Models.Models;

namespace Services.Interfaces.Repositories
{
    public interface ICartRepository : IBaseRepository<Cart>
    {
        Task<List<List<Guid>>> GetCartsData();
    }
}
