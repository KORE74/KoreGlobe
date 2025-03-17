using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Numerics;

public static class FssNumeric2DArrayIO<T> where T : struct, INumber<T>
{
    // --------------------------------------------------------------------------------------------
    // MARK: CSV Operations
    // --------------------------------------------------------------------------------------------

    public static string ToCSVString(FssNumeric2DArray<T> array, int decimalPlaces)
    {
        StringBuilder csvBuilder = new StringBuilder();
        string format = "F" + decimalPlaces; // Format string to specify decimal places

        for (int i = 0; i < array.Height; i++)
        {
            for (int j = 0; j < array.Width; j++)
            {
                csvBuilder.Append(ConvertToString(array[j, i], format));
                if (j < array.Width - 1)
                    csvBuilder.Append(", ");
            }
            csvBuilder.AppendLine();
        }
        return csvBuilder.ToString();
    }

    public static FssNumeric2DArray<T> FromCSVString(string csvString)
    {
        string[] lines = csvString.Trim().Split('\n');
        int rows = lines.Length;
        int cols = lines[0].Split(',').Length;
        FssNumeric2DArray<T> array = new FssNumeric2DArray<T>(cols, rows);

        for (int i = 0; i < rows; i++)
        {
            string[] values = lines[i].Split(',');
            for (int j = 0; j < cols; j++)
            {
                array[j, i] = ConvertFromString(values[j].Trim());
            }
        }

        array.Populated = true;
        return array;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: File Operations
    // --------------------------------------------------------------------------------------------

    public static void SaveToCSVFile(FssNumeric2DArray<T> array, string filePath, int decimalPlaces)
    {
        string csvString = ToCSVString(array, decimalPlaces);
        File.WriteAllText(filePath, csvString);
    }

    public static FssNumeric2DArray<T> LoadFromCSVFile(string filePath)
    {
        string csvString = File.ReadAllText(filePath);
        return FromCSVString(csvString);
    }

    public static void SaveToBinaryFile(FssNumeric2DArray<T> array, string filePath)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            writer.Write(array.Width);
            writer.Write(array.Height);

            for (int j = 0; j < array.Height; j++)
            {
                for (int i = 0; i < array.Width; i++)
                {
                    WriteValue(writer, array[i, j]);
                }
            }
        }
    }

    public static FssNumeric2DArray<T> LoadFromBinaryFile(string filePath)
    {
        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            FssNumeric2DArray<T> array = new FssNumeric2DArray<T>(width, height);

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    array[i, j] = ReadValue(reader);
                }
            }

            array.Populated = true;
            return array;
        }
    }

    // --------------------------------------------------------------------------------------------
    // Helper Methods
    // --------------------------------------------------------------------------------------------

    private static string ConvertToString(T value, string format)
    {
        return Convert.ToDouble(value).ToString(format, CultureInfo.InvariantCulture);
    }

    private static T ConvertFromString(string value)
    {
        return T.CreateChecked(double.Parse(value, CultureInfo.InvariantCulture));
    }

    private static void WriteValue(BinaryWriter writer, T value)
    {
        writer.Write(Convert.ToDouble(value));
    }

    private static T ReadValue(BinaryReader reader)
    {
        return T.CreateChecked(reader.ReadDouble());
    }
}
