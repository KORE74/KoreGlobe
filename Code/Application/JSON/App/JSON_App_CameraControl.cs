using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FssJSON
{
    public class AppCameraControl : JSONMessage
    {
        [JsonPropertyName("DisplayAction1")]
        public string DisplayAction1 { get; set; }

        [JsonPropertyName("DisplayAction2")]
        public string DisplayAction2 { get; set; }

        [JsonPropertyName("PlatformName1")]
        public string PlatformName1 { get; set; }

        [JsonPropertyName("PlatformName2")]
        public string PlatformName2 { get; set; }

        [JsonPropertyName("DistKm")]
        public double DistKm { get; set; }

        [JsonPropertyName("AzAngDegs")]
        public double AzAngDegs { get; set; }

        [JsonPropertyName("AltOffsetKm")]
        public double AltOffsetKm { get; set; }

        [JsonPropertyName("DurationSecs")]
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
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    JsonElement root = doc.RootElement;

                    if (root.TryGetProperty("AppCameraControl", out JsonElement jsonToken))
                    {
                        AppCameraControl newMsg = JsonSerializer.Deserialize<AppCameraControl>(jsonToken.GetRawText());

                        return newMsg;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    } // end class
} // end namespace
