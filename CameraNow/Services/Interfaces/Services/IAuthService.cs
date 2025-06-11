using Datas.Extensions.Responses;
using Datas.ViewModels.Auth;

namespace Services.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResultViewModel> LoginAsync(string UserName, string Password);
        Task<ResponseMessage> RegisterAsync(RegisterViewModel model);
        Task<ResponseMessage> ForgetPasword(string email);
    }
}
