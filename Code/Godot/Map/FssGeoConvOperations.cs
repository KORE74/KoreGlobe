
using Godot;

// FssGeoConvOperations: Functions for converting between real-world values and the Godot GameEngine presentation.
// - Real world is in mainly SI units, with other accessor operations. The Godot World in is units of kilometers, to best suit the full world presentation.
// - The Godot axis system does not match the ECEF orientatino of teh Z axis, so this will see inverting.

public struct FssPosV3
{
    public Vector3 Pos;
    public Vector3 PosAbove;
    public Vector3 PosNorth;
    public Vector3 VecUp;
    public Vector3 VecNorth;
}

public struct FssEntityV3
{
    public Vector3 Pos;
    public Vector3 PosAbove;
    public Vector3 PosAhead;
    public Vector3 PosNorth;
    public Vector3 VecUp;
    public Vector3 VecForward;
    public Vector3 VecNorth;
}

public static class FssGeoConvOperations
{
    // --------------------------------------------------------------------------------------------
    // MARK: Elevation conversion
    // --------------------------------------------------------------------------------------------

    // routine to take a model elevation in meters, and ouput a scaled elevation in km, in propoortion
    // to the current presentation range FssZeroOffset.GeEarthRadius.
    public static float ScaleElevation(double modelEleM)
    {
        // Determine model to presentation scale factor
        double scaleMetersPerDisplayUnit = FssPosConsts.EarthRadiusM / FssZeroOffset.GeEarthRadius;

        return (float)(modelEleM / scaleMetersPerDisplayUnit);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Bare Position - No Zero offsets
    // --------------------------------------------------------------------------------------------

    // FssGeoConvOperations.RwToGeStruct(pos);
    public static Vector3 RwToGe(double radiusM, double latDegs, double lonDegs)
    {
        // Scale the radius
        radiusM = radiusM * FssZeroOffset.RwToGeDistanceMultiplierM;

        // Convert the LLA to an XYZ - No offset for this function
        FssLLAPoint llap = new FssLLAPoint() { LatDegs = latDegs, LonDegs = lonDegs, RadiusM = radiusM };
        FssXYZPoint p = llap.ToXYZ();

        // Create a new vecotr3, with teh Z axis inverted as Godot needs.
        return new Vector3((float)p.X, (float)p.Y, (float)-p.Z);
    }

    public static Vector3 RwToGe(FssLLAPoint llap)
    {
        return RwToGe(llap.RadiusM, llap.LatDegs, llap.LonDegs);
    }

    // --------------------------------------------------------------------------------------------

    // A GE position is a vec3 AS AN OFFSET FROM TEH 0,0,0 position.
    // 1 - We start by scaling that back into a real world offset.
    // 2 - We apply the real world earth centre offset (still all in XYZ)
    // 3 - We convert the XYZ into an LLA.

    // FssGeoConvOperations.GeToRw(pos);
    public static FssLLAPoint GeToRw(Vector3 gePos)
    {
        // 1 - Scale the position back to real world
        //     The GE position has a flipped Z axis, so we need to invert it back
        double rwX = gePos.X * FssZeroOffset.GeToRwDistanceMultiplierM;
        double rwY = gePos.Y * FssZeroOffset.GeToRwDistanceMultiplierM;
        double rwZ = gePos.Z * FssZeroOffset.GeToRwDistanceMultiplierM * -1;

        FssXYZPoint rwOffset = new FssXYZPoint(rwX, rwY, rwZ);

        // 2 - Apply the real world earth centre offset
        FssXYZPoint rwEarthCentrePos = FssZeroOffset.RwZeroPointXYZ + rwOffset;

        // 3 - Convert the XYZ into an LLA
        FssLLAPoint llap = FssLLAPoint.FromXYZ(rwEarthCentrePos);

        return llap;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Bare Position - WITH Zero offsets
    // --------------------------------------------------------------------------------------------

    // Usage: Vector3 v3Pos = FssGeoConvOperations.RwToOffsetGe(pos);
    public static Vector3 RwToOffsetGe(FssLLAPoint pos)
    {
        //pos.RadiusM = ScaleElevation(pos.RadiusM);
        return FssZeroOffset.GeZeroPointOffset(pos.ToXYZ());
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Position with Orientation
    // --------------------------------------------------------------------------------------------

    // Usage: FssPosV3 posV3 = FssGeoConvOperations.RwToGeStruct(Pos);
    public static FssPosV3 RwToGeStruct(FssLLAPoint pos)
    {
        // Define the position and associated up direction for the label
        FssLLAPoint posAbove = new FssLLAPoint() { LatDegs = pos.LatDegs,     LonDegs = pos.LonDegs, AltMslM = pos.AltMslM + FssZeroOffset.UpDistRwM};
        FssLLAPoint posNorth = new FssLLAPoint() { LatDegs = pos.LatDegs + 1, LonDegs = pos.LonDegs, AltMslM = pos.AltMslM};

        // Define the aobsolye positions
        Vector3 v3Pos        = FssZeroOffset.GeZeroPointOffset(pos.ToXYZ());
        Vector3 v3PosAbove   = FssZeroOffset.GeZeroPointOffset(posAbove.ToXYZ());
        Vector3 v3PosNorth   = FssZeroOffset.GeZeroPointOffset(posNorth.ToXYZ());

        // Define the relative vectors
        Vector3 v3VecUp      = v3PosAbove - v3Pos;
        Vector3 v3VecNorth   = v3PosNorth - v3Pos;

        FssPosV3 retStruct = new FssPosV3 {
            Pos        = v3Pos,
            PosAbove   = v3PosAbove,
            PosNorth   = v3PosNorth,
            VecUp      = v3VecUp,
            VecNorth   = v3VecNorth};

        return retStruct;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Position with Course and Orientation
    // --------------------------------------------------------------------------------------------

    // Usage: FssEntityV3 platformV3 = FssGeoConvOperations.RealWorldToStruct(PlatformPos, PlatformCourse);

    public static FssEntityV3 RwToGeStruct(FssLLAPoint pos, FssCourse course)
    {
        // Define the position and associated up direction for the label
        FssLLAPoint posAbove = new FssLLAPoint() { LatDegs = pos.LatDegs,     LonDegs = pos.LonDegs, AltMslM = pos.AltMslM + FssZeroOffset.UpDistRwM};
        FssLLAPoint posNorth = new FssLLAPoint() { LatDegs = pos.LatDegs + 1, LonDegs = pos.LonDegs, AltMslM = pos.AltMslM};

        // get the offset and ensure we have sufficient magnitude
        FssRangeBearing aheadCourse = new FssRangeBearing() { RangeM = FssZeroOffset.AheadDistGeM, BearingDegs = course.HeadingDegs };
        FssLLAPoint posAhead = pos.PlusRangeBearing(aheadCourse);

        // Define the absolute positions
        Vector3 v3Pos        = FssZeroOffset.GeZeroPointOffset(pos.ToXYZ());
        Vector3 v3PosAbove   = FssZeroOffset.GeZeroPointOffset(posAbove.ToXYZ());
        Vector3 v3PosAhead   = FssZeroOffset.GeZeroPointOffset(posAhead.ToXYZ());
        Vector3 v3PosNorth   = FssZeroOffset.GeZeroPointOffset(posNorth.ToXYZ());

        // Define the relative vectors
        Vector3 v3VecUp      = (v3PosAbove - v3Pos).Normalized();
        Vector3 v3VecForward = (v3PosAhead - v3Pos).Normalized();
        Vector3 v3VecNorth   = (v3PosNorth - v3Pos).Normalized();

        FssEntityV3 retStruct = new FssEntityV3 {
            Pos        = v3Pos,
            PosAbove   = v3PosAbove,
            PosAhead   = v3PosAhead,
            PosNorth   = v3PosNorth,
            VecUp      = v3VecUp,
            VecForward = v3VecForward,
            VecNorth   = v3VecNorth};

        return retStruct;
    }























    public static FssEntityV3 RwToGeStruct(FssLLAPoint pos, double HeadingDegs)
    {
        // Translate the core position to the GE offset vector3
        Vector3 v3Pos        = FssZeroOffset.GeZeroPointOffset(pos.ToXYZ());

        // Define the position and associated up direction for the label
        FssLLAPoint posAbove = new FssLLAPoint() { LatDegs = pos.LatDegs, LonDegs = pos.LonDegs, AltMslM = pos.AltMslM + FssZeroOffset.UpDistRwM};
        Vector3 v3PosAbove   = FssZeroOffset.GeZeroPointOffset(posAbove.ToXYZ());

        // get the offset and ensure we have sufficient magnitude
        FssRangeBearing aheadCourse = new FssRangeBearing() { RangeM = FssZeroOffset.AheadDistGeM, BearingDegs = HeadingDegs };
        FssLLAPoint posAhead = pos.PlusRangeBearing(aheadCourse);
        Vector3 v3PosAhead   = FssZeroOffset.GeZeroPointOffset(posAhead.ToXYZ());

        {
            FssXYZPoint p      = pos.ToXYZ();
            FssXYZPoint pAhead = posAhead.ToXYZ();
            FssXYZPoint pAbove = posAbove.ToXYZ();

            double distUp    = p.DistanceTo(pAbove);
            double distAhead = p.DistanceTo(pAhead);

            // GD.Print($"V3Debug2\n- Pos:{p}\n- PosAbove:{pAbove} // DistUp:{distUp}\n- PosAhead:{pAhead} // DistAhead:{distAhead}");
        }


        // Define the relative vectors
        Vector3 v3VecUp      = (v3PosAbove - v3Pos).Normalized();
        Vector3 v3VecForward = (v3PosAhead - v3Pos).Normalized();

        // Find the north position
        FssLLAPoint posNorth = FssLLAPoint.Zero;
        Vector3 v3PosNorth   = Vector3.Zero;
        Vector3 v3VecNorth   = Vector3.Zero;
        if (pos.LatDegs < 89.89)
        {
            posNorth   = new FssLLAPoint() { LatDegs = pos.LatDegs + 0.1, LonDegs = pos.LonDegs, AltMslM = pos.AltMslM};
            v3PosNorth = FssZeroOffset.GeZeroPointOffset(posNorth.ToXYZ());
            v3VecNorth   = (v3PosNorth - v3Pos).Normalized();
        }

        FssEntityV3 retStruct = new FssEntityV3 {
            Pos        = v3Pos,
            PosAbove   = v3PosAbove,
            PosAhead   = v3PosAhead,
            PosNorth   = v3PosNorth,
            VecUp      = v3VecUp,
            VecForward = v3VecForward,
            VecNorth   = v3VecNorth};

        return retStruct;
    }
































    // --------------------------------------------------------------------------------------------

    // Usage: FssEntityV3 platformV3 = FssGeoConvOperations.RwToGeStruct(currPos, futurePos);

    public static FssEntityV3 RwToGeStruct(FssLLAPoint frompos, FssLLAPoint topos)
    {
        // Define the position and associated up direction for the label
        FssLLAPoint posAbove = frompos;
        posAbove.AltMslM += FssZeroOffset.UpDistRwM;

        // Define the position and associated up direction for the label
        FssLLAPoint posNorth = frompos;
        posNorth.LatDegs += 1.01f;

        // Define the aobsolye positions
        Vector3 v3Pos        = FssZeroOffset.GeZeroPointOffset(frompos.ToXYZ());
        Vector3 v3PosAbove   = FssZeroOffset.GeZeroPointOffset(posAbove.ToXYZ());
        Vector3 v3PosAhead   = FssZeroOffset.GeZeroPointOffset(topos.ToXYZ());
        Vector3 v3PosNorth   = FssZeroOffset.GeZeroPointOffset(posNorth.ToXYZ());

        // Define the relative vectors
        Vector3 v3VecUp      = v3PosAbove - v3Pos;
        Vector3 v3VecForward = v3PosAhead - v3Pos;
        Vector3 v3VecNorth   = v3PosNorth - v3Pos;

        FssEntityV3 retStruct = new FssEntityV3 {
            Pos        = v3Pos,
            PosAbove   = v3PosAbove,
            PosAhead   = v3PosAhead,
            PosNorth   = v3PosNorth,
            VecUp      = v3VecUp,
            VecForward = v3VecForward,
            VecNorth   = v3VecNorth};

        return retStruct;
    }

    // --------------------------------------------------------------------------------------------

    public static void DebugV3(Vector3 pos, Vector3 posAhead, Vector3 posAbove)
    {
        double distUp    = pos.DistanceTo(posAbove);
        double distAhead = pos.DistanceTo(posAhead);

        GD.Print($"V3Debug\n- Pos:{pos}\n- PosAbove:{posAbove} // DistUp:{distUp}\n- PosAhead:{posAhead} // DistAhead:{distAhead}");
    }

}
