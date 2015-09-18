using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;

using DynamicRestProxy.PortableHttpClient;

using Newtonsoft.Json;

namespace DeviceOAuth2
{
    /// <summary>
    /// Implementation of device based OAuth2 flow
    /// </summary>
    public class DeviceOAuth : IDeviceOAuth2, IDeviceOAuth2Stepwise
    {
        /// <summary>
        /// Event raised when the auth confirmation url and code are known
        /// Display these to the user and tell them to enter the code at the referenced web page
        /// </summary>
        public event EventHandler<AuthInfo> PromptUser;

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
        public DeviceOAuth(EndPointInfo authEndPoint, string scope, string clientId)
            : this(authEndPoint, scope, clientId, null)
        {
        }

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
            _clientSecret = clientSecret == "" ? null : clientSecret; // we want to change empty string to null so it gets culled form the paramrter list
        }

        /// <summary>
        /// The endpoint of the OAuth2 interface
        /// </summary>
        public EndPointInfo EndPoint { get { return _endPoint; } }

        /// <summary>
        /// The scope(s) being authorized
        /// </summary>
        public string Scope { get { return _scope; } }

        /// <summary>
        /// The ClientId requesting authorization
        /// </summary>
        public string ClientId { get { return _clientId; } }

        /// <summary>
        /// Starts the authorization flow
        /// </summary>
        /// <param name="token">An existing token that can be checked for needing to be refreshed. Pass null if the app has never been authorized</param>
        /// <returns>An auth token. If the token paramter is still valid it will be returned</returns>
        public async Task<TokenInfo> Authorize(TokenInfo token)
        {
            return await Authorize(token, CancellationToken.None);
        }

        /// <summary>
        /// Starts the authorization flow
        /// </summary>
        /// <param name="token">An existing token that can be checked for needing to be refreshed. Pass null if the app has never been authorized</param>
        /// <param name="cancelToken">Cancellation token</param>
        /// <returns>An auth token. If the token parameter is still valid it will be returned</returns>
        public async Task<TokenInfo> Authorize(TokenInfo token, CancellationToken cancelToken)
        {
            if (token != null)
            {
                // if the stored token is expired refresh it
                if (DateTime.UtcNow >= token.Expiry)
                {
                    if (!string.IsNullOrEmpty(token.RefreshToken))
                    {
                        return await RefreshAccessToken(token, cancelToken);
                    }
                    else
                    {
                        // the token doesn't support refresh so just initiate the new token flow
                        return await GetNewAccessToken(cancelToken);
                    }
                }

                return token; // this token is still valid just pass it back
            }

            // no stored token - go get a new one
            return await GetNewAccessToken(cancelToken);
        }


        /// <summary>
        /// Checks the validity of a token against the auth endpoint.
        /// It does this by makeing a get request to the token's <see cref="EndPointInfo.CheckUri"/>
        /// This is useful for ensuring that the user hasn't revoked authorization for a stored token and that it hasn't expired
        /// </summary>
        /// <param name="token">The token to check</param>
        public async Task<bool> CheckToken(TokenInfo token)
        {
            return await CheckToken(token, CancellationToken.None);
        }

