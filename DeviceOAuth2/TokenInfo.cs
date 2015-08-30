using System;

namespace DeviceOAuth2
{
    public class TokenInfo
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime Expiry { get; set; } = DateTime.MinValue;

        public string Site { get; set; }
    }
}
