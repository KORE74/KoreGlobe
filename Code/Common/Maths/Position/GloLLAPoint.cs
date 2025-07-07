using System;
using System.Collections.Generic;

// The struct GloLLAPoint stores a Lat Long Altitude (LLA) position and calculates various new positions, angles and offsets related to it.
// Data is stored in SI units and additional accessor properties allow setting and getting in different units.
// An associates GloXYZPoint class defines a position in an ENU, XYZ format. An associates GloConsts class hold constants regarding conversion and earth radius.

// Design Decisions:
// - The code uses a struct rather than an immutable class, as the constructor options with flexible units
//   are simply too useful. We rely on the struct's pass by value to avoid issues with mutability.

public struct GloLLAPoint
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
        get { return LatRads * GloConsts.RadsToDegsMultiplier; }
        set { LatRads = value * GloConsts.DegsToRadsMultiplier; }
    }
    public double LonDegs
    {
        get { return LonRads * GloConsts.RadsToDegsMultiplier; }
        set { LonRads = value * GloConsts.DegsToRadsMultiplier; }
    }
    public double AltMslKm // Alt above MSL
    {
        get { return (RadiusM - GloWorldConsts.EarthRadiusM) * GloWorldConsts.MetresToKmMultiplier; }
        set { RadiusM = (value + GloWorldConsts.EarthRadiusKm) * GloWorldConsts.KmToMetresMultiplier; }
    }
    public double AltMslM // Alt above M
    {
        get { return (RadiusM - GloWorldConsts.EarthRadiusM); }
        set { RadiusM = value + GloWorldConsts.EarthRadiusM; }
    }
    public double RadiusKm // Alt above EarthRadius
    {
        get { return (RadiusM * GloWorldConsts.MetresToKmMultiplier); }
        set { RadiusM = (value * GloWorldConsts.KmToMetresMultiplier); }
    }

    public override string ToString()
    {
        return string.Format($"(LatDegs:{LatDegs:F2}, LonDegs:{LonDegs:F2}, AltMslM:{AltMslM:F2})");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors - different options and units
    // --------------------------------------------------------------------------------------------

    // Note that fields can be set:
    //   GloLLAPoint pos = new GloLLAPoint() { latDegs = X, LonDegs = Y, AltMslM = Z };

    public GloLLAPoint(double laRads, double loRads, double altM)
    {
        this.LatRads = laRads;
        this.LonRads = loRads;
        this.RadiusM = altM;
    }

    public GloLLAPoint(double laRads, double loRads)
    {
        this.LatRads = laRads;
        this.LonRads = loRads;
        this.RadiusM = GloWorldConsts.EarthRadiusM;
    }

    public GloLLAPoint(GloLLPoint llPoint)
    {
        this.LatRads = llPoint.LatRads;
        this.LonRads = llPoint.LonRads;
        this.RadiusM = GloWorldConsts.EarthRadiusM;
    }

    public static GloLLAPoint Zero
    {
        get { return new GloLLAPoint { LatRads = 0.0, LonRads = 0.0, RadiusM = 0.0 }; }
    }

    // Function to return an LLA position on the ground, either using the default MSL value, or an optional elevation above MSL in metres.

    public GloLLAPoint Grounded(double? ElevAboveMslM = null)
    {
        // Default to MSL radius
        double radius = GloWorldConsts.EarthRadiusM;

        // If the ElevAboveMslM value is provided, use it added to earth radius
        if (ElevAboveMslM.HasValue)
            radius = GloWorldConsts.EarthRadiusM + ElevAboveMslM.Value;

        // Return new "grounded" position
        return new GloLLAPoint(LatRads, LonRads, radius);
    }

    public GloLLAPoint WithMinAltMslM(double minAltMslM)
    {
        double retElev = Math.Max(GloWorldConsts.EarthRadiusM + minAltMslM, AltMslM);
        return new GloLLAPoint(LatRads, LonRads, retElev);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Conversion
    // --------------------------------------------------------------------------------------------

    public GloXYZPoint ToXYZ()
    {
        GloXYZPoint retXYZ = new GloXYZPoint(
            RadiusM * Math.Cos(LatRads) * Math.Cos(LonRads),
            RadiusM * Math.Sin(LatRads),
            RadiusM * Math.Cos(LatRads) * Math.Sin(LonRads));
        return retXYZ;
    }

    // Usage: GloLLAPoint pos = GloLLAPoint.FromXYZ(xyz);
    public static GloLLAPoint FromXYZ(GloXYZPoint inputXYZ)
    {
        double radius = inputXYZ.Magnitude;

        // Protect against div0 radius
        if (radius < GloWorldConsts.MinCalculationRadiusM)
            return GloLLAPoint.Zero;

        double latRads = Math.Asin(inputXYZ.Y / radius);
        double lonRads = Math.Atan2(inputXYZ.Z, inputXYZ.X);
        return new GloLLAPoint(latRads, lonRads, radius);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Distance
    // --------------------------------------------------------------------------------------------

    // Nullable default parameter allows it to assume the altitude for the calculation
    // is the average of the two points, unless a value (such a zero "do calc at ground
    // level") is supplied.
    // The curved distance can be at MSL if the Alt parameter is zero, will otherwise
    // use an average to the two input altitudes.

    public double CurvedDistanceToM(GloLLAPoint destPos, double? AtAltMslM = null)
    {
        // If a specific altitude is provided, use it for the radius calculation.
        // Otherwise, use the average of the radii from the Earth's center to each point.
        double radius = GloWorldConsts.EarthRadiusM;
        if (AtAltMslM.HasValue)
            radius = AtAltMslM.Value;
        else
            radius = (this.RadiusM + destPos.RadiusM) / 2;

        // Calculate the difference in latitude and longitude between the two points.
        // The AngleDiffRads function ensures the angle difference wraps correctly
        // around the Glo, handling the transition across the 180-degree meridian.
        double dLat = GloValueUtils.AngleDiffRads(destPos.LatRads, this.LatRads);
        double dLon = GloValueUtils.AngleDiffRads(destPos.LonRads, this.LonRads);

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

    public double StraightLineDistanceToM(GloLLAPoint destPos)
    {
        GloXYZPoint startXYZ = this.ToXYZ();
        GloXYZPoint destXYZ = destPos.ToXYZ();
        return startXYZ.DistanceTo(destXYZ);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Elevation
    // --------------------------------------------------------------------------------------------

    // The elevation to a distant point. Useful in aiming a camera.

    public double ElevationToRads(GloLLAPoint destPos)
    {
        // Calculate horizontal distance
        //double horizontalDistance = this.CurvedDistanceToM(destPos);
        double horizontalDistance = this.StraightLineDistanceToM(destPos);

        // Calculate altitude difference (raw)
        double altitudeDifference = destPos.AltMslM - this.AltMslM;

        // Adjust altitude difference to account for Earth's curvature
        double curvatureDrop = (horizontalDistance * horizontalDistance) / (2 * GloWorldConsts.EarthRadiusM);
        double adjustedAltitudeDifference = altitudeDifference - curvatureDrop;

        // Calculate the elevation angle in radians
        double elevationRads = Math.Atan2(adjustedAltitudeDifference, horizontalDistance);

        // Return the elevation angle
        return elevationRads * -1;
    }


    // /// <summary>
    // /// Updates the altitude of this GloLLAPoint point to result in a specific elevation angle when viewed from another point.
    // /// The elevation angle is specified in radians, where 0 radians represents no elevation change (horizontal line of sight),
    // /// negative values represent a downward angle, and positive values represent an upward angle relative to the local plane.
    // /// </summary>
    // /// <param name="relativetoPos">The GloLLAPoint point from which the elevation is measured.</param>
    // /// <param name="elevRads">The desired elevation angle in radians. Negative for downward, positive for upward.</param>
    // /// <returns>The updated GloLLAPoint object with adjusted altitude.</returns>
    // public GloLLAPoint WithElevationRads(GloLLAPoint relativetoPos, double elevRads)
    // {
    //     // Step 1: Calculate the horizontal distance
    //     double horizontalDistance = this.CurvedDistanceToM(relativetoPos, null);

    //     // Step 2: Calculate the altitude difference
    //     double altitudeDifference = Math.Tan(elevRads) * horizontalDistance;

    //     // Step 3: Update the altitude of this point
    //     // Assuming AltMslM is the altitude above mean sea level
    //     this.AltMslM = relativetoPos.AltMslM + altitudeDifference;

    //     return this; // Return the modified object
    // }

    // --------------------------------------------------------------------------------------------
    // MARK: Azimuth
    // --------------------------------------------------------------------------------------------

    public double BearingToRads(GloLLAPoint destPos)
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
    // MARK: Range Bearing
    // --------------------------------------------------------------------------------------------

    // Use haversine formula to calculate distance

    public GloRangeBearing RangeBearingTo(GloLLAPoint destPos)
    {
        double lat1 = this.LatRads;
        double lon1 = this.LonRads;
        double lat2 = destPos.LatRads;
        double lon2 = destPos.LonRads;

        double dLat = lat2 - lat1;
        double dLon = lon2 - lon1;

        double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dLon / 2), 2);

        double distanceM = 2 * GloWorldConsts.EarthRadiusM * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)); // in meters

        // Calculate bearing
        double y = Math.Sin(dLon) * Math.Cos(lat2);
        double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
        double bearingRads = Math.Atan2(y, x);

        return new GloRangeBearing { RangeM = distanceM, BearingRads = bearingRads };
    }

    public GloLLAPoint PlusRangeBearing(GloRangeBearing inputRB)
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

        return new GloLLAPoint(NewLatRads, NewLonRads, this.RadiusM);
    }

    // --------------------------------------------------------------------------------------------

    public GloLLAPoint PlusDeltaForTime(GloCourse course, double timeSecs)
    {
        // Turn the course into a range bearing distance (for the time duration) that we can apply)
        double distM = course.SpeedMps * timeSecs;
        GloRangeBearing rb = new GloRangeBearing() { RangeM = distM, BearingRads = course.HeadingRads };
        GloLLAPoint retPnt = PlusRangeBearing(rb);

        // Now accomodate the climb rate
        double climbM = course.ClimbRateMps * timeSecs;
        retPnt.RadiusM += climbM;

        return retPnt;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Polar Offset
    // --------------------------------------------------------------------------------------------

    public GloAzElRange StraightLinePolarOffsetTo(GloLLAPoint destPos)
    {
        double azRads = this.BearingToRads(destPos);
        double elRads = this.ElevationToRads(destPos);
        double distM  = this.StraightLineDistanceToM(destPos);
        //double distM  = this.CurvedDistanceToM(destPos);

        return new GloAzElRange() { AzRads = azRads, ElRads = elRads, RangeM = distM };
    }

    public GloLLAPoint PlusPolarOffset(GloAzElRange offset)
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
        double newRadiusM = this.RadiusM + offset.RangeM * ( Math.Sin(offset.ElRads) * -1);

        return new GloLLAPoint(newLatRads, newLonRads, newRadiusM);
    }

    public GloLLAPoint PlusPolarOffset(double azRads, double elRads, double rangeM)
    {
        GloAzElRange offset = new GloAzElRange() { AzRads = azRads, ElRads = elRads, RangeM = rangeM };
        return this.PlusPolarOffset(offset);
    }

    public GloLLAPoint PlusPolarOffsetDegs(double azDegs, double elDegs, double rangeM)
    {
        double azRads = azDegs * GloConsts.DegsToRadsMultiplier;
        double elRads = elDegs * GloConsts.DegsToRadsMultiplier;

        GloAzElRange offset = new GloAzElRange() { AzRads = azRads, ElRads = elRads, RangeM = rangeM };
        return this.PlusPolarOffset(offset);
    }


}
