

#nullable enable

public class FssElevationTile
{
    public FssFloat2DArray ElevationData { get; private set; }
    public FssLLBox        LLBox         { get; private set; }
    public int             HorizRes      { get; private set; }
    public int             VertRes       { get; private set; }
    public double          HorizResDegs  { get; private set; }
    public double          VertResDegs   { get; private set; }

    // --------------------------------------------------------------------------------------------
    // MARK: Initialization
    // --------------------------------------------------------------------------------------------

    public FssElevationTile(FssFloat2DArray elevationData, FssLLBox llBox, int horizRes, int vertRes)
    {
        ElevationData = elevationData;
        LLBox         = llBox;
        HorizRes      = horizRes;
        VertRes       = vertRes;

        HorizResDegs = llBox.DeltaLonDegs / horizRes;
        VertResDegs  = llBox.DeltaLatDegs / vertRes;
    }

    // Empty constructor for subsequent population

    public FssElevationTile(FssLLBox llBox, int horizRes, int vertRes)
    {
        ElevationData = new FssFloat2DArray();
        LLBox         = llBox;
        HorizRes      = horizRes;
        VertRes       = vertRes;
        HorizResDegs  = llBox.DeltaLonDegs / horizRes;
        VertResDegs   = llBox.DeltaLatDegs / vertRes;
    }

    // --------------------------------------------------------------------------------------------

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