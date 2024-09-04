
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
        sb.AppendLine($"Resolution: {tile.HorizRes}, {tile.VertRes}");

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
        string[] lines = content.Split('\n');

        // Read bounding box
        string[] l1parts = lines[0].Split(':');
        if (l1parts.Length != 2) return null;
        string[] l1values = l1parts[1].Split(',');
        if (l1values.Length != 4) return null;

        double minLat = double.Parse(l1values[0]);
        double minLon = double.Parse(l1values[1]);
        double maxLat = double.Parse(l1values[2]);
        double maxLon = double.Parse(l1values[3]);

        FssLLBox llBox = new FssLLBox(minLat, minLon, maxLat, maxLon);

        // Read resolution
        string[] l2parts = lines[1].Split(':');
        if (l2parts.Length != 2) return null;
        string[] l2values = l2parts[1].Split(',');
        if (l2values.Length != 2) return null;

        int horizRes = int.Parse(l2values[0]);
        int vertRes  = int.Parse(l2values[1]);

        // Read elevation data
        int numRows = lines.Length - 2;
        FssFloat2DArray elevData = new FssFloat2DArray(horizRes, vertRes);

        for (int i = 0; i < numRows; i++)
        {
            string[] values = lines[i + 2].Split(',');
            FssFloat1DArray row = new FssFloat1DArray(values.Length);
            for (int j = 0; j < values.Length; j++)
            {
                row[j] = float.Parse(values[j]);
            }
            elevData.SetRow(i, row);
        }

        return new FssElevationTile(elevData, llBox, horizRes, vertRes);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Binary Read Write
    // --------------------------------------------------------------------------------------------

    public static void WriteToBinaryFile(FssElevationTile tile, string filePath)
    {
        //

    }

    public static FssElevationTile? ReadFromBinaryFile(string filePath)
    {
        //

        return null;
    }

}



