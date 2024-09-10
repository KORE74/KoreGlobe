
// Static utility class to read and write FSS elevation tiles to file.


using System;
using System.IO;
using System.Text;

public static class FssElevationTileIO
{
    // --------------------------------------------------------------------------------------------
    // MARK: Constants
    // --------------------------------------------------------------------------------------------

    public const string ElevationTileExtension = ".fssElevTile";

    // --------------------------------------------------------------------------------------------
    // MARK: Text Read Write
    // --------------------------------------------------------------------------------------------

    public static string WriteToString(FssElevationTile tile)
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

    public static FssElevationTile? ReadFromString(string content)
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

            return new FssElevationTile() { ElevationData = elevData, LLBox = llBox };
        }
        catch (Exception)
        {
            return null; // Return null in case of any errors during parsing.
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Binary Read Write
    // --------------------------------------------------------------------------------------------

    public static void WriteToBinaryFile(FssElevationTile tile, string filePath)
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

    public static FssElevationTile? ReadFromBinaryFile(string filePath)
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

                return new FssElevationTile() { ElevationData = elevData, LLBox = llBox };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading from binary file: {ex.Message}");
            return null;
        }
    }

}



