


public class FssElevationTile
{
    public FssMapTileCode  TileCode       = FssMapTileCode.Zero;
    public FssLLBox        LLBox          = FssLLBox.Zero;
    public FssFloat2DArray ElevationData;

    // Zero tile
    public static FssElevationTile Zero
    {
        get { return new FssElevationTile(); }
    }

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

    public bool Contains(FssLLPoint pos) => LLBox.Contains(pos);

    public float ElevationAtPos(FssLLPoint pos)
    {
        // Check if the position is within the bounds of the tile.
        if (!LLBox.Contains(pos))
            return FssElevationConsts.InvalidEle;

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