using System;
using System.Collections.Generic;

// The struct FssLLAPoint stores a Lat Long Altitude (LLA) position and calculates various new positions, angles and offsets related to it.
// Data is stored in SI units and additional accessor properties allow setting and getting in different units.
// An associates FssXYZPoint class defines a position in an ENU, XYZ format. An associates FssConsts class hold constants regarding conversion and earth radius.

// Design Decisions:
// - The code uses a struct rather than an immutable class, as the constructor options with flexible units
//   are simply too useful. We rely on the struct's pass by value to avoid issues with mutability.

public struct FssLLAPoint
{
    // SI Units and a pure radius value so the trig functions are simple.
    // Accomodate units and MSL during accessor functions

    public double LatRads { get; set; }
    public double LonRads { get; set; }
    public double RadiusM { get; set; } // Alt above EarthCentre

    // --------------------------------------------------------------------------------------------
    // Additional simple accessors - adding units
    // --------------------------------------------------------------------------------------------

    public double LatDegs
    {
        get { return LatRads * FssConsts.RadsToDegsMultiplier; }
        set { LatRads = value * FssConsts.DegsToRadsMultiplier; }
    }
    public double LonDegs
    {
        get { return LonRads * FssConsts.RadsToDegsMultiplier; }
        set { LonRads = value * FssConsts.DegsToRadsMultiplier; }
    }
    public double AltMslKm // Alt above MSL
    {
        get { return (RadiusM - FssPosConsts.EarthRadiusM) * FssPosConsts.MetresToKmMultiplier; }
        set { RadiusM = (value + FssPosConsts.EarthRadiusKm) * FssPosConsts.KmToMetresMultiplier; }
    }
    public double AltMslM // Alt above M
    {
        get { return (RadiusM - FssPosConsts.EarthRadiusM); }
        set { RadiusM = value + FssPosConsts.EarthRadiusM; }
    }
    public double RadiusKm // Alt above EarthRadius
    {
        get { return (RadiusM * FssPosConsts.MetresToKmMultiplier); }
        set { RadiusM = (value * FssPosConsts.KmToMetresMultiplier); }
    }

