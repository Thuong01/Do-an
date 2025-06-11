using Datas.Data;

namespace Datas.Infrastructures.Interfaces
{
    public interface IDbFactory : IDisposable
    {
        CameraNowContext Init();
    }
}
