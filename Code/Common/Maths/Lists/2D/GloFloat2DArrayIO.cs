using System;
using System.IO;
using System.Text;

public static class GloFloat2DArrayIO
{
    // Format the numbers using "InvariantCulture" that ensures localisation formating
    // around thousand separators and decimal points is NOT applied.

    // In all the IO operations, and text presentations, [0,0] is the top-left corner of the array.
    // When used as a heightmap, [0,0] is the top-left corner of the map.

    public static string ToCSVString(GloFloat2DArray array, int decimalPlaces)
    {
        StringBuilder csvBuilder = new StringBuilder();
        string format = "F" + decimalPlaces; // Format string to specify decimal places

        for (int i = 0; i < array.Height; i++)
        {
            for (int j = 0; j < array.Width; j++)
            {
                csvBuilder.Append(array[j, i].ToString(format, System.Globalization.CultureInfo.InvariantCulture));
                //if (j < array.Width - 1)
                    csvBuilder.Append(", ");
            }
            //csvBuilder.AppendLine();
        }
        return csvBuilder.ToString();
    }

    public static GloFloat2DArray FromCSVString(string csvString)
    {
        string[] lines = csvString.Trim().Split('\n');
        int rows = lines.Length;
        int cols = lines[0].Split(',').Length;
        GloFloat2DArray array = new GloFloat2DArray(rows, cols);

        for (int i = 0; i < rows; i++)
        {
            string[] values = lines[i].Split(',');
            for (int j = 0; j < cols; j++)
            {
                array[i, j] = float.Parse(values[j].Trim(), System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        return array;
    }

    // --------------------------------------------------------------------------------------------

    public static void SaveToCSVFile(GloFloat2DArray array, string filePath, int decimalPlaces)
    {
        string csvString = ToCSVString(array, decimalPlaces);
        File.WriteAllText(filePath, csvString);
    }

    public static GloFloat2DArray LoadFromCSVFile(string filePath)
    {
        string csvString = File.ReadAllText(filePath);
        return FromCSVString(csvString);
    }

    // --------------------------------------------------------------------------------------------

    public static void SaveToBinaryFile(GloFloat2DArray array, string filePath)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            writer.Write(array.Width);
            writer.Write(array.Height);

            for (int j = 0; j < array.Height; j++)
            {
                for (int i = 0; i < array.Width; i++)
                {
                    writer.Write(array[i, j]);
                }
            }
        }
    }

    public static GloFloat2DArray LoadFromBinaryFile(string filePath)
    {
        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            int width  = reader.ReadInt32();
            int height = reader.ReadInt32();
            GloFloat2DArray array = new GloFloat2DArray(width, height);

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    array[i, j] = reader.ReadSingle(); // ReadSingle is used for reading float values
                }
            }

            array.Populated = true;
            return array;
        }
    }


}
