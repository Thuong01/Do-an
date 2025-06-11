using Datas.Infrastructures.Cores;
using Datas.Infrastructures.Interfaces;
using Models.Models;
using Services.Interfaces.Repositories;

namespace Services.Repositories
{
    public class ImageRepository : BaseRepository<ProductImage>, IImageRepository
    {
        public ImageRepository(IDbFactory dbFactory) : base (dbFactory)
        {
            
        }
    }
}
