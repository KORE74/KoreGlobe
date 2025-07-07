using System;
using System.IO;
using System.Text;
using System.Globalization;

public static class GloFloat1DArrayIO
{

    // --------------------------------------------------------------------------------------------
    // MARK: CSV String
    // --------------------------------------------------------------------------------------------

    public static string ToCSVString(GloFloat1DArray array, int decimalPlaces)
    {
        StringBuilder csvBuilder = new StringBuilder();
        string format = "F" + decimalPlaces;

        for (int i = 0; i < array.Length; i++)
        {
            csvBuilder.Append(array[i].ToString(format, CultureInfo.InvariantCulture));
            if (i < array.Length - 1)
                csvBuilder.Append(", ");
        }
        return csvBuilder.ToString();
    }

    public static GloFloat1DArray FromCSVString(string csvString)
    {
        string[] values = csvString.Trim().Split(',');
        GloFloat1DArray array = new GloFloat1DArray(values.Length);

        for (int i = 0; i < values.Length; i++)
        {
            array[i] = float.Parse(values[i].Trim(), CultureInfo.InvariantCulture);
        }

        return array;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: CSV File
    // --------------------------------------------------------------------------------------------

    public static void SaveToCSVFile(GloFloat1DArray array, string filePath, int decimalPlaces)
    {
        string csvString = ToCSVString(array, decimalPlaces);
        File.WriteAllText(filePath, csvString);
    }

    public static GloFloat1DArray LoadFromCSVFile(string filePath)
    {
        string csvString = File.ReadAllText(filePath);
        return FromCSVString(csvString);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Binary File
    // --------------------------------------------------------------------------------------------

    public static void SaveToBinaryFile(GloFloat1DArray array, string filePath)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            writer.Write(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                writer.Write(array[i]);
            }
        }
    }

    public static GloFloat1DArray LoadFromBinaryFile(string filePath)
    {
        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            int length = reader.ReadInt32();
            GloFloat1DArray array = new GloFloat1DArray(length);

            for (int i = 0; i < length; i++)
            {
                array[i] = reader.ReadSingle();
            }

            return array;
        }
    }
}
