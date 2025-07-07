using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GloJSON
{
    public class GloLLAConverter : JsonConverter<GloLLAPoint>
    {
        public override GloLLAPoint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsonObject = JsonDocument.ParseValue(ref reader).RootElement;

            return new GloLLAPoint
            {
                LatDegs = jsonObject.GetProperty("LatDegs").GetDouble(),
                LonDegs = jsonObject.GetProperty("LonDegs").GetDouble(),
                AltMslM = jsonObject.GetProperty("AltMslM").GetDouble()
            };
        }

        public override void Write(Utf8JsonWriter writer, GloLLAPoint value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("LatDegs", value.LatDegs.ToString("0.####"));
            writer.WriteString("LonDegs", value.LonDegs.ToString("0.####"));
            writer.WriteString("AltMslM", value.AltMslM.ToString("0.####"));
            writer.WriteEndObject();
        }
    }

    // --------------------------------------------------------------------------------------------

    public class GloCourseConverter : JsonConverter<GloCourse>
    {
        public override GloCourse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsonObject = JsonDocument.ParseValue(ref reader).RootElement;

            return new GloCourse
            {
                HeadingDegs = jsonObject.GetProperty("HeadingDegs").GetDouble(),
                SpeedKph = jsonObject.GetProperty("SpeedKph").GetDouble()
            };
        }

        public override void Write(Utf8JsonWriter writer, GloCourse value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("HeadingDegs", value.HeadingDegs);
            writer.WriteNumber("SpeedKph", value.SpeedKph);
            writer.WriteEndObject();
        }
    }

    // --------------------------------------------------------------------------------------------

    public class CustomDoubleConverter : JsonConverter<double>
    {
        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Math.Round(reader.GetDouble(), 3);
        }

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(Math.Round(value, 3));
        }
    }
} // end namespace
