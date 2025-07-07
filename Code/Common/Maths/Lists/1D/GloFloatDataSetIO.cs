// using System;
// using System.IO;
// using System.Text;
// using System.Globalization;

// public static class GloFloatDataSetIO
// {
//     // --------------------------------------------------------------------------------------------
//     // MARK: CSV String
//     // --------------------------------------------------------------------------------------------

//     public static string ToCSVString(GloFloatDataSet dataSet, int decimalPlaces)
//     {
//         StringBuilder csvBuilder = new StringBuilder();
//         string valueFormat = "F" + decimalPlaces;

//         foreach (var dataPoint in dataSet.DataPoints)
//         {
//             string fractionStr = dataPoint.Fraction.ToString(valueFormat, CultureInfo.InvariantCulture);
//             string valueStr    = dataPoint.Value.ToString(valueFormat, CultureInfo.InvariantCulture);
//             csvBuilder.AppendLine($"{fractionStr}, {valueStr}");
//         }
//         return csvBuilder.ToString();
//     }

//     public static GloFloatDataSet FromCSVString(string csvString)
//     {
//         string[] lines = csvString.Trim().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
//         GloFloatDataSet dataSet = new GloFloatDataSet();

//         foreach (var line in lines)
//         {
//             string[] parts = line.Split(',');
//             float fraction = float.Parse(parts[0].Trim(), CultureInfo.InvariantCulture);
//             float value    = float.Parse(parts[1].Trim(), CultureInfo.InvariantCulture);
//             dataSet.AddDataPoint(fraction, value);
//         }

//         return dataSet;
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: CSV File
//     // --------------------------------------------------------------------------------------------

//     public static void SaveToCSVFile(GloFloatDataSet dataSet, string filePath, int decimalPlaces)
//     {
//         string csvString = ToCSVString(dataSet, decimalPlaces);
//         File.WriteAllText(filePath, csvString);
//     }

//     public static GloFloatDataSet LoadFromCSVFile(string filePath)
//     {
//         string csvString = File.ReadAllText(filePath);
//         return FromCSVString(csvString);
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Binary File
//     // --------------------------------------------------------------------------------------------

//     public static void SaveToBinaryFile(GloFloatDataSet dataSet, string filePath)
//     {
//         using (BinaryWriter writer = new (File.Open(filePath, FileMode.Create)))
//         {
//             writer.Write(dataSet.DataPoints.Count);
//             foreach (var dataPoint in dataSet.DataPoints)
//             {
//                 writer.Write(dataPoint.Fraction);
//                 writer.Write(dataPoint.Value);
//             }
//         }
//     }

//     public static GloFloatDataSet LoadFromBinaryFile(string filePath)
//     {
//         using (BinaryReader reader = new (File.Open(filePath, FileMode.Open)))
//         {
//             int count = reader.ReadInt32();
//             GloFloatDataSet dataSet = new GloFloatDataSet();

//             for (int i = 0; i < count; i++)
//             {
//                 float fraction = reader.ReadSingle();
//                 float value = reader.ReadSingle();
//                 dataSet.AddDataPoint(fraction, value);
//             }

//             return dataSet;
//         }
//     }
// }
