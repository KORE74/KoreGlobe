﻿

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

//  {"PlatWayPoints":{"PlatName":"Red Leader","Count":4,
    // "Legs":[
    //     {"LatDegs":51.1599,"LongDegs":-0.6887,"AltitudeMtrs":3500.0,"GrndSpeedMtrsSec":0.0,"WPType":"Origin"},
    //     {"LatDegs":49.122049,"LongDegs":0.531608,"AltitudeMtrs":3500.0,"GrndSpeedMtrsSec":277.8,"WPType":"linear"},
    //     {"LatDegs":50.292102,"LongDegs":0.403041,"AltitudeMtrs":3500.0,"GrndSpeedMtrsSec":277.8,"WPType":"arc"},
    //     {"LatDegs":51.276886,"LongDegs":-1.384932,"AltitudeMtrs":3500.0,"GrndSpeedMtrsSec":277.8,"WPType":"linear"}]}}


namespace GloJSON
{
    public class WayPoint
    {
        [JsonPropertyName("LatDegs")]
        public double LatDegs { get; set; }

        [JsonPropertyName("LongDegs")]
        public double LongDegs { get; set; }

        [JsonPropertyName("AltitudeMtrs")]
        public double AltitudeMtrs { get; set; }

        [JsonPropertyName("GrndSpeedMtrsSec")]
        public double GrndSpeedMtrsSec { get; set; }

        [JsonPropertyName("WPType")]
        public string WPType { get; set; }
    }

    public class PlatWayPoints : JSONMessage
    {
        [JsonPropertyName("PlatName")]
        public string PlatName { get; set; }

        [JsonPropertyName("Count")]
        public int Count { get; set; }

        [JsonPropertyName("Legs")]
        public List<WayPoint> Legs { get; set; }

        // -----------------------

        // Additional accessor methods

        public GloLLAPoint GetPoint(int pntIndex)
        {
            if (pntIndex < 0) return new GloLLAPoint();
            if (pntIndex > Legs.Count - 1) return new GloLLAPoint();

            WayPoint currleg = Legs[pntIndex];

            GloLLAPoint currPos = new GloLLAPoint
            {
                LatDegs = currleg.LatDegs,
                LonDegs = currleg.LongDegs,
                AltMslM = currleg.AltitudeMtrs
            };

            return currPos;
        }

        public List<GloLLAPoint> Points()
        {
            List<GloLLAPoint> points = new List<GloLLAPoint>();

            foreach (WayPoint leg in Legs)
            {
                // Get the type of waypoint. Some are "unknown" which we need to filter out.
                bool isLegValid = false;
                if      (leg.WPType.Equals("origin", StringComparison.OrdinalIgnoreCase)) isLegValid = true;
                else if (leg.WPType.Equals("linear", StringComparison.OrdinalIgnoreCase)) isLegValid = true;
                else if (leg.WPType.Equals("arc",    StringComparison.OrdinalIgnoreCase)) isLegValid = true;

                if (isLegValid)
                {
                    GloLLAPoint currPos = new GloLLAPoint
                    {
                        LatDegs = leg.LatDegs,
                        LonDegs = leg.LongDegs,
                        AltMslM = leg.AltitudeMtrs
                    };

                    points.Add(currPos);
                }
            }

            return points;
        }

        // -----------------------

        public static PlatWayPoints ParseJSON(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("PlatWayPoints", out JsonElement jsonContent))
                    {
                        PlatWayPoints newMsg = JsonSerializer.Deserialize<PlatWayPoints>(jsonContent.GetRawText());

                        GloCentralLog.AddEntry("PlatWayPoints -> JsonContent OK");
                        GloCentralLog.AddEntry($"PlatWayPoints -> PlatName = {newMsg.PlatName}");
                        GloCentralLog.AddEntry($"PlatWayPoints -> Count = {newMsg.Count}");

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
            }
            catch (Exception)
            {
                return null;
            }
        }
    } // end class

} // end namespace
