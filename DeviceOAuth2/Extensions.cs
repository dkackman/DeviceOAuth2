using System;
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
        /// The single place to diseect the response from the polling rest method to 
        /// figure out if there is an error message otehr than indicating it is still pending user authorization
        /// </summary>
        /// <param name="d"></param>
        public static void ThrowIfError(this IDictionary<string, object> d)
        {
            if (d.ContainsKey("error"))
            {
                var msg = d.GetErrorMessage();
                if (msg.Contains("denied") || msg.Contains("declined"))
                {
                    throw new UnauthorizedAccessException("The user denied access");
                }
                else if (!msg.Contains("pending"))
                {
                    throw new InvalidOperationException(msg);
                }
            }
        }

        /// <summary>
        /// This is to deal with the different ways that facebook and good encode the content of the polling method
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        static string GetErrorMessage(this IDictionary<string, object> d)
        {
            Debug.Assert(d.ContainsKey("error"));

            // google returns name value pair where value is a string {error, "message"}
            object o = d["error"];
            var s = o as string;
            if (s != null)
            {
                return s;
            }

            // google returns name value pair where value is an object with a property of message {error, {message, "message"}}
            var expando = o as IDictionary<string, object>;
            if (expando != null && expando.ContainsKey("message"))
            {
                return (string)expando["message"];
            }

            return "unkown error";
        }
    }
}
