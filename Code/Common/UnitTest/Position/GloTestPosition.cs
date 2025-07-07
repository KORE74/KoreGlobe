using System;

public static partial class GloTestPosition
{
    public static void RunTests(GloTestLog testLog)
    {
        // 2D
        TestGloXYPoint(testLog);
        TestGloXYLine(testLog);


        // 3D
        TestGloXYZ(testLog);
        TestGloXYZLine(testLog);
        //TestGloXYZPlane(testLog);
    }

    private static void TestGloXYZ(GloTestLog testLog)
    {
        // Example: Test creation of GloXYZPoint points and basic operations
        var pointA = new GloXYZPoint(1, 2, 3);
        var pointB = new GloXYZPoint(4, 5, 6);

        testLog.Add("GloXYZPoint Creation", pointA.X == 1 && pointA.Y == 2 && pointA.Z == 3);
        testLog.Add("GloXYZPoint Distance", Math.Abs(pointA.DistanceTo(pointB) - 5.196) < 0.001); // Example threshold for floating point comparison

        // Add more tests for GloXYZPoint
    }

    private static void TestGloXYZLine(GloTestLog testLog)
    {
        // Example: Test GloXYZLine creation and properties
        var line = new GloXYZLine(new GloXYZPoint(0, 0, 0), new GloXYZPoint(1, 1, 1));

        testLog.Add("GloXYZLine Length", Math.Abs(line.Length - Math.Sqrt(3)) < 0.001);
        //testLog.Add("GloXYZLine MidPoint", line.MidPoint().Equals(new GloXYZPoint(0.5, 0.5, 0.5)));

        // Add more tests for GloXYZLine
    }

    // private static void TestGloXYZPlane(GloTestLog testLog)
    // {
    //     // Example: Test GloXYZPlane creation and normal calculation
    //     GloXYZPlane plane = new GloXYZPlane(new GloXYZPoint(0, 0, 0), new GloXYZPoint(1, 0, 0), new GloXYZPoint(0, 1, 0));

    //     bool testColinear = GloXYZPlaneOperations.PointsCollinear(new GloXYZPoint(0, 0, 0), new GloXYZPoint(1, 0, 0), new GloXYZPoint(0, 1, 0));
    //     testLog.Add("GloXYZPlane Collinear", (testColinear == false));

    //     // testLog.Add("GloXYZPlane Normal", plane.Normal().Equals(new GloXYZPoint(0, 0, 1)));

    //     // // Example: Test distance from a point to the plane
    //     // var point = new GloXYZPoint(0, 0, 1);
    //     // testLog.Add("GloXYZPlane DistanceFromPlane", Math.Abs(GloXYZPlane.DistanceFromPlane(point, plane) - 1) < 0.001);

    //     // Add more tests for GloXYZPlane
    // }

    // Additional helper methods or test cases as needed
}
