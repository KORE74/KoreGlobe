using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEngine;


//  {"PlatWayPoints":{"PlatName":"Red Leader","Count":4,
    // "Legs":[
    //     {"LatDegs":51.1599,"LongDegs":-0.6887,"AltitudeMtrs":3500.0,"GrndSpeedMtrsSec":0.0,"WPType":"Origin"},
    //     {"LatDegs":49.122049,"LongDegs":0.531608,"AltitudeMtrs":3500.0,"GrndSpeedMtrsSec":277.8,"WPType":"linear"},
    //     {"LatDegs":50.292102,"LongDegs":0.403041,"AltitudeMtrs":3500.0,"GrndSpeedMtrsSec":277.8,"WPType":"arc"},
    //     {"LatDegs":51.276886,"LongDegs":-1.384932,"AltitudeMtrs":3500.0,"GrndSpeedMtrsSec":277.8,"WPType":"linear"}]}}


// FssNetworkingManager:FixedUpdate () (at Assets/Code/Unity/FssNetworkingManager.cs:86)


namespace GlobeJSON
{
    public class WayPoint
    {
        [JsonProperty("LatDegs")]
        public double LatDegs { get; set; }

        [JsonProperty("LongDegs")]
        public double LongDegs { get; set; }

        [JsonProperty("AltitudeMtrs")]
        public double AltitudeMtrs { get; set; }

        [JsonProperty("GrndSpeedMtrsSec")]
        public double GrndSpeedMtrsSec { get; set; }

        [JsonProperty("WPType")]
        public string WPType { get; set; }
    }

    public class PlatWayPoints : JSONMessage
    {
        [JsonProperty("PlatName")]
        public string PlatName { get; set; }

        [JsonProperty("Count")]
        public int Count { get; set; }

        [JsonProperty("Legs")]
        public List<WayPoint> Legs { get; set; }

        public GlobeLLA GetPoint(int pntIndex)
        {
            if (pntIndex < 0)              return new GlobeLLA();
            if (pntIndex > Legs.Count - 1) return new GlobeLLA();

            WayPoint currleg = Legs[pntIndex];

            GlobeLLA currPos = new GlobeLLA() {
                LatDegs = currleg.LatDegs,
                LonDegs = currleg.LongDegs,
                AltMslM = currleg.AltitudeMtrs};

            return currPos;
        }

        public static PlatWayPoints ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken JsonContent = messageObj.GetValue("PlatWayPoints");

                Debug.Log("PlatWayPoints -> JsonContent OK");

                if (JsonContent != null)
                {
                    string readPlatName  = (string)JsonContent["PlatName"];
                    int    readCount     = (int)JsonContent["Count"];
                    var    legs          = JsonContent["Legs"].ToObject<List<WayPoint>>();

                    Debug.Log($"PlatWayPoints -> PlatName = {readPlatName}");
                    Debug.Log($"PlatWayPoints -> Count = {readCount}");



                    PlatWayPoints newMsg = new PlatWayPoints
                    {
                        PlatName = readPlatName,
                        Count = readCount,
                        Legs = legs
                    };

                    // quick validation that the message data lines up
                    if (newMsg != null && newMsg.Legs != null && newMsg.Count == newMsg.Legs.Count)
                        return newMsg;

                    return null;
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






