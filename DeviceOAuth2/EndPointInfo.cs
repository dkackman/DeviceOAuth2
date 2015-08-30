using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceOAuth2
{
    public class EndPointInfo
    {
        public string AuthUrl { get; private set; }

        public string TokenPath { get; private set; }

        public string DevicePath { get; private set; }

        public string VerificationAddressName { get; private set; }

        public string DeviceCodeName { get; private set; }

        public string Name { get; private set; }

        public static EndPointInfo Google
        {
            get
            {
                return new EndPointInfo()
                {
                    Name = "Google",
                    AuthUrl = "https://accounts.google.com/o/oauth2/",
                    TokenPath = "token/",
                    DevicePath = "device/code/",
                    VerificationAddressName = "verification_url",
                    DeviceCodeName = "device_code"
                };
            }
        }

        public static EndPointInfo Facebook
        {
            get
            {
                return new EndPointInfo()
                {
                    Name = "Facebook",
                    AuthUrl = "https://graph.facebook.com/oauth/",
                    TokenPath = "device/",
                    DevicePath = "device/",
                    VerificationAddressName = "verification_uri",
                    DeviceCodeName = "code"
                };
            }
        }
    }
}
