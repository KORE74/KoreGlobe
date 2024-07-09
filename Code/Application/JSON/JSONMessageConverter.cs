using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobeJSON
{

    public class GlobeLLAConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GlobeLLA);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var globeLLA = (GlobeLLA)value;

            var jsonObject = new JObject
            {
                {"LatDegs", globeLLA.LatDegs.ToString("0.####")},
                {"LonDegs", globeLLA.LonDegs.ToString("0.####")},
                {"AltMslM", globeLLA.AltMslM.ToString("0.####")}
            };

            jsonObject.WriteTo(writer);
        }


        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var globeLLA = new GlobeLLA
            {
                LatDegs = (double)jsonObject["LatDegs"],
                LonDegs = (double)jsonObject["LonDegs"],
                AltMslM = (double)jsonObject["AltMslM"]
            };
            return globeLLA;
        }
    }

    // --------------------------------------------------------------------------------------------

    public class GlobeCourseConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GlobeCourse);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var globeCourse = (GlobeCourse)value;

            var jsonObject = new JObject
            {
                {"HeadingDegs", globeCourse.HeadingDegs},
                {"SpeedKph",    globeCourse.SpeedKph}
            };

            jsonObject.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var globeCourse = new GlobeCourse
            {
                HeadingDegs = (double)jsonObject["HeadingDegs"],
                SpeedKph    = (double)jsonObject["SpeedKph"]
            };
            return globeCourse;
        }
    }

    // --------------------------------------------------------------------------------------------

    public class CustomDoubleConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(double);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Integer)
            {
                var value = JToken.Load(reader);
                return Math.Round(value.Value<double>(), 3);
            }
            return 0.0;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(Math.Round((double)value, 3));
        }
    }


} // end namespace






