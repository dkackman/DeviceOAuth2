namespace DeviceOAuth2
{
    /// <summary>
    /// Return data from the initial auth request
    /// </summary>
    public class AuthInfo
    {
        /// <summary>
        /// The Uti the user needs to navigate to and enter the user code
        /// </summary>
        public string VerificationUri { get; private set; }

        /// <summary>
        /// The authorization code
        /// </summary>
        public string UserCode { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="url"></param>
        /// <param name="code"></param>
        public AuthInfo(string url, string code)
        {
            VerificationUri = url;
            UserCode = code;
        }
    }
}
