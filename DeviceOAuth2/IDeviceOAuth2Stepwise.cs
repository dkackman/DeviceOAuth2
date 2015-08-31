using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceOAuth2
{
    public interface IDeviceOAuth2Stepwise
    {
        Task<AuthInfo> BeginAuth();

        Task<TokenInfo> CheckAuth(AuthInfo info);
    }
}
