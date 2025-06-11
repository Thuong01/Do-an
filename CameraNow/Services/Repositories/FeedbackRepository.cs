using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Datas.Data;
using Datas.Infrastructures.Cores;
using Datas.Infrastructures.Interfaces;
using Models.Models;
using Services.Interfaces.Repositories;
using System.Linq.Expressions;

namespace Services.Repositories
{
    public class FeedbackRepository : BaseRepository<Feedback>, IFeedbackRepository
    {
        private readonly IDbFactory _dbFactory;
        private readonly CameraNowContext _dbContext;

        public FeedbackRepository(IDbFactory dbFactory, CameraNowContext dbContext) : base(dbFactory)
        {
            _dbFactory = dbFactory;
            _dbContext = dbContext;
        }

        public override async Task<IEnumerable<Feedback>> GetManyAsync(Expression<Func<Feedback, bool>> where, string[] includes = null)
        {
            var queryable = _dbContext.Feedbacks.AsQueryable();

            if (where != null)
            {
                queryable = queryable.Where(where);
            }

            if (includes != null && includes.Count() > 0)
            {
                queryable = queryable.Include(includes.First());
                foreach (var item in includes.Skip(1))
                {
                    queryable = queryable.Include(item);
                }
            }

            var feedbacks = (await queryable.ToListAsync()).ToList();

            return feedbacks;
        }
    }
}
