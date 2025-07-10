
// using Godot;

// // KoreZeroOffsetOperations: Functions for converting between real-world values and the Godot GameEngine presentation.
// // - Real world is in mainly SI units, with other accessor operations. The Godot World in is units of kilometers, to best suit the full world presentation.
// // - The Godot axis system does not match the ECEF orientatino of teh Z axis, so this will see inverting.

// public struct KorePosV3
// {
//     public Vector3 Pos;
//     public Vector3 PosAbove;
//     public Vector3 PosNorth;
//     public Vector3 VecUp;
//     public Vector3 VecNorth;
// }

// public struct KoreEntityV3
// {
//     public Vector3 Pos;
//     public Vector3 PosAbove;
//     public Vector3 PosAhead;
//     public Vector3 PosNorth;
//     public Vector3 VecUp;
//     public Vector3 VecForward;
//     public Vector3 VecNorth;
// }

// public static class KoreZeroOffsetOperations
// {
//     // --------------------------------------------------------------------------------------------
//     // MARK: Bare Position - No Zero offsets
//     // --------------------------------------------------------------------------------------------

//     // KoreZeroOffsetOperations.RwToGeStruct(pos);
//     public static Vector3 RwToGe(double radiusM, double latDegs, double lonDegs)
//     {
//         // Scale the radius
//         double radiusGe = radiusM * KoreZeroOffset.RwToGeDistMultiplier;

//         // Convert the LLA to an XYZ - No offset for this function
//         GloLLAPoint llap = new GloLLAPoint() { LatDegs = latDegs, LonDegs = lonDegs, RadiusM = radiusGe };
//         GloXYZPoint p = llap.ToXYZ();

//         // Create a new vecotr3, with the Z axis inverted as Godot needs.
//         return new Vector3((float)p.X, (float)p.Y, (float)-p.Z);
//     }

//     public static Vector3 RwToGe(GloLLAPoint llap)
//     {
//         return RwToGe(llap.RadiusM, llap.LatDegs, llap.LonDegs);
//     }

//     // --------------------------------------------------------------------------------------------

//     // A GE position is a vec3 AS AN OFFSET FROM 0,0,0 position.
//     // 1 - We start by scaling that back into a real world offset.
//     // 2 - We apply the real world earth centre offset (still all in XYZ)
//     // 3 - We convert the XYZ into an LLA.

//     // KoreZeroOffsetOperations.GeToRw(pos);
//     public static GloLLAPoint GeToRw(Vector3 gePos)
//     {
//         // 1 - Scale the position back to real world
//         //     The GE position has a flipped Z axis, so we need to invert it back
//         double rwX = gePos.X * KoreZeroOffset.ReToRwDistMultiplier;
//         double rwY = gePos.Y * KoreZeroOffset.ReToRwDistMultiplier;
//         double rwZ = gePos.Z * KoreZeroOffset.ReToRwDistMultiplier * -1;

//         GloXYZPoint rwOffset = new GloXYZPoint(rwX, rwY, rwZ);

//         // 2 - Apply the real world earth centre offset
//         GloXYZPoint rwEarthCentrePos = KoreZeroOffset.RwZeroPointXYZ + rwOffset;

//         // 3 - Convert the XYZ into an LLA
//         GloLLAPoint llap = GloLLAPoint.FromXYZ(rwEarthCentrePos);

//         return llap;
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Bare Position - WITH Zero offsets
//     // --------------------------------------------------------------------------------------------

//     public static Vector3 RwToOffsetGe(GloLLPoint llPos) => KoreZeroOffsetOperations.RwToOffsetGe(new GloLLAPoint(llPos));

//     // Usage: Vector3 v3Pos = KoreZeroOffsetOperations.RwToOffsetGe(pos);
//     public static Vector3 RwToOffsetGe(GloLLAPoint pos)
//     {
//         //pos.RadiusM = ScaleDistance(pos.RadiusM);
//         return KoreZeroOffset.GeZeroPointOffset(pos.ToXYZ());
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Position with Orientation
//     // --------------------------------------------------------------------------------------------

//     public static KorePosV3 RwToGeStruct(GloLLPoint llPos) => KoreZeroOffsetOperations.RwToGeStruct(new GloLLAPoint(llPos));

