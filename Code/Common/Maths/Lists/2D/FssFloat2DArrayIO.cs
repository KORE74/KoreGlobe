using System;
using System.IO;
using System.Text;

public static class FssFloat2DArrayIO
{
    // Format the numbers using "InvariantCulture" that ensures localisation formating
    // around thousand separators and decimal points is NOT applied.

    // In all the IO operations, and text presentations, [0,0] is the top-left corner of the array.
    // When used as a heightmap, [0,0] is the top-left corner of the map.

    public static string ToCSVString(FssFloat2DArray array, int decimalPlaces)
    {
        StringBuilder csvBuilder = new StringBuilder();
        string format = "F" + decimalPlaces; // Format string to specify decimal places

        for (int i = 0; i < array.Height; i++)
        {
            for (int j = 0; j < array.Width; j++)
            {
                csvBuilder.Append(array[j, i].ToString(format, System.Globalization.CultureInfo.InvariantCulture));
                if (j < array.Height - 1)
                    csvBuilder.Append(", ");
            }
            csvBuilder.AppendLine();
        }
        return csvBuilder.ToString();
    }

    public static FssFloat2DArray FromCSVString(string csvString)
    {
        string[] lines = csvString.Trim().Split('\n');
        int rows = lines.Length;
        int cols = lines[0].Split(',').Length;
        FssFloat2DArray array = new FssFloat2DArray(rows, cols);

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

    public static void SaveToCSVFile(FssFloat2DArray array, string filePath, int decimalPlaces)
    {
        string csvString = ToCSVString(array, decimalPlaces);
        File.WriteAllText(filePath, csvString);
    }

    public static FssFloat2DArray LoadFromCSVFile(string filePath)
    {
        string csvString = File.ReadAllText(filePath);
        return FromCSVString(csvString);
    }

    // --------------------------------------------------------------------------------------------

    public static void SaveToBinaryFile(FssFloat2DArray array, string filePath)
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

    public static FssFloat2DArray LoadFromBinaryFile(string filePath)
    {
        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            int width  = reader.ReadInt32();
            int height = reader.ReadInt32();
            FssFloat2DArray array = new FssFloat2DArray(width, height);

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

    // --------------------------------------------------------------------------------------------

    /*
        ncols         315
        nrows         270
        xllcorner     -491880.41
        yllcorner     5849212.228
        cellsize      200
        nodata_value  -9999.0
        <space separated data>
    */

    // Usage: FssFloat2DArray asciiArcArry = FssFloat2DArrayIO.LoadFromArcASIIGridFile(<filename>);

    public static FssFloat2DArray LoadFromArcASIIGridFile(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);

        // Parse header
        int   inwidth     =   int.Parse(lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
        int   inheight    =   int.Parse(lines[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
        float xllcorner   = float.Parse(lines[2].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
        float yllcorner   = float.Parse(lines[3].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
        float cellsize    = float.Parse(lines[4].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
        float nodataValue = float.Parse(lines[5].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);

        FssFloat2DArray grid = new FssFloat2DArray(inwidth, inheight);

        // Start reading data from line 6 (after the header)
        for (int i = 6; i < lines.Length; i++)
        {
            int lineIndex = i - 6;

            string[] elevationValues = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < inwidth; j++)
            {
                float elevation = float.Parse(elevationValues[j]);

                // Check for no data value
                if (elevation == nodataValue)
                {
                    // Handle no data value if needed
                    elevation = 0; // Example: set to 0 or some other appropriate value
                }
                grid[j, lineIndex] = elevation;
            }
        }
        return grid;
    }

    // Write a file back out to something like an ASCII ArcGrid file for later reading.
    // We don't have the maths/geography libraries to do this accurately, so many of the params are expected to be deafults.

    // Usage: FssFloat2DArrayIO.SaveToArcASIIGridFile(asciiArcArry, <filename>);

    public static void SaveToArcASIIGridFile(
            FssFloat2DArray array, string filePath,
            float xllcorner = -1f, float yllcorner = -1f,
            float cellsize = -1f, float nodataValue = -1f)
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

}
