// GloRouteLegPointToPoint: A class that has a start and end point for straight line route leg, along with speed.

using System;

public class GloRouteLegLine : IGloRouteLeg
{
    public GloLLAPoint StartPoint { get; set; }
    public GloLLAPoint EndPoint   { get; set; }

    // Course Operations
    public GloCourse StartCourse { get; private set; }
    public GloCourse EndCourse   { get; private set; }

    // Attitude Operations
    public GloAttitude StartAttitude { get; private set; }
    public GloAttitude EndAttitude   { get; private set; }

    // Attitude Delta Operations
    public GloAttitudeDelta StartAttitudeDelta { get; private set; }
    public GloAttitudeDelta EndAttitudeDelta   { get; private set; }

    // --------------------------------------------------------------------------------------------
    // Constructors
    // --------------------------------------------------------------------------------------------

    public GloRouteLegLine(GloLLAPoint startPoint, GloLLAPoint endPoint, double speedMPS)
    {
        StartPoint = startPoint;
        EndPoint   = endPoint;
    }

    // --------------------------------------------------------------------------------------------
    // Public Methods
    // --------------------------------------------------------------------------------------------

    private void SetupRoute(double speedMPS)
    {
        // Calculate the course and distance between the two points

        GloRangeBearing rb = StartPoint.RangeBearingTo(EndPoint);

        GloCourse course = new GloCourse() {
            SpeedMps     = speedMPS,
            HeadingDegs  = rb.BearingDegs,
            ClimbRateMps = 0
        };

        // Set the course and distance
        StartCourse = course;
        EndCourse   = course;

        // Set the attitude and attitude delta
        StartAttitude      = GloAttitude.Zero;
        EndAttitude        = GloAttitude.Zero;
        StartAttitudeDelta = GloAttitudeDelta.Zero;
        EndAttitudeDelta   = GloAttitudeDelta.Zero;
    }

    // --------------------------------------------------------------------------------------------
    // Public Methods
    // --------------------------------------------------------------------------------------------

    public double GetDistanceM()
    {
        return StartPoint.CurvedDistanceToM(EndPoint);
    }

    public double GetDurationS()
    {
        return GetDistanceM() / StartCourse.SpeedMps;
    }

    public GloLLAPoint CurrentPosition(double timeS)
    {
        double distanceM = StartCourse.SpeedMps * timeS;
        double fraction = distanceM / GetDistanceM();
        return GloLLAPointOperations.RhumbLineInterpolation(StartPoint, EndPoint, fraction);
    }
}



