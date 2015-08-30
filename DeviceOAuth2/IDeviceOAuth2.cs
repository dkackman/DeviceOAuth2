using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceOAuth2
{
    public interface IDeviceOAuth2
    {
        /// <summary>
        /// Event raised when the auth confirmation url and code are known
        /// Display these to the user and tell them to enter the code at the referenced web page
        /// </summary>
        event EventHandler<AuthInfo> AuthenticatePrompt;

        /// <summary>
        /// Status event raised each time confirmation is checked for
        /// </summary>
        event EventHandler<int> WaitingForConfirmation;

        Task<TokenInfo> Authenticate(TokenInfo token);

        Task<TokenInfo> Authenticate(TokenInfo token, CancellationToken cancelToken);
    }
}
