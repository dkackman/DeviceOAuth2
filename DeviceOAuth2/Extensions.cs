using System.IO;
using System.Net.Http;
using System.Dynamic;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DeviceOAuth2
{
    static class Extensions
    {
        /// <summary>
        /// This is to deal with the different ways that facebook and good encode the content of the polling method
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string GetErrorMessage(this IDictionary<string,object> d)
        {
            Debug.Assert(d.ContainsKey("error"));

            object o = d["error"];
            var s = o as string;
            if (s != null)
            {
                return s;
            }

            var expando = o as IDictionary<string,object>;
            if(expando != null && expando.ContainsKey("message"))
            {
                return (string)expando["message"];
            }

            return "";
        }
    }
}
