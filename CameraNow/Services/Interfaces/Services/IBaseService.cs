using Datas.ViewModels;
using Datas.Extensions;

namespace Services.Interfaces.Services
{
    public interface IBaseService<TViewModel, TCreate, TUpdate> 
        where TViewModel : class where TCreate : class where TUpdate : class
    {        
        Task<TViewModel> GetByIdAsync(Guid id, string[] includes = null);
        Task<int> DeleteAsync(Guid id);
        Task<int> DeleteRangeAsync(List<Guid> ids);
        Task<int> CreateAsync(TCreate create);
        Task<int> UpdateAsync(Guid id, TUpdate update);
    }
}
