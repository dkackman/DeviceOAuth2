﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceOAuth2
{
    /// <summary>
    /// Interface for device based OAuth2 flow
    /// </summary>
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

        /// <summary>
        /// Starts the authentication flow
        /// </summary>
        /// <param name="token">An existing token that can be checked for needing to be refreshed. Pass null if the app has never been authenticated</param>
        /// <returns>An auth token. If the token paramter is still valid it will be returned</returns>
        Task<TokenInfo> Authenticate(TokenInfo token);

        /// <summary>
        /// Starts the authentication flow
        /// </summary>
        /// <param name="token">An existing token that can be checked for needing to be refreshed. Pass null if the app has never been authenticated</param>
        /// <param name="cancelToken">Cancellation token</param>
        /// <returns>An auth token. If the token paramter is still valid it will be returned</returns>
        Task<TokenInfo> Authenticate(TokenInfo token, CancellationToken cancelToken);
    }
}