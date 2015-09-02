using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using DeviceOAuth2;

using DynamicRestProxy.PortableHttpClient;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DeviceAuthConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var task = Go();
            task.Wait();

            Console.WriteLine("\n\nPress any key to continue...");
            Console.ReadKey();
        }

        private static async Task Go()
        {
            var keys = GetAppCredentials("Facebook");
            IDeviceOAuth2 auth = new DeviceOAuth(EndPointInfo.Facebook, (string)keys.scopes, (string)keys.client_id, (string)keys.client_secret);

            auth.WaitingForConfirmation += (o, e) =>
            {
                Console.CursorLeft = 0;
                Console.Write(e + " seconds left");
            };
            auth.PromptUser += (o, e) =>
            {
                Console.WriteLine("");
                Console.WriteLine("Go to this url on any computer:");
                Console.WriteLine(e.VerificationUri);
                Console.WriteLine("And enter this code:");
                Console.WriteLine(e.UserCode);
                Console.WriteLine("");
            };

            Console.WriteLine("Authenticating...");

            try
            {
                var token = await auth.Authorize(null);

                await ShowUserProfile(token);
            }
            catch (AggregateException e)
            {
                Console.WriteLine("Error:");
                foreach (var inner in e.InnerExceptions)
                {
                    Console.WriteLine(inner.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:");
                Console.WriteLine(e.Message);
            }
        }

        private static async Task ShowUserProfile(TokenInfo token)
        {
            Console.WriteLine("");
            Debug.Assert(!string.IsNullOrEmpty(token.AccessToken));

            if (token.Site == "Google")
            {
                // using a DynamicRestClient for this example - any old rest client would work
                var defaults = new DynamicRestClientDefaults()
                {
                    AuthScheme = "OAuth",
                    AuthToken = token.AccessToken
                };

                dynamic client = new DynamicRestClient("https://www.googleapis.com/oauth2/v1/userinfo", defaults);

                dynamic v = await client.get();

                Console.WriteLine("Name = " + v.name);
            }
            else if (token.Site == "Facebook")
            {
                // using a DynamicRestClient for this example - any old rest client would work
                var defaults = new DynamicRestClientDefaults()
                {
                    AuthScheme = "Bearer",
                    AuthToken = token.AccessToken
                };

                dynamic client = new DynamicRestClient("https://graph.facebook.com/v2.3/me", defaults);

                dynamic v = await client.get(fields: "name");

                Console.WriteLine("Name = " + v.name);
            }
        }

        static dynamic GetAppCredentials(string name)
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("DeviceAuthConsole.keys.json")))
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