using System;

namespace DeviceOAuth2
{
    /// <summary>
    /// Return data from the initial auth request
    /// </summary>
    public class AuthInfo
    {
        public int PollInterval { get; internal set; }

        public string DeviceCode { get; internal set; }

        public DateTimeOffset Timestamp { get; internal set; }

        public DateTimeOffset Expiration { get; internal set; }

        /// <summary>
        /// The Uti the user needs to navigate to and enter the user code
        /// </summary>
        public string VerificationUri { get; internal set; }

        /// <summary>
        /// The authorization code
        /// </summary>
        public string UserCode { get; internal set; }
    }
}
