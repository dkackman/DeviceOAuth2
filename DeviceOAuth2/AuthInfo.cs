namespace DeviceOAuth2
{
    public class AuthInfo
    {
        public string VerificationUrl { get; private set; }

        public string UserCode { get; private set; }

        public AuthInfo(string url, string code)
        {
            VerificationUrl = url;
            UserCode = code;
        }
    }
}
