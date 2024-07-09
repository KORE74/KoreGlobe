using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{
    public class NullMsg : JSONMessage
    {

        public static NullMsg ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken JsonContent = messageObj.GetValue("NullMsg");
                if (JsonContent != null)
                {
                    NullMsg newMsg = new NullMsg();
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






