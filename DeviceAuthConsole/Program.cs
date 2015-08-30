using System;
using System.Threading.Tasks;
using System.Diagnostics;

using DeviceOAuth2;

using DynamicRestProxy.PortableHttpClient;

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
            IDeviceOAuth2 auth = new DeviceOAuth(EndPointInfo.Facebook, keys.Item1, keys.Item2, keys.Item3);

            auth.WaitingForConfirmation += (o, e) =>
            {
                Console.CursorLeft = 0;
                Console.Write(e + " seconds left");
            };
            auth.AuthenticatePrompt += (o, e) =>
            {
                Console.WriteLine("");
                Console.WriteLine("Go to this url on any computer:");
                Console.WriteLine(e.VerificationUrl);
                Console.WriteLine("And enter this code:");
                Console.WriteLine(e.UserCode);
                Console.WriteLine("");
            };

            Console.WriteLine("Authenticating...");

            try
            {
                var token = await auth.Authenticate(null);

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

        static Tuple<string, string, string> GetAppCredentials(string name)
        {
            return null;
        }
    }
}

