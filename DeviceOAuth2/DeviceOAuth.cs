using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;

using DynamicRestProxy.PortableHttpClient;

namespace DeviceOAuth2
{
    /// <summary>
    /// Implementation of device based OAuth2 flow
    /// </summary>
    public class DeviceOAuth : IDeviceOAuth2
    {
        /// <summary>
        /// Event raised when the auth confirmation url and code are known
        /// Display these to the user and tell them to enter the code at the referenced web page
        /// </summary>
        public event EventHandler<AuthInfo> AuthenticatePrompt;

        /// <summary>
        /// Status event raised each time confirmation is checked for
        /// </summary>
        public event EventHandler<int> WaitingForConfirmation;

        private readonly EndPointInfo _endPoint;
        private readonly string _scope;
        private readonly string _clientId;
        private readonly string _clientSecret;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="authEndPoint"></param>
        /// <param name="scope"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        public DeviceOAuth(EndPointInfo authEndPoint, string scope, string clientId, string clientSecret)
        {
            if (authEndPoint == null) throw new ArgumentNullException("authEndPoint");
            if (string.IsNullOrEmpty(scope)) throw new InvalidOperationException("scope cannot be empty");
            if (string.IsNullOrEmpty(clientId)) throw new InvalidOperationException("clientId cannot be empty");

            _endPoint = authEndPoint;
            _scope = scope;
            _clientId = clientId;
            _clientSecret = clientSecret == "" ? null : clientSecret; // we wnat to change emptry string to null so this gets culled form the paramter list
        }

        /// <summary>
        /// Starts the authentication flow
        /// </summary>
        /// <param name="token">An existing token that can be checked for needing to be refreshed. Pass null if the app has never been authenticated</param>
        /// <returns>An auth token. If the token paramter is still valid it will be returned</returns>
        public async Task<TokenInfo> Authenticate(TokenInfo token)
        {
            return await Authenticate(token, CancellationToken.None);
        }

        /// <summary>
        /// Starts the authentication flow
        /// </summary>
        /// <param name="token">An existing token that can be checked for needing to be refreshed. Pass null if the app has never been authenticated</param>
        /// <param name="cancelToken">Cancellation token</param>
        /// <returns>An auth token. If the token paramter is still valid it will be returned</returns>
        public async Task<TokenInfo> Authenticate(TokenInfo token, CancellationToken cancelToken)
        {
            // if the stored token is expired refresh it
            if (token != null)
            {
                if (DateTime.UtcNow >= token.Expiry)
                {
                    return await RefreshAccessToken(token, cancelToken);
                }

                return token;
            }

            // no stored token - go get a new one
            return await GetNewAccessToken(cancelToken);
        }

        private async Task<TokenInfo> RefreshAccessToken(TokenInfo token, CancellationToken cancelToken)
        {
            if (!string.IsNullOrEmpty(token.RefreshToken))
            {
                using (dynamic tokenEndPoint = new DynamicRestClient(_endPoint.AuthUri))
                {
                    var response = await tokenEndPoint(_endPoint.TokenPath).post(cancelToken, client_id: _clientId, client_secret: _clientSecret, refresh_token: token.RefreshToken, grant_type: "refresh_token") as IDictionary<string, object>;

                    return new TokenInfo()
                    {
                        Site = _endPoint.Name,
                        RefreshToken = token.RefreshToken,
                        AccessToken = (string)response["access_token"],
                        Expiry = DateTime.UtcNow + TimeSpan.FromSeconds((long)response["expires_in"])
                    };
                }
            }
            else
            {
                // the token doesn't support refresh so just initiate the new token flow
                return await GetNewAccessToken(cancelToken);
            }
        }

        /// <summary>
        /// This authenticates against user and requires user interaction to authorize the unit test to access apis
        /// This will do the auth, put the auth code on the clipboard and then open a browser with the app auth permission page
        /// The auth code needs to be sent back to google
        /// 
        /// This should only need to be done once because the access token will be stored and refreshed for future test runs
        /// </summary>
        /// <returns></returns>
        private async Task<TokenInfo> GetNewAccessToken(CancellationToken cancelToken)
        {
            // create a connection to the oauth endpoint
            using (dynamic authEndPoint = new DynamicRestClient(_endPoint.AuthUri))
            {
                // this call gets the device code, verification url and user code
                var deviceResponse = await authEndPoint(_endPoint.DevicePath).post(cancelToken, client_id: _clientId, scope: _scope, type: "device_code") as IDictionary<string, object>;

                OnAuthenticatePrompt((string)deviceResponse[_endPoint.VerificationAddressName], (string)deviceResponse["user_code"]);

                long expiration = (long)deviceResponse["expires_in"];
                long interval = (long)deviceResponse["interval"];
                long time = interval;

                string deviceCode = (string)deviceResponse[_endPoint.DeviceCodeName];

                // here poll for success
                while (time < expiration)
                {
                    await Task.Delay((int)interval * 1000);

                    // check the oauth token endpoint ot see if access has been authorized yet
                    using (HttpResponseMessage message = await authEndPoint(_endPoint.TokenPath).post(typeof(HttpResponseMessage), cancelToken, client_id: _clientId, client_secret: _clientSecret, code: deviceCode, type: "device_token", grant_type: "http://oauth.net/grant_type/device/1.0"))
                    {
                        // for some reason facebook returns 400 bad request while waiting for authorization from the user
                        if (message.StatusCode != HttpStatusCode.BadRequest)
                        {
                            message.EnsureSuccessStatusCode();
                        }

                        var tokenResponse = await message.Deserialize<dynamic>() as IDictionary<string, object>;

                        if (tokenResponse.ContainsKey("access_token"))
                        {
                            return new TokenInfo()
                            {
                                Site = _endPoint.Name,
                                AccessToken = (string)tokenResponse["access_token"],
                                RefreshToken = tokenResponse.ContainsKey("refresh_token") ? (string)tokenResponse["refresh_token"] : null,
                                Expiry = tokenResponse.ContainsKey("expires_in") ? DateTime.UtcNow + TimeSpan.FromSeconds((long)tokenResponse["expires_in"]) : DateTime.MaxValue
                            };
                        }
                        else if (tokenResponse.ContainsKey("error"))
                        {
                            var msg = tokenResponse["error"].ToString();
                            if (msg.Contains("denied") || msg.Contains("declined"))
                            {
                                throw new UnauthorizedAccessException("The user denied access");
                            }
                            else if (!msg.Contains("pending"))
                            {
                                throw new InvalidOperationException(msg);
                            }
                        }
                    }
                    time += interval;
                    OnWaitingForConfirmation(expiration - time);
                }

                throw new TimeoutException("Access timeout expired");
            }
        }

        private void OnWaitingForConfirmation(long secondsLeft)
        {
            var e = WaitingForConfirmation;
            if (e != null)
            {
                e.BeginInvoke(this, (int)secondsLeft, null, null);
            }
        }

        private void OnAuthenticatePrompt(string url, string code)
        {
            Debug.WriteLine(code);

            var e = AuthenticatePrompt;
            if (e != null)
            {
                e.BeginInvoke(this, new AuthInfo(url, code), null, null);
            }
        }
    }
}

