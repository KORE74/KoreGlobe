
using System;

//nullable enable

// GloElevationPatch: A tile sized by an arbitrary AzEl box, and at an arbitrary resolution. Exists to
// interpolate elevation values at lat/long values required for exporting tiles.

public class GloElevationPatch
{
    public GloFloat2DArray ElevationData { get; set; } = new();
    public GloLLBox        LLBox         { get; set; } = GloLLBox.Zero;

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
    // MARK: Elevation for Position
    // --------------------------------------------------------------------------------------------

    public bool Contains(GloLLPoint pos)
    {
        return LLBox.Contains(pos);
    }

    public float ElevationAtPos(GloLLPoint pos)
    {
        // Check if the position is within the bounds of the tile.
        if (!LLBox.Contains(pos))
            return GloElevationUtils.InvalidEle;

        // Calculate the fractional position within the tile, then use that to access the value.
        double fracLat = (pos.LatDegs - LLBox.MinLatDegs) / LLBox.DeltaLatDegs;
        double fracLon = (pos.LonDegs - LLBox.MinLonDegs) / LLBox.DeltaLonDegs;

        // Validate the fraction
        fracLat = GloValueUtils.LimitToRange(fracLat, 0, 1);
        fracLon = GloValueUtils.LimitToRange(fracLon, 0, 1);

        // invert the lat fraction, as the data is stored with the top left as 0,0
        fracLat = 1 - fracLat;

        // Always accessing X, Y
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
