using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeviceOAuth2.UnitTests
{
    [TestClass]
    public class AuthTests
    {
        [TestMethod]
        public async Task FacebookAuth()
        {
            var keys = TestHelpers.GetAppCredentials("Facebook");
            IDeviceOAuth2 auth = new DeviceOAuth(EndPointInfo.Facebook, (string)keys.scopes, (string)keys.client_id, (string)keys.client_secret);

            auth.AuthenticatePrompt += (o, e) =>
            {
                TestHelpers.SpawnBrowser(e.VerificationUri, e.UserCode);
            };

            var token = await auth.Authenticate(null);

            Assert.IsNotNull(token);
            Assert.IsFalse(string.IsNullOrEmpty(token.AccessToken));
        }

        [TestMethod]
        public async Task GoogleAuth()
        {
            var keys = TestHelpers.GetAppCredentials("Google");
            IDeviceOAuth2 auth = new DeviceOAuth(EndPointInfo.Google, (string)keys.scopes, (string)keys.client_id, (string)keys.client_secret);

            auth.AuthenticatePrompt += (o, e) =>
            {
                TestHelpers.SpawnBrowser(e.VerificationUri, e.UserCode);
            };

            var token = await auth.Authenticate(null);

            Assert.IsNotNull(token);
            Assert.IsFalse(string.IsNullOrEmpty(token.AccessToken));
            Assert.IsFalse(string.IsNullOrEmpty(token.RefreshToken));
        }
    }
}
