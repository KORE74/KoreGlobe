using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{
    public class AppCameraControl : JSONMessage
    {
        [JsonProperty("DisplayAction1")]
        public string DisplayAction1 { get; set; }

        [JsonProperty("DisplayAction2")]
        public string DisplayAction2 { get; set; }

        [JsonProperty("PlatformName1")]
        public string PlatformName1 { get; set; }

        [JsonProperty("PlatformName2")]
        public string PlatformName2 { get; set; }

        [JsonProperty("DistKm")]
        public double DistKm { get; set; }

        [JsonProperty("AzAngDegs")]
        public double AzAngDegs { get; set; }

        [JsonProperty("AltOffsetKm")]
        public double AltOffsetKm { get; set; }

        [JsonProperty("DurationSecs")]
        public double DurationSecs { get; set; }

        public AppCameraControl()
        {
            DisplayAction1 = "NoAction";
            DisplayAction2 = "NoAction";
            PlatformName1  = "NoPlatform";
            PlatformName2  = "NoPlatform";
            DistKm         = 1;
            AzAngDegs      = 0;
            AltOffsetKm    = 0;
            DurationSecs   = 0;
        }

        // -----------------------

        public void SetupSimpleFlyCam(string targetplatform)
        {
            DisplayAction1 = "SimpleFlyCam";
            PlatformName1 = targetplatform;
            DurationSecs = 5; // Flight time

            // Setup default values to rotate around platform
            DisplayAction2 = "Rotate";
            DistKm = 0.5;
            AzAngDegs = 0;
            AltOffsetKm = 0.05;
        }

        public bool IsSimpleFlyCam()
        {
            return (DisplayAction1 == "SimpleFlyCam");
        }

        // -----------------------

        public void SetupFlyToAlignCam(string neartargetplatform, string fartargetplatform)
        {
            DisplayAction1 = "FlyToAlignCam";
            PlatformName1 = neartargetplatform;
            PlatformName2 = fartargetplatform;

            DistKm      = 0.05;
            AzAngDegs   = 0;
            AltOffsetKm = 0.01;
        }

        public bool IsFlyToAlignCam()
        {
            return (DisplayAction1 == "FlyToAlignCam");
        }

        // -----------------------

        public void SetupTrackCam(string targetplatform, string aimPlatform)
        {
            // Setup motion to tracking position
            PlatformName1  = targetplatform;
            DisplayAction1 = "TrackCam";
            DurationSecs   = 5; // Flight time

            // Setup default values to rotate around platform
            DisplayAction2 = "TrackCam";
            PlatformName2  = aimPlatform;
            DistKm         = 0.5;
            AzAngDegs      = 0;
            AltOffsetKm    = 20;
        }

        public static AppCameraControl ParseJSON(string json)
        {
            try
            {
                JObject messageObj = JObject.Parse(json);
                JToken JsonToken = messageObj.GetValue("AppCameraControl");
                if (JsonToken != null)
                {
                    AppCameraControl newMsg = new AppCameraControl();

                    newMsg.DisplayAction1 = (string)JsonToken["DisplayAction1"];
                    newMsg.DisplayAction2 = (string)JsonToken["DisplayAction2"];
                    newMsg.PlatformName1  = (string)JsonToken["PlatformName1"];
                    newMsg.PlatformName2  = (string)JsonToken["PlatformName2"];
                    newMsg.DistKm         = (double)JsonToken["DistKm"];
                    newMsg.AzAngDegs      = (double)JsonToken["AzAngDegs"];
                    newMsg.AltOffsetKm    = (double)JsonToken["AltOffsetKm"];
                    newMsg.DurationSecs   = (double)JsonToken["DurationSecs"];

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






