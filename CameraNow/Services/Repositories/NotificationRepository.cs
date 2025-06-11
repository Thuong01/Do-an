using Datas.Data;
using Datas.Infrastructures.Cores;
using Datas.Infrastructures.Interfaces;
using Models.Models;
using Services.Interfaces.Repositories;

namespace Services.Repositories
{
    public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
