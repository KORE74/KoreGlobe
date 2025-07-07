


public class GloElevationTile
{
    public GloLLBox        LLBox;
    public GloFloat2DArray ElevationData;
    public GloMapTileCode  TileCode = new();

    // Zero tile
    public static GloElevationTile Zero
    {
        get { return new GloElevationTile(); }
    }

    public float GetElevation(double latDegs, double lonDegs)
    {
        GloLLPoint pos = new() { LatDegs = latDegs, LonDegs = lonDegs };

        if (LLBox.Contains(pos))
        {
            float latFrac;
            float lonFrac;

            (latFrac, lonFrac) = LLBox.GetLatLonFraction(pos);
            float eleAtFraction = ElevationData.InterpolatedValue(lonFrac, latFrac); // lon first, X Y.

            return eleAtFraction;
        }
        return GloElevationUtils.InvalidEle;
    }


}
