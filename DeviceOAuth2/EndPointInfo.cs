namespace DeviceOAuth2
{
    /// <summary>
    /// Properties describing a device Oauth endpoint
    /// </summary>
    public sealed class EndPointInfo
    {
        /// <summary>
        /// The base Uri for the endpoint
        /// </summary>
        public string AuthUri { get; set; }

        /// <summary>
        /// The uri path to get a token - relative to <see cref="AuthUri"/>
        /// </summary>
        public string TokenPath { get; set; }

        /// <summary>
        /// The uri path to request a device code - relative to <see cref="AuthUri"/>
        /// </summary>
        public string DevicePath { get; set; }

        /// <summary>
        /// The property name of the token return json that has the verification Uri
        /// </summary>
        public string VerificationAddressName { get; set; }

        /// <summary>
        /// The name of the paramter that describes the code
        /// </summary>
        public string DeviceCodeName { get; set; }

        /// <summary>
        /// The name of the endpoint
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Auth scheme used by the resulting <see cref="TokenInfo"/>
        /// </summary>
        public string Scheme { get; set; }

        /// <summary>
        /// Uri that will return the user profile for the auth endpoint once authorized
        /// </summary>
        public string ProfileUri { get; set; }

        /// <summary>
        /// Description of Google's device oauth2 endpoint 
        /// https://developers.google.com/identity/protocols/OAuth2ForDevices
        /// </summary>
        public static EndPointInfo Google
        {
            get
            {
                return new EndPointInfo()
                {
                    Name = "Google",
                    AuthUri = "https://accounts.google.com/o/oauth2/",
                    TokenPath = "token/",
                    DevicePath = "device/code/",
                    VerificationAddressName = "verification_url",
                    DeviceCodeName = "device_code",
                    Scheme = "OAuth",
                    ProfileUri = "https://www.googleapis.com/oauth2/v1/userinfo"
                };
            }
        }

        /// <summary>
        /// Description of Facebook's oauth2 endpoint
        /// https://developers.facebook.com/docs/facebook-login/for-devices
        /// </summary>
        public static EndPointInfo Facebook
        {
            get
            {
                return new EndPointInfo()
                {
                    Name = "Facebook",
                    AuthUri = "https://graph.facebook.com/oauth/",
                    TokenPath = "device/",
                    DevicePath = "device/",
                    VerificationAddressName = "verification_uri",
                    DeviceCodeName = "code",
                    Scheme = "Bearer",
                    ProfileUri = "https://graph.facebook.com/v2.4/me?fields=name,picture"
                };
            }
        }
    }
}
