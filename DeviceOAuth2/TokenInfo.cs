using System;

namespace DeviceOAuth2
{
    /// <summary>
    /// Information about the Auth token
    /// </summary>
    public class TokenInfo
    {
        /// <summary>
        /// The access token itself
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// A token that can be used to refresh the access token. Null if endpoint doesn't support refresh
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// The Expiration time of the access token. Will be DateTime.MaxValue if refresh is unknown or unsupported by teh endpoint
        /// </summary>
        public DateTime Expiry { get; set; } = DateTime.MinValue;

        /// <summary>
        /// The site that the token is associated with
        /// </summary>
        public string Site { get; set; }
    }
}
