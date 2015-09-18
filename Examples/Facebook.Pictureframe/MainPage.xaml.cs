using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using DeviceOAuth2;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using DynamicRestProxy.PortableHttpClient;
using Windows.Storage;

namespace Facebook.Pictureframe
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Authorize();
        }

        private async Task Authorize()
        {
            dynamic keys = await GetAppCredentials("Facebook");
            string clientId = keys.client_id;

            IDeviceOAuth2Stepwise auth = new DeviceOAuth(EndPointInfo.Facebook, "public_profile", clientId);
            var info = await auth.StartAuthorization();
            
            var msg = $"Navigate to {info.VerificationUri} \nEnter this code: {info.UserCode}";
            Notify.Text = msg;

            var token = await auth.WaitForUserConsent(info);

            var defaults = new DynamicRestClientDefaults()
            {
                AuthScheme = "Bearer",
                AuthToken = token.AccessToken
            };

            dynamic client = new DynamicRestClient("https://graph.facebook.com/v2.3/me", defaults);

            dynamic v = await client.get(fields: "name");
            Notify.Text = "";
            UserName.Text = v.name;
        }

        static async Task<dynamic> GetAppCredentials(string name)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///keys.json"));
            using (Stream stream = (await file.OpenReadAsync()).AsStreamForRead())
            using (var reader = new StreamReader(stream))
            {
                var s = reader.ReadToEnd();

                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new ExpandoObjectConverter());

                var keys = JsonConvert.DeserializeObject<List<dynamic>>(s, settings);
                return keys.First(d => d.name == name);
            }
        }
    }
}
