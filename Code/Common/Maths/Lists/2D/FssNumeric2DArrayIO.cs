using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Numerics;

public static class FssNumeric2DArrayIO<T> where T : struct, INumber<T>
{
    // --------------------------------------------------------------------------------------------
    // MARK: CSV String
    // --------------------------------------------------------------------------------------------

    public static string ToCSVString(FssNumeric2DArray<T> array, int decimalPlaces)
    {
        StringBuilder csvBuilder = new StringBuilder();
        string format = "F" + decimalPlaces; // Format string to specify decimal places

        for (int i = 0; i < array.Height; i++)
        {
            for (int j = 0; j < array.Width; j++)
            {
                csvBuilder.Append(array[j, i].ToString(format, CultureInfo.InvariantCulture));
                csvBuilder.Append(", ");
            }
        }
        return csvBuilder.ToString().TrimEnd(',', ' ');
    }

    // --------------------------------------------------------------------------------------------

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
                array[j, i] = T.Parse(values[j].Trim(), CultureInfo.InvariantCulture);
            }
        }

        return array;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: CSV File I/O
    // --------------------------------------------------------------------------------------------

    public static void SaveToCSVFile(FssNumeric2DArray<T> array, string filePath, int decimalPlaces)
    {
        string csvString = ToCSVString(array, decimalPlaces);
        File.WriteAllText(filePath, csvString);
    }

    // --------------------------------------------------------------------------------------------

    public static FssNumeric2DArray<T> LoadFromCSVFile(string filePath)
    {
        string csvString = File.ReadAllText(filePath);
        return FromCSVString(csvString);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Binary File I/O
    // --------------------------------------------------------------------------------------------

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
                    if (typeof(T) == typeof(float))
                    {
                        writer.Write(float.CreateChecked(array[i, j]));
                    }
                    else if (typeof(T) == typeof(double))
                    {
                        writer.Write(double.CreateChecked(array[i, j]));
                    }
                    else if (typeof(T) == typeof(int))
                    {
                        writer.Write(int.CreateChecked(array[i, j]));
                    }
                }
            }
        }
    }

    // --------------------------------------------------------------------------------------------

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
                    array[i, j] = T.CreateChecked(reader.ReadDouble());
                }
            }

            array.Populated = true;
            return array;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Arc ASCII Grid File I/O
    // --------------------------------------------------------------------------------------------

    public static void SaveToArcASCIIGridFile(
        FssNumeric2DArray<T> array, string filePath,
        T xllcorner = default, T yllcorner = default,
        T cellsize = default, T nodataValue = default)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine($"ncols        {array.Width}");
            writer.WriteLine($"nrows        {array.Height}");
            writer.WriteLine($"xllcorner    {xllcorner}");
            writer.WriteLine($"yllcorner    {yllcorner}");
            writer.WriteLine($"cellsize     {cellsize}");
            writer.WriteLine($"nodata_value {nodataValue}");

            for (int j = 0; j < array.Height; j++)
            {
                for (int i = 0; i < array.Width; i++)
                {
                    writer.Write(array[i, j] + " ");
                }
                writer.WriteLine();
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    public static FssNumeric2DArray<T> LoadFromArcASCIIGridFile(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);

        int inwidth = int.Parse(lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
        int inheight = int.Parse(lines[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
        T xllcorner = T.Parse(lines[2].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1], CultureInfo.InvariantCulture);
        T yllcorner = T.Parse(lines[3].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1], CultureInfo.InvariantCulture);
        T cellsize = T.Parse(lines[4].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1], CultureInfo.InvariantCulture);
        T nodataValue = T.Parse(lines[5].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1], CultureInfo.InvariantCulture);

        FssNumeric2DArray<T> grid = new FssNumeric2DArray<T>(inwidth, inheight);

        for (int i = 6; i < lines.Length; i++)
        {
            int lineIndex = i - 6;
            string[] elevationValues = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < inwidth; j++)
            {
                T elevation = T.Parse(elevationValues[j], CultureInfo.InvariantCulture);
                if (elevation.Equals(nodataValue))
                {
                    elevation = T.Zero;
                }
                grid[j, lineIndex] = elevation;
            }
        }
        return grid;
    }
}
