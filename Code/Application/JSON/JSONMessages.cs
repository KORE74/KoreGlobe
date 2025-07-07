using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GloJSON
{
    public class JSONMessage
    {
    }

    // --------------------------------------------------------------------------------------------

    // Usage: IncomingMessageHandler.ProcessMessage("AppShutdown", message);
    public class IncomingMessageHandler
    {
        static public JSONMessage ProcessMessage(string strtype, string msgText)
        {
            switch (strtype)
            {
                // System Control
                case "AppShutdown":    return AppShutdown.ParseJSON(msgText);
                case "NullMsg":        return NullMsg.ParseJSON(msgText);

                // Camera Control
                case "PlatFocus":      return PlatFocus.ParseJSON(msgText);

                // Platform
                case "PlatUpdate":     return PlatUpdate.ParseJSON(msgText);
                case "PlatPosition":   return PlatPosition.ParseJSON(msgText);
                case "PlatAdd":        return PlatAdd.ParseJSON(msgText);
                case "PlatDelete":     return PlatDelete.ParseJSON(msgText);
                case "PlatWayPoints":  return PlatWayPoints.ParseJSON(msgText);

                // Scenario / Time Control
                case "ScenLoad":       return ScenLoad.ParseJSON(msgText);
                case "ScenStart":      return ScenStart.ParseJSON(msgText);
                case "ScenStop":       return ScenStop.ParseJSON(msgText);
                case "ScenPause":      return ScenPause.ParseJSON(msgText);
                case "ScenCont":       return ScenCont.ParseJSON(msgText);
                case "ClockSync":      return ClockSync.ParseJSON(msgText);

                // Geometry - Emitter
                case "BeamLoad":       return BeamLoad.ParseJSON(msgText);
                case "BeamDelete":     return BeamDelete.ParseJSON(msgText);
                case "BeamEnable":     return BeamEnable.ParseJSON(msgText);
                case "BeamDisable":    return BeamDisable.ParseJSON(msgText);
                case "AntennaPattern": return AntennaPattern.ParseJSON(msgText);
                case "ScanPattern":    return ScanPattern.ParseJSON(msgText);

                default:
                    return null;
            }
        }

        static public string GetMessageTypeName(string message)
        {
            string substr = message.Substring(0, Math.Min(message.Length, 25));

            try
            {
                using (JsonDocument doc = JsonDocument.Parse(message))
                {
                    JsonElement root = doc.RootElement;
                    if (root.EnumerateObject().MoveNext())
                    {
                        string firstPropertyName = root.EnumerateObject().First().Name;
                        return firstPropertyName;
                    }
                }
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }
    } // end IncomingMessageHandler
}
