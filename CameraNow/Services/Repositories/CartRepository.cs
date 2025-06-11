using Datas.Infrastructures.Cores;
using Datas.Infrastructures.Interfaces;
using Models.Models;
using Services.Interfaces.Repositories;

namespace Services.Repositories
{
    public class CartRepository : BaseRepository<Cart>, ICartRepository
    {
        private readonly IDbFactory _dbFactory;

        public CartRepository(IDbFactory dbFactory) : base(dbFactory)
        {
            _dbFactory = dbFactory;
        }
    }
}
