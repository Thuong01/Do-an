using Datas.Infrastructures.Cores;
using Datas.Infrastructures.Interfaces;
using Models.Models;
using Services.Interfaces.Repositories;

namespace Services.Repositories
{
    public class CartItemrepository : BaseRepository<CartItem>, ICartItemRepository
    {
        public CartItemrepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
