

#nullable enable

public class FssElevationTile
{
    public FssFloat2DArray ElevationData { get; private set; } = new FssFloat2DArray();
    public FssLLBox        LLBox         { get; private set; } = FssLLBox.ZeroBox;

    // --------------------------------------------------------------------------------------------
    // MARK: Resolution
    // --------------------------------------------------------------------------------------------

    // The lat long box will have a resolution in degrees. The 2D array will have a number of points.
    // GetRes calculates and returns the lowest number of points per degree in the lat long box.

    public float TileRes()
    {
        int numLatPts = ElevationData.NumRows;
        int numLonPts = ElevationData.NumCols;

        float latRes = LLBox.DeltaLatDegs / numLatPts;
        float lonRes = LLBox.DeltaLonDegs / numLonPts;

        return FssValueUtils.Min(latRes, lonRes);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Elevation for Position
    // --------------------------------------------------------------------------------------------

    public bool Contains(FssLLPoint pos)
    {
        return LLBox.Contains(pos);
    }

    public float? ElevationAtPos(FssLLPoint pos)
    {
        // Check if the position is within the bounds of the tile.
        if (!LLBox.Contains(pos))
            return null;

        // Calculate the fractional position within the tile, then use that to access the value.
        double fracLat = (pos.LatDegs - LLBox.MinLatDegs) / LLBox.DeltaLatDegs;
        double fracLon = (pos.LonDegs - LLBox.MinLonDegs) / LLBox.DeltaLonDegs;

        // Validate the fraction
        fracLat = FssValueUtils.LimitToRange(fracLat, 0, 1);
        fracLon = FssValueUtils.LimitToRange(fracLon, 0, 1);

        return ElevationData.InterpolatedValue((float)fracLon, (float)fracLat);
    }



}