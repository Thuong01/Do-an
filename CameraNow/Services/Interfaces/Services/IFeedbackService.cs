using Datas.Extensions;
using Datas.ViewModels.Feedback;

namespace Services.Interfaces.Services
{
    public interface IFeedbackService
    {
        Task<int> CreateAsync(FeedbackCreateViewModel input);
        Task<Data> GetUserFeedback(string userId, Guid productId);
        Task<FeedbackViewModel> GetFeedbacks(Guid productId, PaginatedParams pageParams);
        Task<int> LikeFeedback(Guid fbId, Guid productId);
        Task<int> DeleteFeedback(Guid fbId, Guid productId);
    }
}
