
using System;

#nullable enable

// FssElevationPrepTile: A tile sized by an arbitrary AzEl box, and at an arbitrary resolution. Exists to
// interpolate elevation values at lat/long values required for exporting tiles.

public class FssElevationPrepTile
{
    public FssFloat2DArray ElevationData { get; set; } = new();
    public FssLLBox        LLBox         { get; set; } = FssLLBox.Zero;

    // --------------------------------------------------------------------------------------------
    // MARK: Resolution
    // --------------------------------------------------------------------------------------------

    // The lat long box will have a resolution in degrees. The 2D array will have a number of points.
    // GetRes calculates and returns the lowest number of points per degree in the lat long box.

    public float TileRes()
    {
        float latRes = (float)LLBox.DeltaLatDegs / ElevationData.Height;
        float lonRes = (float)LLBox.DeltaLonDegs / ElevationData.Width;

        return (latRes < lonRes) ? latRes : lonRes;
    }

    // --------------------------------------------------------------------------------------------

    // Function to return the number of points for longitude given a latitude.
    // Usage: int lonRes = FssElevationPrepTile.GetLongitudeResolution(20, 60.0);
    public static int GetLongitudeResolution(int latitudeResolution, double latDegs)
    {
        // Clamp latitude to valid range (-90 to 90 degrees).
        latDegs = Math.Max(-90.0, Math.Min(90.0, latDegs));

        // Convert latitude to radians for easier trigonometric calculations.
        double latRads = Math.Abs(latDegs) * (Math.PI / 180.0);

        // Calculate a scaling factor based on the cosine of the latitude.
        // Near the equator (latitude = 0), factor is 1.0 (full resolution).
        // Near the poles (latitude = +/-90), factor approaches 0 (lower resolution).
        double scale = Math.Cos(latRads);

        // Calculate the adjusted resolution for longitude.
        int adjustedResolution = (int)Math.Max(1, Math.Round(latitudeResolution * scale));

        return adjustedResolution;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Elevation for Position
    // --------------------------------------------------------------------------------------------

    public bool Contains(FssLLPoint pos)
    {
        return LLBox.Contains(pos);
    }

    public float ElevationAtPos(FssLLPoint pos)
    {
        // Check if the position is within the bounds of the tile.
        if (!LLBox.Contains(pos))
            return FssElevationPrepSystem.InvalidEle;

        // Calculate the fractional position within the tile, then use that to access the value.
        double fracLat = (pos.LatDegs - LLBox.MinLatDegs) / LLBox.DeltaLatDegs;
        double fracLon = (pos.LonDegs - LLBox.MinLonDegs) / LLBox.DeltaLonDegs;

        // Validate the fraction
        fracLat = FssValueUtils.LimitToRange(fracLat, 0, 1);
        fracLon = FssValueUtils.LimitToRange(fracLon, 0, 1);

        return ElevationData.InterpolatedValue((float)fracLon, (float)fracLat);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Report
    // --------------------------------------------------------------------------------------------

    public string Report()
    {
        return $"LLBox: {LLBox.ToStringVerbose()} / Res: {TileRes()} / GridSize:{ElevationData.Width}x{ElevationData.Height}";
    }

}