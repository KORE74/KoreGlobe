
// Static utility class to read and write Glo elevation patches to file.


using System;
using System.IO;
using System.Text;

#nullable enable

public static class GloElevationPatchIO
{
    // --------------------------------------------------------------------------------------------
    // MARK: Constants
    // --------------------------------------------------------------------------------------------

    public const string ElevationTileExtension = ".elepatch";

    // --------------------------------------------------------------------------------------------
    // MARK: Text File IO
    // --------------------------------------------------------------------------------------------

    // GloElevationPatchIO.WriteToTextFile

    public static void WriteToTextFile(GloElevationPatch patch, string filePath)
    {
        try
        {
            string content = WriteToString(patch);
            File.WriteAllText(filePath, content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to text file: {ex.Message}");
        }
    }

    public static GloElevationPatch? ReadFromTextFile(string filePath)
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

    public static string WriteToString(GloElevationPatch patch)
    {
        StringBuilder sb = new StringBuilder();

        // Write bounding box
        sb.AppendLine($"BoundingBox: {patch.LLBox.MinLatDegs:F5}, {patch.LLBox.MinLonDegs:F5}, {patch.LLBox.MaxLatDegs:F5}, {patch.LLBox.MaxLonDegs:F5}");

        // Write resolution
        sb.AppendLine($"Resolution: {patch.ElevationData.Width}, {patch.ElevationData.Height}");

        // Write elevation data (2D grid of values, and limit to 2dp for readability and space)
        int numrows = patch.ElevationData.Height;
        for (int i = 0; i < numrows; i++)
        {
            GloFloat1DArray row = patch.ElevationData.GetRow(i);
            for (int j = 0; j < row.Length; j++)
            {
                sb.Append(row[j].ToString("F2"));
                if (j < row.Length - 1) sb.Append(", ");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }


    // Could be too big to put into a string, so append to a file incrementally

    public static void WriteToTextFile2(GloElevationPatch patch, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write bounding box
            writer.WriteLine($"BoundingBox: {patch.LLBox.MinLatDegs:F5}, {patch.LLBox.MinLonDegs:F5}, {patch.LLBox.MaxLatDegs:F5}, {patch.LLBox.MaxLonDegs:F5}");

            // Write resolution
            writer.WriteLine($"Resolution: {patch.ElevationData.Width}, {patch.ElevationData.Height}");

            // Write elevation data (streaming row by row to avoid high memory usage)
            int numRows = patch.ElevationData.Height;
            for (int i = 0; i < numRows; i++)
            {
                GloFloat1DArray row = patch.ElevationData.GetRow(i);

                // Write row to the file
                for (int j = 0; j < row.Length; j++)
                {
                    writer.Write(row[j].ToString("F2"));
                    if (j < row.Length - 1)
                        writer.Write(", "); // Add comma for separation
                }

                writer.WriteLine(); // End of the row
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    public static GloElevationPatch? ReadFromString(string content)
    {
        try
        {
            string[] lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            // Read bounding box
            string[] l1parts = lines[0].Split(':');
            if (l1parts.Length != 2) return null;
            string[] l1values = l1parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (l1values.Length != 4) return null;

            double minLatDegs = double.Parse(l1values[0]);
            double minLonDegs = double.Parse(l1values[1]);
            double maxLatDegs = double.Parse(l1values[2]);
            double maxLonDegs = double.Parse(l1values[3]);

            GloLLBox llBox = new GloLLBox() {
                MinLatDegs = minLatDegs,
                MinLonDegs = minLonDegs,
                MaxLatDegs = maxLatDegs,
                MaxLonDegs = maxLonDegs };

            // Read resolution
            string[] l2parts = lines[1].Split(':');
            if (l2parts.Length != 2) return null;
            string[] l2values = l2parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (l2values.Length != 2) return null;

            int horizRes = int.Parse(l2values[0]);
            int vertRes  = int.Parse(l2values[1]);

            // Read elevation data
            int numRows = lines.Length - 2;
            GloFloat2DArray elevData = new GloFloat2DArray(horizRes, vertRes);

            for (int i = 0; i < numRows; i++)
            {
                string[] values = lines[i + 2].Split(',', StringSplitOptions.RemoveEmptyEntries);
                GloFloat1DArray row = new GloFloat1DArray(values.Length);
                for (int j = 0; j < values.Length; j++)
                {
                    row[j] = float.Parse(values[j]);
                }
                elevData.SetRow(i, row);
            }

            return new GloElevationPatch() { ElevationData = elevData, LLBox = llBox };
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

    // Usage: GloFloat2DArray asciiArcArry = GloFloat2DArrayIO.LoadFromArcASIIGridFile(<filename>);

    public static GloFloat2DArray LoadFromArcASIIGridFile(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);

        // Parse header
        int   inwidth     =   int.Parse(lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
        int   inheight    =   int.Parse(lines[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
        float xllcorner   = float.Parse(lines[2].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
        float yllcorner   = float.Parse(lines[3].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
        float cellsize    = float.Parse(lines[4].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
        float nodataValue = float.Parse(lines[5].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);

        GloFloat2DArray grid = new GloFloat2DArray(inwidth, inheight);

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
                // X, Y top to bottom as in the file
                grid[j, lineIndex] = elevation;
            }
        }
        return grid;
    }

    // Write a file back out to something like an ASCII ArcGrid file for later reading.
    // We don't have the maths/geography libraries to do this accurately, so many of the params are expected to be deafults.

    // Usage: GloFloat2DArrayIO.SaveToArcASIIGridFile(asciiArcArry, <filename>);

    public static void SaveToArcASIIGridFile(
            GloFloat2DArray array, string filePath,
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
                    // X, Y, top to bottom as in the array
                    writer.Write(array[i, j] + " ");
                }
                writer.WriteLine();
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Binary File IO
    // --------------------------------------------------------------------------------------------

    public static void WriteToBinaryFile(GloElevationPatch patch, string filePath)
    {
        try
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                // Write bounding box
                writer.Write(patch.LLBox.MinLatDegs);
                writer.Write(patch.LLBox.MinLonDegs);
                writer.Write(patch.LLBox.MaxLatDegs);
                writer.Write(patch.LLBox.MaxLonDegs);

                // Write resolution
                writer.Write(patch.ElevationData.Width);
                writer.Write(patch.ElevationData.Height);

                // Write elevation data
                int numRows = patch.ElevationData.Height;
                for (int i = 0; i < numRows; i++)
                {
                    GloFloat1DArray row = patch.ElevationData.GetRow(i);
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

    public static GloElevationPatch? ReadFromBinaryFile(string filePath)
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
                GloLLBox llBox = new GloLLBox(minLat, minLon, maxLat, maxLon);

                // Read resolution
                int horizRes = reader.ReadInt32();
                int vertRes  = reader.ReadInt32();

                // Read elevation data
                GloFloat2DArray elevData = new GloFloat2DArray(horizRes, vertRes);
                for (int i = 0; i < vertRes; i++)
                {
                    GloFloat1DArray row = new GloFloat1DArray(horizRes);
                    for (int j = 0; j < horizRes; j++)
                    {
                        row[j] = reader.ReadSingle();
                    }
                    elevData.SetRow(i, row);
                }

                return new GloElevationPatch() { ElevationData = elevData, LLBox = llBox };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading from binary file: {ex.Message}");
            return null;
        }
    }

}



