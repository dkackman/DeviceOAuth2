using System;

namespace DeviceOAuth2
{
    /// <summary>
    /// Return data from the initial auth request. This includes the <see cref="VerificationUri"/> and <see cref="DeviceCode"/> 
    /// that the user needs to enter in order to authorize the request.
    /// </summary>
    public sealed class AuthInfo
    {
        /// <summary>
        /// The time, in seconds, to pause between polls to see if the user has authorized the code.
        /// </summary>
        public int PollInterval { get; internal set; }

        /// <summary>
        /// The device code that represents the current authorization instance.
        /// </summary>
        public string DeviceCode { get; internal set; }

        /// <summary>
        /// The UTC timestamp of when the authorization process was started
        /// </summary>
        public DateTimeOffset Timestamp { get; internal set; }

        /// <summary>
        /// The UTC timestamp of when the authorization process will expire if the user does not grant or deny access.
        /// </summary>
        public DateTimeOffset Expiration { get; internal set; }

        /// <summary>
        /// The Uri the user needs to navigate to and enter the <see cref="UserCode"/>.
        /// </summary>
        public string VerificationUri { get; internal set; }

        /// <summary>
        /// The authorization code that the user needs to enter at <see cref="VerificationUri"/>.
        /// </summary>
        public string UserCode { get; internal set; }
    }
}
