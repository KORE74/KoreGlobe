using System;

// GloModelPlatformKinetics: Class for the physical position and representation of a platform.
// Design Decisions:
// - Immutable: New objects are created on the fly and highly likely to be copied around the rest of
//   the system, so readonly to avoid confusion.

public class GloPlatformKinetics
{
    public GloLLAPoint StartPosition { set; get; } = GloLLAPoint.Zero;
    public GloAttitude StartAttitude { set; get; } = GloAttitude.Zero;
    public GloCourse   StartCourse   { set; get; } = GloCourse.Zero;

    // Clamp to ground is a flag with no function in the model, it is maintained, but up to the
    // rendering side to lookup the elevation of the current map tile and apply it.
    public bool        ClampToGround { set; get; } = false;

    // --------------------------------------------------------------------------------------------

    // Current position and orentation
    public GloLLAPoint CurrPosition { set; get; } = GloLLAPoint.Zero;
    public GloAttitude CurrAttitude { set; get; } = GloAttitude.Zero;

    // Changes being applied
    public GloAttitudeDelta CurrAttitudeDelta { set; get; } = GloAttitudeDelta.Zero;
    public GloCourse        CurrCourse        { set; get; } = GloCourse.Zero;
    public GloCourseDelta   CurrCourseDelta   { set; get; } = GloCourseDelta.Zero;

    // Outline of the platform (judging intersections and extremities)
    public GloXYZBox BoundingBox { set; get; } = GloXYZBox.One;

    // --------------------------------------------------------------------------------------------

    // log of positions for the last 100 seconds
    GloLimitedConcurrentQueue<GloLLAPoint> PositionLog = new GloLimitedConcurrentQueue<GloLLAPoint>(100);

    // --------------------------------------------------------------------------------------------

    public GloPlatformKinetics()
    {
    }

    // --------------------------------------------------------------------------------------------

    public void ResetPosition()
    {
        CurrPosition = StartPosition;
        CurrAttitude = StartAttitude;
        CurrCourse   = StartCourse;
    }

    public void SetStart(GloLLAPoint pos, GloAttitude att, GloCourse course)
    {
        StartPosition = pos;
        StartAttitude = att;
        StartCourse   = course;
    }


    public void UpdateForDuration(float elapsedSeconds)
    {
        // Update from the highest derivative (acceleration) to the lowest (position)



        // AttitudeDelta
        // CourseDelta
        CurrCourse   = CurrCourse.PlusDeltaForTime(CurrCourseDelta, elapsedSeconds);
        CurrAttitude = CurrAttitude.PlusDeltaForTime(CurrAttitudeDelta, elapsedSeconds);
        CurrPosition = CurrPosition.PlusDeltaForTime(CurrCourse, elapsedSeconds);

        // Log the current position, if it has moved more than a nominal amount.
        double minDistanceSeparationM = 10;

        if (PositionLog.Count == 0)
            PositionLog.Enqueue(CurrPosition);
        else
        {
            GloLLAPoint lastPos = PositionLog.GetMostRecent();
            if (CurrPosition.StraightLineDistanceToM(lastPos) > minDistanceSeparationM)
                PositionLog.Enqueue(CurrPosition);
        }

    }

    // --------------------------------------------------------------------------------------------

    // Output a roll value in Rads.

    public static double PlatformRollFromCourseDelta(GloCourse course, GloCourseDelta delta, float scaleFactor = 1f)
    {
        // This function provides a basic roll angle value to apply to an aircraft in a turn.
        // - We want to exract the angle change per second from the source delta.
        // - We need the aircraft speed to factor in the energy in the turn.
        // - We have a scale factor to amplify or reduce the effect anecdotally.
        // - We need to output a roll value of 0 to 90 degrees, with a sign matching to the clockwise/anticlockwise orientation. The likely
        //   destination GloAttitude attribute is "RollClockwiseRads".

        // Extract the rate of heading change in degrees per second, as provided by the delta.
        double headingChangeRateDegsSec = delta.HeadingChangeClockwiseDegsSec;

        // The speed of the aircraft in meters per second is used directly from the course information.
        double speedMps = course.SpeedMps;

        // The roll angle calculation incorporates the heading change rate, the speed as an energy factor, and the scale factor.
        // This combination reflects the dynamics of the turn and allows for empirical adjustment through the scale factor.
        double adjustedHeadingChange = headingChangeRateDegsSec * scaleFactor * speedMps;

        // Clamp the adjusted heading change to a maximum of 90 degrees to maintain realistic roll limits.
        // The direction (sign) of the roll matches the clockwise or anticlockwise direction of the turn.
        double rollDegrees = Math.Clamp(adjustedHeadingChange, -90, 90);

        // Since the desired output is in radians and within the range of -π/2 to π/2,
        // convert the clamped roll degrees to radians for the return value.
        double rollRads = rollDegrees * (Math.PI / 180);

        // Ensure the rollRadians is within the specified range of -π/2 to π/2 radians.
        rollRads = Math.Clamp(rollRads, -Math.PI / 2, Math.PI / 2);

        return rollRads;
    }
}
