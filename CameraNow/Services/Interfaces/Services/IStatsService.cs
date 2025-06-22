using Datas.ViewModels.Statistic;

namespace Services.Interfaces.Services
{
    public interface IStatsService
    {
        Task<StatsViewModel> GetGeneralStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<Dictionary<string, decimal>> GetRevenueByDayAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<TopProduct>> GetTopSellingProductsAsync(int top = 5);
    }
}
