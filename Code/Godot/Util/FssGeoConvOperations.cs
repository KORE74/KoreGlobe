
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
    // MARK: Bare Position - No Zero offsets
    // --------------------------------------------------------------------------------------------

    // FssGeoConvOperations.RwToGeStruct(pos);
    public static Vector3 RwToGe(double radiusM, double latDegs, double lonDegs)
    {
        FssLLAPoint llap = new FssLLAPoint() { LatDegs = latDegs, LonDegs = lonDegs, RadiusM = radiusM };
        FssXYZPoint p = llap.ToXYZ();
        return new Vector3((float)p.X, (float)p.Y, (float)-p.Z);
    }

    public static Vector3 RwToGe(FssLLAPoint llap)
    {
        FssXYZPoint p = llap.ToXYZ();
        return new Vector3((float)p.X, (float)p.Y, (float)-p.Z);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Bare Position - WITH Zero offsets
    // --------------------------------------------------------------------------------------------

    // Usage: Vector3 v3Pos = FssGeoConvOperations.RwToOffsetGe(pos);
    public static Vector3 RwToOffsetGe(FssLLAPoint pos)
    {
        return FssZeroOffset.GeZeroPointOffset(pos.ToXYZ());
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Position with Orientation
    // --------------------------------------------------------------------------------------------

    // Usage: FssPosV3 posV3 = FssGeoConvOperations.RwToGeStruct(Pos);
    public static FssPosV3 RwToGeStruct(FssLLAPoint pos)
    {
        // Define the position and associated up direction for the label
        FssLLAPoint posAbove = pos;
        posAbove.AltMslM += 0.04f;

        // Define the position and associated up direction for the label
        FssLLAPoint posNorth = pos;
        posAbove.LatDegs += 0.01f;

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
        FssLLAPoint posAbove = pos;
        posAbove.AltMslM += 0.04f;

        // Define the position and associated up direction for the label
        FssLLAPoint posNorth = pos;
        posNorth.LatDegs += 0.01f;

        // Get the position 5 seconds ahead, or just north if stationary
        FssLLAPoint posAhead = FssLLAPoint.Zero;
        if (course.IsStationary())
        {
            posAhead.LatDegs += 0.001;
        }
        else
        {
            // get the offset and ensure we have sufficient magnitude
            FssPolarOffset aheadCourse = course.ToPolarOffset(5);
            if (aheadCourse.RangeM < 0.1)
                aheadCourse.RangeM = 0.1;

            posAhead = pos.PlusPolarOffset(aheadCourse); // The course, 5 seconds ahead
        }

        // Define the aobsolye positions
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


    // Usage: FssEntityV3 platformV3 = FssGeoConvOperations.RwToGeStruct(currPos, futurePos);

    public static FssEntityV3 RwToGeStruct(FssLLAPoint frompos, FssLLAPoint topos)
    {
        // Define the position and associated up direction for the label
        FssLLAPoint posAbove = frompos;
        posAbove.AltMslM += 0.04f;

        // Define the position and associated up direction for the label
        FssLLAPoint posNorth = frompos;
        posNorth.LatDegs += 0.01f;

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

}
