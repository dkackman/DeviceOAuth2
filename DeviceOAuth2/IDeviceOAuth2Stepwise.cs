using System.Threading;
using System.Threading.Tasks;

namespace DeviceOAuth2
{    
    /// <summary>
     /// Interface for device based OAuth2 flow that does not use event callbacks
     /// </summary>
    public interface IDeviceOAuth2Stepwise : IDeviceOAuthInfo
    {
        /// <summary>
        /// Begins the authorization workflow by getting a device and user code from the endpoint
        /// </summary>
        /// <returns>An <see cref="AuthInfo"/>. The user should be presented the VerificationUri and UserCode as well as instruction. 
        /// This object should then be passed to <see cref="WaitForUserConsent(AuthInfo)"/></returns>
        Task<AuthInfo> StartAuthorization();

        /// <summary>
        /// Begins the authorization workflow by getting a device and user code from the endpoint
        /// </summary>
        /// <param name="cancelToken">A <see cref="CancellationToken"/></param>
        /// <returns>An <see cref="AuthInfo"/>. The user should be presented the VerificationUri and UserCode as well as instruction. 
        /// This object should then be passed to <see cref="WaitForUserConsent(AuthInfo, CancellationToken)"/></returns>
        Task<AuthInfo> StartAuthorization(CancellationToken cancelToken);

        /// <summary>
        /// Polls the oauth endpoint until:
        /// 1) The user authorizes acess
        /// 2) The user denies access
        /// 3) The authorization times out (endpoint specific)
        /// </summary>
        /// <param name="info">The <see cref="AuthInfo"/> returned from <see cref="StartAuthorization()"/></param>
        /// <returns>A <see cref="TokenInfo"/> that contains the Access Token</returns>
        Task<TokenInfo> WaitForUserConsent(AuthInfo info);

        /// <summary>
        /// Polls the oauth endpoint until:
        /// 1) The user authorizes acess
        /// 2) The user denies access
        /// 3) The authorization times out (endpoint specific)
        /// </summary>
        /// <param name="info">The <see cref="AuthInfo"/> returned from <see cref="StartAuthorization(CancellationToken)"/></param>
        /// <param name="cancelToken">A <see cref="CancellationToken"/></param>
        /// <returns>A <see cref="TokenInfo"/> that contains the Access Token</returns>
        Task<TokenInfo> WaitForUserConsent(AuthInfo info, CancellationToken cancelToken);

        /// <summary>
        /// Refreshes aan access token if supported
        /// </summary>
        /// <param name="token">The token to refresh</param>
        /// <returns>Refreshed token</returns>
        Task<TokenInfo> RefreshAccessToken(TokenInfo token);

        /// <summary>
        /// Refreshes aan access token if supported
        /// </summary>
        /// <param name="token">The token to refresh</param>
        /// <param name="cancelToken">A <see cref="CancellationToken"/></param>
        /// <returns>Refreshed token</returns>
        Task<TokenInfo> RefreshAccessToken(TokenInfo token, CancellationToken cancelToken);
    }
}
