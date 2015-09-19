using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using DeviceOAuth2;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DeviceAuthConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            int choice = 0;

            while (!(choice == 1 || choice == 2))
            {
                Console.WriteLine("Choose the endpoint to authorize:");
                Console.WriteLine("\t1) Google");
                Console.WriteLine("\t2) Facebook");

                var line = Console.ReadLine();
                int.TryParse(line, out choice);
            }

            var endpoint = choice == 1 ? EndPointInfo.Google : EndPointInfo.Facebook;

            var task = Go(endpoint);
            task.Wait();

            Console.WriteLine("\n\nPress any key to continue...");
            Console.ReadKey();
        }

        private static async Task Go(EndPointInfo endpoint)
        {
            var keys = GetAppCredentials(endpoint.Name);
            IDeviceOAuth2 auth = new DeviceOAuth(endpoint, (string)keys.scopes, (string)keys.client_id, (string)keys.client_secret);

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

                dynamic profile = await auth.GetProfile(token);

                Console.WriteLine("");
                Console.WriteLine("Name = " + profile.name);
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