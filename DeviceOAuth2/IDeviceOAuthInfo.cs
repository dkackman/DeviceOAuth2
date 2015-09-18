using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceOAuth2
{
    /// <summary>
    /// Basic details of a device OAuth instance
    /// </summary>
    public interface IDeviceOAuthInfo
    {
        /// <summary>
        /// The endpoint of the OAuth2 interface
        /// </summary>
        EndPointInfo EndPoint { get; }

        /// <summary>
        /// The scope(s) being authorized
        /// </summary>
        string Scope { get; }

        /// <summary>
        /// The ClientId requesting authorization
        /// </summary>
        string ClientId { get; }
    }
}
