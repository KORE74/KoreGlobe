using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{
    public class AppShutdown : JSONMessage
    {

        public AppShutdown()
        {

        }

        // -----------------------

        public static AppShutdown ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken JsonToken = messageObj.GetValue("AppShutdown");
                if (JsonToken != null)
                {
                    AppShutdown newMsg = new AppShutdown();

                    return newMsg;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

    } // end class
} // end namespace