    public override string ToString()
    {
        return string.Format($"(LatDegs:{LatDegs:F2}, LonDegs:{LonDegs:F2}, AltMslM:{AltMslM:F2})");
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Constructors - different options and units
    // --------------------------------------------------------------------------------------------

    // Note that fields can be set:
    //   FssLLAPoint pos = new FssLLAPoint() { latDegs = X, LonDegs = Y, AltMslM = Z };

    public FssLLAPoint(double laRads, double loRads, double altM)
    {
        this.LatRads = laRads;
        this.LonRads = loRads;
        this.RadiusM = altM;
    }

    public FssLLAPoint(double laRads, double loRads)
    {
        this.LatRads = laRads;
        this.LonRads = loRads;
        this.RadiusM = FssPosConsts.EarthRadiusM;
    }

    public FssLLAPoint(FssLLPoint llPos)
    {
        this.LatRads = llPos.LatRads;
        this.LonRads = llPos.LonRads;
        this.RadiusM = FssPosConsts.EarthRadiusM;
    }

    public static FssLLAPoint Zero
    {
        get { return new FssLLAPoint { LatRads = 0.0, LonRads = 0.0, RadiusM = 0.0 }; }
    }

    // Function to return an LLA position on the ground, either using the default MSL value, or an optional elevation above MSL in metres.

    public FssLLAPoint Grounded(double? ElevAboveMslM = null)
    {
        // Default to MSL radius
        double radius = FssPosConsts.EarthRadiusM;

        // If the ElevAboveMslM value is provided, use it added to earth radius
        if (ElevAboveMslM.HasValue)
            radius = FssPosConsts.EarthRadiusM + ElevAboveMslM.Value;

        // Return new "grounded" position
        return new FssLLAPoint(LatRads, LonRads, radius);
    }

    public FssLLAPoint WithMinAltMslM(double minAltMslM)
    {
        double retElev = Math.Max(FssPosConsts.EarthRadiusM + minAltMslM, AltMslM);
        return new FssLLAPoint(LatRads, LonRads, retElev);
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Conversion
    // --------------------------------------------------------------------------------------------

    public FssXYZPoint ToXYZ()
    {
        FssXYZPoint retXYZ = new FssXYZPoint(
            RadiusM * Math.Cos(LatRads) * Math.Cos(LonRads),
            RadiusM * Math.Sin(LatRads),
            RadiusM * Math.Cos(LatRads) * Math.Sin(LonRads));
        return retXYZ;
    }

    // Usage: FssLLAPoint pos = FssLLAPoint.FromXYZ(xyz);
    public static FssLLAPoint FromXYZ(FssXYZPoint inputXYZ)
    {
        double radius = inputXYZ.Magnitude;
        double latRads = Math.Asin(inputXYZ.Y / radius);
        double lonRads = Math.Atan2(inputXYZ.Z, inputXYZ.X);
        return new FssLLAPoint(latRads, lonRads, radius);
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Distance
    // --------------------------------------------------------------------------------------------

    // Nullable default parameter allows it to assume the altitude for the calculation
    // is the average of the two points, unless a value (such a zero "do calc at ground
    // level") is supplied.
    // The curved distance can be at MSL if the Alt parameter is zero, will otherwise
    // use an average to the two input altitudes.

    public double CurvedDistanceToM(FssLLAPoint destPos, double? AtAltMslM = null)
    {
        // If a specific altitude is provided, use it for the radius calculation.
        // Otherwise, use the average of the radii from the Earth's center to each point.
        double radius = FssPosConsts.EarthRadiusM;
        if (AtAltMslM.HasValue)
            radius = AtAltMslM.Value;
        else
            radius = (this.RadiusM + destPos.RadiusM) / 2;

        // Calculate the difference in latitude and longitude between the two points.
        // The AngleDiffRads function ensures the angle difference wraps correctly
        // around the Fss, handling the transition across the 180-degree meridian.
        double dLat = FssValueUtils.AngleDiffRads(destPos.LatRads, this.LatRads);
        double dLon = FssValueUtils.AngleDiffRads(destPos.LonRads, this.LonRads);

        // Compute the components of the haversine formula:
        // 'a' calculates the square of half the chord length between the points.
        // 'c' is the angular distance in radians, derived from 'a'.
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(this.LatRads) * Math.Cos(destPos.LatRads) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // The final distance is the product of the radius and the angular distance.
        // This gives the curved (arc) distance along the surface of the sphere.
        double distance = radius * c;

        return distance;
    }

    public double StraightLineDistanceToM(FssLLAPoint destPos)
    {
        FssXYZPoint startXYZ = this.ToXYZ();
        FssXYZPoint destXYZ = destPos.ToXYZ();
        return startXYZ.DistanceTo(destXYZ);
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Elevation
    // --------------------------------------------------------------------------------------------

    // The elevation to a distant point. Useful in aiming a camera.

    public double ElevationToRads(FssLLAPoint destPos)
    {
        // Calculate horizontal distance
        double horizontalDistance = this.CurvedDistanceToM(destPos);

        // Calculate altitude difference
        double altitudeDifference = destPos.AltMslM - this.AltMslM; // Assuming AltMslM is the altitude property in FssLLAPoint

        // Calculate the elevation angle in radians
        double elevationRads = Math.Atan2(altitudeDifference, horizontalDistance);

        // Return the elevation angle
        return elevationRads * -1;
    }

    /// <summary>
    /// Updates the altitude of this FssLLAPoint point to result in a specific elevation angle when viewed from another point.
    /// The elevation angle is specified in radians, where 0 radians represents no elevation change (horizontal line of sight),
    /// negative values represent a downward angle, and positive values represent an upward angle relative to the local plane.
    /// </summary>
    /// <param name="relativetoPos">The FssLLAPoint point from which the elevation is measured.</param>
    /// <param name="elevRads">The desired elevation angle in radians. Negative for downward, positive for upward.</param>
    /// <returns>The updated FssLLAPoint object with adjusted altitude.</returns>
    public FssLLAPoint WithElevationRads(FssLLAPoint relativetoPos, double elevRads)
    {
        // Step 1: Calculate the horizontal distance
        double horizontalDistance = this.CurvedDistanceToM(relativetoPos, null);

        // Step 2: Calculate the altitude difference
        double altitudeDifference = Math.Tan(elevRads) * horizontalDistance;

        // Step 3: Update the altitude of this point
        // Assuming AltMslM is the altitude above mean sea level
        this.AltMslM = relativetoPos.AltMslM + altitudeDifference;

        return this; // Return the modified object
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Horizon
    // --------------------------------------------------------------------------------------------

    // Calculate the distance to the horizon from an altitude above mean sea level.
    public double DistanceToHorizonM()
    {
        // Earth's radius in meters (mean radius)
        double earthRadiusM = FssPosConsts.EarthRadiusM;
        double altMslM = this.AltMslM;

        // Calculate the distance to the horizon using the formula:
        // distanceM = sqrt(2 * earthRadiusM * altMslM + altMslM^2)
        double distanceM = Math.Sqrt(2 * earthRadiusM * altMslM + altMslM * altMslM);

        return distanceM;
    }

    // Calculate the angle below horizontal (dip angle) to the horizon.
    // This is useful for aiming cameras or sensors from an altitude.
    public double HorizonElevationRads()
    {
        // Calculate altitude above mean sea level
        double altMslM = RadiusM - FssPosConsts.EarthRadiusM;

        // Use the existing method to compute the horizon distance
        double horizonDistanceM = DistanceToHorizonM();

        // Calculate the dip angle (in radians)
        double horizonAngleRads = Math.Atan(horizonDistanceM / RadiusM);

        return horizonAngleRads;
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Azimuth
    // --------------------------------------------------------------------------------------------

    public double BearingToRads(FssLLAPoint destPos)
    {
        // Convert latitudes and longitudes from degrees to radians
        double lat1 = this.LatRads;
        double lon1 = this.LonRads;
        double lat2 = destPos.LatRads;
        double lon2 = destPos.LonRads;

        // Calculate difference in coordinates
        double dLon = lon2 - lon1;

        // Calculate bearing
        double y = Math.Sin(dLon) * Math.Cos(lat2);
        double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
        double bearingRads = Math.Atan2(y, x);

        // Ensure bearing is between 0 and 2pi
        bearingRads = (bearingRads + 2 * Math.PI) % (2 * Math.PI);

        return bearingRads;
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Range Bearing
    // --------------------------------------------------------------------------------------------

    // Use haversine formula to calculate distance

    public FssRangeBearing RangeBearingTo(FssLLAPoint destPos)
    {
        double lat1 = this.LatRads;
        double lon1 = this.LonRads;
        double lat2 = destPos.LatRads;
        double lon2 = destPos.LonRads;

        double dLat = lat2 - lat1;
        double dLon = lon2 - lon1;

        double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dLon / 2), 2);

        double distanceM = 2 * FssPosConsts.EarthRadiusM * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)); // in meters

        // Calculate bearing
        double y = Math.Sin(dLon) * Math.Cos(lat2);
        double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
        double bearingRads = Math.Atan2(y, x);

        return new FssRangeBearing { RangeM = distanceM, BearingRads = bearingRads };
    }

    public FssLLAPoint PlusRangeBearing(FssRangeBearing inputRB)
    {
        // Setup working variables
        double LatRads          = this.LatRads;
        double LonRads          = this.LonRads;
        double CalcRadius       = this.RadiusM;
        double InputRangeM      = inputRB.RangeM;
        double InputBearingRads = inputRB.BearingRads;

        // Shortcut calculations, avoid repetition.
        double SinLatRads           = Math.Sin(LatRads);
        double RangeDividedByRadius = InputRangeM / CalcRadius;

        double NewLatRads =
            Math.Asin(
                SinLatRads * Math.Cos(RangeDividedByRadius) +
                Math.Cos(LatRads) * Math.Sin(RangeDividedByRadius) *
                Math.Cos(InputBearingRads)
            );

        double NewLonRads =
            LonRads + Math.Atan2(
                Math.Sin(InputBearingRads) * Math.Sin(RangeDividedByRadius) * Math.Cos(LatRads),
                Math.Cos(RangeDividedByRadius) - SinLatRads * Math.Sin(NewLatRads)
            );

        return new FssLLAPoint(NewLatRads, NewLonRads, this.RadiusM);
    }

    // --------------------------------------------------------------------------------------------

    public FssLLAPoint PlusDeltaForTime(FssCourse course, double timeSecs)
    {
        // Turn the course into a range bearing distance (for the time duration) that we can apply)
        double distM = course.GroundSpeedMps * timeSecs;
        FssRangeBearing rb = new FssRangeBearing() { RangeM = distM, BearingRads = course.HeadingRads };
        FssLLAPoint retPnt = PlusRangeBearing(rb);

        // Now accomodate the climb rate
        double climbM = course.ClimbRateMps * timeSecs;
        retPnt.RadiusM += climbM;

        return retPnt;
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Polar Offset
    // --------------------------------------------------------------------------------------------

    public FssPolarOffset StraightLinePolarOffsetTo(FssLLAPoint destPos)
    {
        double azRads = this.BearingToRads(destPos);
        double elRads = this.ElevationToRads(destPos);
        double distM  = this.StraightLineDistanceToM(destPos);

        return new FssPolarOffset() { AzRads = azRads, ElRads = elRads, RangeM = distM };
    }

    public FssLLAPoint PlusPolarOffset(FssPolarOffset offset)
    {
        // We start by calculating the displacement along the ground (range*Cos(elevation)).
        double rangeOnGroundM = offset.RangeM * Math.Cos(offset.ElRads);

        // Calculate the change in latitude and longitude from the displacement and azimuth.
        double deltaLatRads = rangeOnGroundM * Math.Cos(offset.AzRads) / this.RadiusM;
        double deltaLonRads = rangeOnGroundM * Math.Sin(offset.AzRads) / (this.RadiusM * Math.Cos(this.LatRads));

        // The new latitude and longitude are then the old values plus the changes.
        double newLatRads = this.LatRads + deltaLatRads;
        double newLonRads = this.LonRads + deltaLonRads;

        // The change in altitude (range*Sin(elevation)) is added to the original altitude to find the new altitude.
        double newRadiusM = this.RadiusM + offset.RangeM * Math.Sin(offset.ElRads);

        return new FssLLAPoint(newLatRads, newLonRads, newRadiusM);
    }

    public FssLLAPoint PlusPolarOffset(double azRads, double elRads, double rangeM)
    {
        FssPolarOffset offset = new FssPolarOffset() { AzRads = azRads, ElRads = elRads, RangeM = rangeM };
        return this.PlusPolarOffset(offset);
    }

    public FssLLAPoint PlusPolarOffsetDegs(double azDegs, double elDegs, double rangeM)
    {
        double azRads = azDegs * FssConsts.DegsToRadsMultiplier;
        double elRads = elDegs * FssConsts.DegsToRadsMultiplier;

        FssPolarOffset offset = new FssPolarOffset() { AzRads = azRads, ElRads = elRads, RangeM = rangeM };
        return this.PlusPolarOffset(offset);
    }


}