        /// <summary>
        /// Checks the validity of a token against the auth endpoint.
        /// It does this by makeing a get request to the token's <see cref="EndPointInfo.CheckUri"/>
        /// This is useful for ensuring that the user hasn't revoked authorization for a stored token and that it hasn't expired
        /// </summary>
        /// <param name="token">The token to check</param>
        /// <param name="cancelToken">A cancellation token</param>
        /// <returns></returns>
        public async Task<bool> CheckToken(TokenInfo token, CancellationToken cancelToken)
        {
            if (token == null) throw new ArgumentNullException("token");

            try
            {
                var defaults = new DynamicRestClientDefaults()
                {
                    AuthScheme = EndPoint.Scheme,
                    AuthToken = token.AccessToken
                };

                using (dynamic checkEndpoint = new DynamicRestClient(_endPoint.CheckUri, defaults))
                {
                    var response = await checkEndpoint.get(cancelToken);
                    return response != null;
                }
            }
            catch (AggregateException e)
            {
                foreach (var ex in e.InnerExceptions)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return false;
        }

        private async Task<TokenInfo> RefreshAccessToken(TokenInfo token, CancellationToken cancelToken)
        {
            if (token == null) throw new ArgumentNullException("token");
            if (string.IsNullOrEmpty(token.RefreshToken)) throw new InvalidOperationException("Token is not refreshable");

            using (dynamic tokenEndPoint = new DynamicRestClient(_endPoint.AuthUri))
            {
                var response = await tokenEndPoint(_endPoint.TokenPath).post(cancelToken, client_id: _clientId, client_secret: _clientSecret, refresh_token: token.RefreshToken, grant_type: "refresh_token") as IDictionary<string, object>;

                return new TokenInfo()
                {
                    Site = _endPoint.Name,
                    RefreshToken = token.RefreshToken,
                    AccessToken = (string)response["access_token"],
                    Expiry = DateTime.UtcNow + TimeSpan.FromSeconds((long)response["expires_in"]),
                    Scheme = _endPoint.Scheme
                };
            }
        }

        private async Task<TokenInfo> GetNewAccessToken(CancellationToken cancelToken)
        {
            var authInfo = await GetDeviceCode(cancelToken);

            OnPromptUser(authInfo);

            return await PollForUserAuth(authInfo, cancelToken);
        }

        private async Task<AuthInfo> GetDeviceCode(CancellationToken cancelToken)
        {
            using (dynamic authEndPoint = new DynamicRestClient(_endPoint.AuthUri))
            {
                // this call gets the device code, verification url and user code
                var deviceResponse = await authEndPoint(_endPoint.DevicePath).post(cancelToken, client_id: _clientId, scope: _scope, type: "device_code") as IDictionary<string, object>;

                long expiration = (long)deviceResponse["expires_in"];
                long interval = (long)deviceResponse["interval"];

                return new AuthInfo()
                {
                    PollInterval = (int)interval,
                    Timestamp = DateTimeOffset.UtcNow,
                    Expiration = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(expiration),
                    DeviceCode = (string)deviceResponse[_endPoint.DeviceCodeName],
                    VerificationUri = (string)deviceResponse[_endPoint.VerificationAddressName],
                    UserCode = (string)deviceResponse["user_code"]
                };
            }
        }

        private async Task<TokenInfo> PollForUserAuth(AuthInfo authInfo, CancellationToken cancelToken)
        {
            if (authInfo == null) throw new ArgumentNullException("authInfo");

            using (dynamic authEndPoint = new DynamicRestClient(_endPoint.AuthUri))
            {
                // here poll for success
                while (DateTimeOffset.UtcNow < authInfo.Expiration)
                {
                    await Task.Delay(authInfo.PollInterval * 1000);

                    // check the oauth token endpoint ot see if access has been authorized yet
                    using (HttpResponseMessage message = await authEndPoint(_endPoint.TokenPath).post(typeof(HttpResponseMessage), cancelToken, client_id: _clientId, client_secret: _clientSecret, code: authInfo.DeviceCode, type: "device_token", grant_type: "http://oauth.net/grant_type/device/1.0"))
                    {
                        // for some reason facebook returns 400 bad request while waiting for authorization from the user
                        if (message.StatusCode != HttpStatusCode.BadRequest)
                        {
                            message.EnsureSuccessStatusCode();
                        }

                        var tokenResponse = await message.Deserialize<dynamic>(new JsonSerializerSettings()) as IDictionary<string, object>;

                        if (tokenResponse.ContainsKey("access_token"))
                        {
                            return new TokenInfo()
                            {
                                Site = _endPoint.Name,
                                AccessToken = (string)tokenResponse["access_token"],
                                RefreshToken = tokenResponse.ContainsKey("refresh_token") ? (string)tokenResponse["refresh_token"] : null,
                                Expiry = tokenResponse.ContainsKey("expires_in") ? DateTime.UtcNow + TimeSpan.FromSeconds((long)tokenResponse["expires_in"]) : DateTime.MaxValue,
                                Scheme = _endPoint.Scheme
                            };
                        }

                        tokenResponse.ThrowIfError();

                        // if we made it this far it inidcates that the rest endpoint is still in a pending state, so go around again
                    }

                    OnWaitingForConfirmation((int)(authInfo.Expiration - DateTimeOffset.UtcNow).TotalSeconds);
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

        private void OnPromptUser(AuthInfo info)
        {
            Debug.WriteLine(info.UserCode);

            var e = PromptUser;
            if (e != null)
            {
                e.BeginInvoke(this, info, null, null);
            }
        }

        async Task<AuthInfo> IDeviceOAuth2Stepwise.StartAuthorization()
        {
            return await GetDeviceCode(CancellationToken.None);
        }
        async Task<AuthInfo> IDeviceOAuth2Stepwise.StartAuthorization(CancellationToken cancelToken)
        {
            return await GetDeviceCode(cancelToken);
        }

        async Task<TokenInfo> IDeviceOAuth2Stepwise.WaitForUserConsent(AuthInfo info)
        {
            return await PollForUserAuth(info, CancellationToken.None);
        }
        async Task<TokenInfo> IDeviceOAuth2Stepwise.WaitForUserConsent(AuthInfo info, CancellationToken cancelToken)
        {
            return await PollForUserAuth(info, cancelToken);
        }

        async Task<TokenInfo> IDeviceOAuth2Stepwise.RefreshAccessToken(TokenInfo token)
        {
            return await RefreshAccessToken(token, CancellationToken.None);
        }
        async Task<TokenInfo> IDeviceOAuth2Stepwise.RefreshAccessToken(TokenInfo token, CancellationToken cancelToken)
        {
            return await RefreshAccessToken(token, cancelToken);
        }
    }
}