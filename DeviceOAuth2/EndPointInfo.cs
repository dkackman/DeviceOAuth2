﻿namespace DeviceOAuth2
{
    /// <summary>
    /// Properties describing a device Oauth endpoint
    /// </summary>
    public class EndPointInfo
    {
        /// <summary>
        /// The base Uri for the endpoint
        /// </summary>
        public string AuthUri { get; set; }

        /// <summary>
        /// The uri path to get a token
        /// </summary>
        public string TokenPath { get; set; }

        /// <summary>
        /// The uri path to request a device code
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
        /// Description of Google's device auth endpoint
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
                    DeviceCodeName = "device_code"
                };
            }
        }

        /// <summary>
        /// Description of Facebook's auth endpoint
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
                    DeviceCodeName = "code"
                };
            }
        }
    }
}