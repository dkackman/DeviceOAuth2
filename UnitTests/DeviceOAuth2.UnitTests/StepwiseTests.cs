using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeviceOAuth2.UnitTests
{
    [TestClass]
    public class StepwiseTests
    {
        [TestMethod]
        public async Task FacebookAuthStepwise()
        {
            var keys = TestHelpers.GetAppCredentials("Facebook");
            IDeviceOAuth2Stepwise auth = new DeviceOAuth(EndPointInfo.Facebook, (string)keys.scopes, (string)keys.client_id, (string)keys.client_secret);

            var info = await auth.BeginAuth();

            TestHelpers.SpawnBrowser(info.VerificationUri, info.UserCode);

            var token = await auth.CheckAuth(info);

            Assert.IsNotNull(token);
            Assert.IsFalse(string.IsNullOrEmpty(token.AccessToken));
        }

        [TestMethod]
        public async Task GoogleAuthStepwise()
        {
            var keys = TestHelpers.GetAppCredentials("Google");
            IDeviceOAuth2Stepwise auth = new DeviceOAuth(EndPointInfo.Google, (string)keys.scopes, (string)keys.client_id, (string)keys.client_secret);

            var info = await auth.BeginAuth();

            TestHelpers.SpawnBrowser(info.VerificationUri, info.UserCode);

            var token = await auth.CheckAuth(info);

            Assert.IsNotNull(token);
            Assert.IsFalse(string.IsNullOrEmpty(token.AccessToken));
            Assert.IsFalse(string.IsNullOrEmpty(token.RefreshToken));
        }
    }
}
