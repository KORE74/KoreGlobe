
// Route legs are the building blocks of a route. They are the segments of the route that connect two points.
// We need operations to establish the entry and exit crietia of each leg and the position/attitude along it to
// control the platform.

// *TIME*: A route leg calculates its duration. Time for overall route position is the route class responsibility,
// not the leg. So all times are for a zero start time duration, not an overall scenario or route time.

public interface IGloRouteLeg
{
    // --------------------------------------------------------------------------------------------
    // MARK: Properties around the ends of a leg
    // --------------------------------------------------------------------------------------------

    // Position Operations
    public GloLLAPoint StartPoint { get; set; }
    public GloLLAPoint EndPoint   { get; set; }

    // Course Operations
    public GloCourse StartCourse { get; }
    public GloCourse EndCourse   { get; }

    // Attitude Operations
    public GloAttitude StartAttitude { get; }
    public GloAttitude EndAttitude   { get; }

    // Attitude Delta Operations
    public GloAttitudeDelta StartAttitudeDelta { get; }
    public GloAttitudeDelta EndAttitudeDelta   { get; }

    // Methods ordered by increasing derivatives, starting with position, velocity, acceleration, and so on.

    // --------------------------------------------------------------------------------------------
    // MARK: Position and Distance
    // --------------------------------------------------------------------------------------------

    public GloLLAPoint PositionAtLegTime(double legtimeS) => GloLLAPoint.Zero;
    public float       GetDistanceM() => 0f;

    // --------------------------------------------------------------------------------------------
    // MARK: Time
    // --------------------------------------------------------------------------------------------

    public float GetDurationS() => 0f;

    // --------------------------------------------------------------------------------------------
    // MARK: Course
    // --------------------------------------------------------------------------------------------

    public GloCourse CourseAtLegTime(double legtimeS) => GloCourse.Zero;

    // --------------------------------------------------------------------------------------------
    // MARK: Attitude
    // --------------------------------------------------------------------------------------------

    public GloAttitude AttitudeAtLegTime(double legtimeS) => GloAttitude.Zero;

    // --------------------------------------------------------------------------------------------
    // MARK: Attitude Delta
    // --------------------------------------------------------------------------------------------

    public GloAttitudeDelta AttitudeDeltaAtLegTime(double legtimeS) => GloAttitudeDelta.Zero;

}


