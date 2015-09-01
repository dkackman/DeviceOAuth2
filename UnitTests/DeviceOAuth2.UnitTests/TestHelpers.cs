using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DeviceOAuth2.UnitTests
{
    class TestHelpers
    {
        public static dynamic GetAppCredentials(string name)
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("DeviceOAuth2.UnitTests.keys.json")))
            {
                var s = reader.ReadToEnd();

                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new ExpandoObjectConverter());

                var keys = JsonConvert.DeserializeObject<List<dynamic>>(s, settings);
                return keys.First(d => d.name == name);
            }
        }

        public static void SpawnBrowser(string verificationUri, string userCode)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = string.Format("/c echo {0}| clip", userCode.Trim());
            p.Start();

            // this requires user permission - open a broswer - enter the user_code which is now in the clipboard
            Process.Start(verificationUri);
        }
    }
}