//     // Usage: KorePosV3 posV3 = KoreZeroOffsetOperations.RwToGeStruct(Pos);
//     public static KorePosV3 RwToGeStruct(GloLLAPoint pos)
//     {
//         // Define the position and associated up direction for the label
//         GloLLAPoint posAbove = new GloLLAPoint() { LatDegs = pos.LatDegs,     LonDegs = pos.LonDegs, AltMslM = pos.AltMslM + KoreZeroOffset.UpDistRwM};
//         GloLLAPoint posNorth = new GloLLAPoint() { LatDegs = pos.LatDegs + 1, LonDegs = pos.LonDegs, AltMslM = pos.AltMslM};

//         // Define the aobsolye positions
//         Vector3 v3Pos        = KoreZeroOffset.GeZeroPointOffset(pos.ToXYZ());
//         Vector3 v3PosAbove   = KoreZeroOffset.GeZeroPointOffset(posAbove.ToXYZ());
//         Vector3 v3PosNorth   = KoreZeroOffset.GeZeroPointOffset(posNorth.ToXYZ());

//         // Define the relative vectors
//         Vector3 v3VecUp      = v3PosAbove - v3Pos;
//         Vector3 v3VecNorth   = v3PosNorth - v3Pos;

//         KorePosV3 retStruct = new KorePosV3 {
//             Pos        = v3Pos,
//             PosAbove   = v3PosAbove,
//             PosNorth   = v3PosNorth,
//             VecUp      = v3VecUp,
//             VecNorth   = v3VecNorth};

//         return retStruct;
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Position with Course and Orientation
//     // --------------------------------------------------------------------------------------------

//     // Usage: KoreEntityV3 platformV3 = KoreZeroOffsetOperations.RealWorldToStruct(PlatformPos, PlatformCourse);

//     public static KoreEntityV3 RwToGeStruct(GloLLAPoint pos, GloCourse course)
//     {
//         // Define the position and associated up direction for the label
//         GloLLAPoint posAbove = new GloLLAPoint() { LatDegs = pos.LatDegs,     LonDegs = pos.LonDegs, AltMslM = pos.AltMslM + KoreZeroOffset.UpDistRwM};
//         GloLLAPoint posNorth = new GloLLAPoint() { LatDegs = pos.LatDegs + 1, LonDegs = pos.LonDegs, AltMslM = pos.AltMslM};

//         // get the offset and ensure we have sufficient magnitude
//         GloRangeBearing aheadCourse = new GloRangeBearing() { RangeM = KoreZeroOffset.AheadDistGE, BearingDegs = course.HeadingDegs };
//         GloLLAPoint posAhead = pos.PlusRangeBearing(aheadCourse);

//         // Define the absolute positions
//         Vector3 v3Pos        = KoreZeroOffset.GeZeroPointOffset(pos.ToXYZ());
//         Vector3 v3PosAbove   = KoreZeroOffset.GeZeroPointOffset(posAbove.ToXYZ());
//         Vector3 v3PosAhead   = KoreZeroOffset.GeZeroPointOffset(posAhead.ToXYZ());
//         Vector3 v3PosNorth   = KoreZeroOffset.GeZeroPointOffset(posNorth.ToXYZ());

//         // Define the relative vectors
//         Vector3 v3VecUp      = (v3PosAbove - v3Pos).Normalized();
//         Vector3 v3VecForward = (v3PosAhead - v3Pos).Normalized();
//         Vector3 v3VecNorth   = (v3PosNorth - v3Pos).Normalized();

//         KoreEntityV3 retStruct = new KoreEntityV3 {
//             Pos        = v3Pos,
//             PosAbove   = v3PosAbove,
//             PosAhead   = v3PosAhead,
//             PosNorth   = v3PosNorth,
//             VecUp      = v3VecUp,
//             VecForward = v3VecForward,
//             VecNorth   = v3VecNorth};

//         return retStruct;
//     }























//     public static KoreEntityV3 RwToGeStruct(GloLLAPoint pos, double HeadingDegs)
//     {
//         // Translate the core position to the GE offset vector3
//         Vector3 v3Pos        = KoreZeroOffset.GeZeroPointOffset(pos.ToXYZ());

//         // Define the position and associated up direction for the label
//         GloLLAPoint posAbove = new GloLLAPoint() { LatDegs = pos.LatDegs, LonDegs = pos.LonDegs, AltMslM = pos.AltMslM + KoreZeroOffset.UpDistRwM};
//         Vector3 v3PosAbove   = KoreZeroOffset.GeZeroPointOffset(posAbove.ToXYZ());

