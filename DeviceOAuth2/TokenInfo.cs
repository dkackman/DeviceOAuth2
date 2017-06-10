using System;

namespace DeviceOAuth2
{
    /// <summary>
    /// Information about an OAuth2 token.
    /// </summary>
    public sealed class TokenInfo
    {
        /// <summary>
        /// The access token used by the app to access the endpoint's assocaited api and authorized scope.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// A token that can be used to refresh the access token. Null if endpoint doesn't support refresh.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// The Expiration time of the access token. Will be DateTime.MaxValue if refresh is unknown or unsupported by the endpoint.
        /// </summary>
        public DateTime Expiry { get; set; } = DateTime.MinValue;

        /// <summary>
        /// The name of the site that the token is associated with.
        /// </summary>
        public string Site { get; set; }

        /// <summary>
        /// The Auth scheme to use with the <see cref="AccessToken"/>.
        /// </summary>
        public string Scheme { get; set; }
    }
}
