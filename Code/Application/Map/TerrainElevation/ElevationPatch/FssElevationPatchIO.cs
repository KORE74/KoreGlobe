
// Static utility class to read and write Fss elevation tiles to file.


using System;
using System.IO;
using System.Text;

#nullable enable

public static class FssElevationPatchIO
{
    // --------------------------------------------------------------------------------------------
    // MARK: Constants
    // --------------------------------------------------------------------------------------------

    public const string ElevationTileExtension = ".FssElevTile";

    // --------------------------------------------------------------------------------------------
    // MARK: Text File IO
    // --------------------------------------------------------------------------------------------

    // FssElevationPatchIO.WriteToTextFile

    public static void WriteToTextFile(FssElevationPatch tile, string filePath)
    {
        try
        {
            string content = WriteToString(tile);
            File.WriteAllText(filePath, content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to text file: {ex.Message}");
        }
    }

    // Usage: FssElevationPatch? newpatch = FssElevationPatchIO.ReadFromTextFile(filename);
    public static FssElevationPatch? ReadFromTextFile(string filePath)
    {
        try
        {
            string content = File.ReadAllText(filePath);
            return ReadFromString(content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading from text file: {ex.Message}");
            return null;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: String IO
    // --------------------------------------------------------------------------------------------

    public static string WriteToString(FssElevationPatch tile)
    {
        StringBuilder sb = new StringBuilder();

        // Write bounding box
        sb.AppendLine($"BoundingBox: {tile.LLBox.MinLatDegs}, {tile.LLBox.MinLonDegs}, {tile.LLBox.MaxLatDegs}, {tile.LLBox.MaxLonDegs}");

        // Write resolution
        sb.AppendLine($"Resolution: {tile.ElevationData.Width}, {tile.ElevationData.Height}");

        // Write elevation data (2D grid of values, and limit to 2dp for readability and space)
        int numrows = tile.ElevationData.Height;
        for (int i = 0; i < numrows; i++)
        {
            FssFloat1DArray row = tile.ElevationData.GetRow(i);
            for (int j = 0; j < row.Length; j++)
            {
                sb.Append(row[j].ToString("F2"));
                if (j < row.Length - 1) sb.Append(",");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    // --------------------------------------------------------------------------------------------

    public static FssElevationPatch? ReadFromString(string content)
    {
        try
        {
            string[] lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            // Read bounding box
            string[] l1parts = lines[0].Split(':');
            if (l1parts.Length != 2) return null;
            string[] l1values = l1parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (l1values.Length != 4) return null;

            double minLat = double.Parse(l1values[0]);
            double minLon = double.Parse(l1values[1]);
            double maxLat = double.Parse(l1values[2]);
            double maxLon = double.Parse(l1values[3]);

            FssLLBox llBox = new FssLLBox(minLat, minLon, maxLat, maxLon);

            // Read resolution
            string[] l2parts = lines[1].Split(':');
            if (l2parts.Length != 2) return null;
            string[] l2values = l2parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (l2values.Length != 2) return null;

            int horizRes = int.Parse(l2values[0]);
            int vertRes  = int.Parse(l2values[1]);

            // Read elevation data
            int numRows = lines.Length - 2;
            FssFloat2DArray elevData = new FssFloat2DArray(horizRes, vertRes);

            for (int i = 0; i < numRows; i++)
            {
                string[] values = lines[i + 2].Split(',', StringSplitOptions.RemoveEmptyEntries);
                FssFloat1DArray row = new FssFloat1DArray(values.Length);
                for (int j = 0; j < values.Length; j++)
                {
                    row[j] = float.Parse(values[j]);
                }
                elevData.SetRow(i, row);
            }

            return new FssElevationPatch() { ElevationData = elevData, LLBox = llBox };
        }
        catch (Exception)
        {
            return null; // Return null in case of any errors during parsing.
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Arc ASII Grid
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
            float cellsize  = -1f, float nodataValue = -1f)
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
    // MARK: Binary File IO
    // --------------------------------------------------------------------------------------------

    public static void WriteToBinaryFile(FssElevationPatch tile, string filePath)
    {
        try
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                // Write bounding box
                writer.Write(tile.LLBox.MinLatDegs);
                writer.Write(tile.LLBox.MinLonDegs);
                writer.Write(tile.LLBox.MaxLatDegs);
                writer.Write(tile.LLBox.MaxLonDegs);

                // Write resolution
                writer.Write(tile.ElevationData.Width);
                writer.Write(tile.ElevationData.Height);

                // Write elevation data
                int numRows = tile.ElevationData.Height;
                for (int i = 0; i < numRows; i++)
                {
                    FssFloat1DArray row = tile.ElevationData.GetRow(i);
                    for (int j = 0; j < row.Length; j++)
                    {
                        writer.Write(row[j]);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to binary file: {ex.Message}");
        }
    }

    // --------------------------------------------------------------------------------------------

    public static FssElevationPatch? ReadFromBinaryFile(string filePath)
    {
        try
        {
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                // Read bounding box
                double minLat = reader.ReadDouble();
                double minLon = reader.ReadDouble();
                double maxLat = reader.ReadDouble();
                double maxLon = reader.ReadDouble();
                FssLLBox llBox = new FssLLBox(minLat, minLon, maxLat, maxLon);

                // Read resolution
                int horizRes = reader.ReadInt32();
                int vertRes  = reader.ReadInt32();

                // Read elevation data
                FssFloat2DArray elevData = new FssFloat2DArray(horizRes, vertRes);
                for (int i = 0; i < vertRes; i++)
                {
                    FssFloat1DArray row = new FssFloat1DArray(horizRes);
                    for (int j = 0; j < horizRes; j++)
                    {
                        row[j] = reader.ReadSingle();
                    }
                    elevData.SetRow(i, row);
                }

                return new FssElevationPatch() { ElevationData = elevData, LLBox = llBox };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading from binary file: {ex.Message}");
            return null;
        }
    }

}



