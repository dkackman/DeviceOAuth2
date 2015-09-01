using System.Threading;
using System.Threading.Tasks;

namespace DeviceOAuth2
{
    public interface IDeviceOAuth2Stepwise
    {
        Task<AuthInfo> BeginAuth();
        Task<AuthInfo> BeginAuth(CancellationToken cancelToken);

        Task<TokenInfo> CheckAuth(AuthInfo info);
        Task<TokenInfo> CheckAuth(AuthInfo info, CancellationToken cancelToken);
    }
}