//         // get the offset and ensure we have sufficient magnitude
//         GloRangeBearing aheadCourse = new GloRangeBearing() { RangeM = KoreZeroOffset.AheadDistGE, BearingDegs = HeadingDegs };
//         GloLLAPoint posAhead = pos.PlusRangeBearing(aheadCourse);
//         Vector3 v3PosAhead   = KoreZeroOffset.GeZeroPointOffset(posAhead.ToXYZ());

//         {
//             GloXYZPoint p      = pos.ToXYZ();
//             GloXYZPoint pAhead = posAhead.ToXYZ();
//             GloXYZPoint pAbove = posAbove.ToXYZ();

//             double distUp    = p.DistanceTo(pAbove);
//             double distAhead = p.DistanceTo(pAhead);

//             // GD.Print($"V3Debug2\n- Pos:{p}\n- PosAbove:{pAbove} // DistUp:{distUp}\n- PosAhead:{pAhead} // DistAhead:{distAhead}");
//         }


//         // Define the relative vectors
//         Vector3 v3VecUp      = (v3PosAbove - v3Pos).Normalized();
//         Vector3 v3VecForward = (v3PosAhead - v3Pos).Normalized();

//         // Find the north position
//         GloLLAPoint posNorth = GloLLAPoint.Zero;
//         Vector3 v3PosNorth   = Vector3.Zero;
//         Vector3 v3VecNorth   = Vector3.Zero;
//         if (pos.LatDegs < 89.89)
//         {
//             posNorth   = new GloLLAPoint() { LatDegs = pos.LatDegs + 0.1, LonDegs = pos.LonDegs, AltMslM = pos.AltMslM};
//             v3PosNorth = KoreZeroOffset.GeZeroPointOffset(posNorth.ToXYZ());
//             v3VecNorth   = (v3PosNorth - v3Pos).Normalized();
//         }

//         KoreEntityV3 retStruct = new KoreEntityV3 {
//             Pos        = v3Pos,
//             PosAbove   = v3PosAbove,
//             PosAhead   = v3PosAhead,
//             PosNorth   = v3PosNorth,
//             VecUp      = v3VecUp,
//             VecForward = v3VecForward,
//             VecNorth   = v3VecNorth};

//         return retStruct;
//     }
































//     // --------------------------------------------------------------------------------------------

//     // Usage: KoreEntityV3 platformV3 = KoreZeroOffsetOperations.RwToGeStruct(currPos, futurePos);

//     public static KoreEntityV3 RwToGeStruct(GloLLAPoint frompos, GloLLAPoint topos)
//     {
//         // Define the position and associated up direction for the label
//         GloLLAPoint posAbove = frompos;
//         posAbove.AltMslM += KoreZeroOffset.UpDistRwM;

//         // Define the position and associated up direction for the label
//         GloLLAPoint posNorth = frompos;
//         posNorth.LatDegs += 1.01f;

//         // Define the aobsolye positions
//         Vector3 v3Pos        = KoreZeroOffset.GeZeroPointOffset(frompos.ToXYZ());
//         Vector3 v3PosAbove   = KoreZeroOffset.GeZeroPointOffset(posAbove.ToXYZ());
//         Vector3 v3PosAhead   = KoreZeroOffset.GeZeroPointOffset(topos.ToXYZ());
//         Vector3 v3PosNorth   = KoreZeroOffset.GeZeroPointOffset(posNorth.ToXYZ());

//         // Define the relative vectors
//         Vector3 v3VecUp      = v3PosAbove - v3Pos;
//         Vector3 v3VecForward = v3PosAhead - v3Pos;
//         Vector3 v3VecNorth   = v3PosNorth - v3Pos;

//         KoreEntityV3 retStruct = new KoreEntityV3 {
//             Pos        = v3Pos,
//             PosAbove   = v3PosAbove,
//             PosAhead   = v3PosAhead,
//             PosNorth   = v3PosNorth,
//             VecUp      = v3VecUp,
//             VecForward = v3VecForward,
//             VecNorth   = v3VecNorth};

//         return retStruct;
//     }

//     // --------------------------------------------------------------------------------------------

//     public static void DebugV3(Vector3 pos, Vector3 posAhead, Vector3 posAbove)
//     {
//         double distUp    = pos.DistanceTo(posAbove);
//         double distAhead = pos.DistanceTo(posAhead);

//         GD.Print($"V3Debug\n- Pos:{pos}\n- PosAbove:{posAbove} // DistUp:{distUp}\n- PosAhead:{posAhead} // DistAhead:{distAhead}");
//     }

// }