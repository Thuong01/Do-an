using Models.Enums;

namespace Datas.ViewModels.Auth
{
    public class AppUserSpecification : BaseSpecification
    {
        public AppUserSpecification(string? filter, Status? status, string? sorting) : base(filter, status, sorting)
        {
        }
    }
}
