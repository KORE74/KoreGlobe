using System;

public static partial class FssTestPosition
{
    public static void RunTests(FssTestLog testLog)
    {
        // 2D
        TestFssXYPoint(testLog);
        TestFssXYLine(testLog);
        

        // 3D
        TestFssXYZ(testLog);
        TestFssXYZLine(testLog);
        //TestFssXYZPlane(testLog);
    }

    private static void TestFssXYZ(FssTestLog testLog)
    {
        // Example: Test creation of FssXYZPoint points and basic operations
        var pointA = new FssXYZPoint(1, 2, 3);
        var pointB = new FssXYZPoint(4, 5, 6);

        testLog.Add("FssXYZPoint Creation", pointA.X == 1 && pointA.Y == 2 && pointA.Z == 3);
        testLog.Add("FssXYZPoint Distance", Math.Abs(pointA.DistanceTo(pointB) - 5.196) < 0.001); // Example threshold for floating point comparison

        // Add more tests for FssXYZPoint
    }

    private static void TestFssXYZLine(FssTestLog testLog)
    {
        // Example: Test FssXYZLine creation and properties
        var line = new FssXYZLine(new FssXYZPoint(0, 0, 0), new FssXYZPoint(1, 1, 1));

        testLog.Add("FssXYZLine Length", Math.Abs(line.Length - Math.Sqrt(3)) < 0.001);
        //testLog.Add("FssXYZLine MidPoint", line.MidPoint().Equals(new FssXYZPoint(0.5, 0.5, 0.5)));

        // Add more tests for FssXYZLine
    }

    private static void TestFssXYZPlane(FssTestLog testLog)
    {
        // Example: Test FssXYZPlane creation and normal calculation
        FssXYZPlane plane = new FssXYZPlane(new FssXYZPoint(0, 0, 0), new FssXYZPoint(1, 0, 0), new FssXYZPoint(0, 1, 0));

        bool testColinear = FssXYZPlaneOperations.PointsCollinear(new FssXYZPoint(0, 0, 0), new FssXYZPoint(1, 0, 0), new FssXYZPoint(0, 1, 0));
        testLog.Add("FssXYZPlane Collinear", (testColinear == false));

        // testLog.Add("FssXYZPlane Normal", plane.Normal().Equals(new FssXYZPoint(0, 0, 1)));

        // // Example: Test distance from a point to the plane
        // var point = new FssXYZPoint(0, 0, 1);
        // testLog.Add("FssXYZPlane DistanceFromPlane", Math.Abs(FssXYZPlane.DistanceFromPlane(point, plane) - 1) < 0.001);

        // Add more tests for FssXYZPlane
    }

    // Additional helper methods or test cases as needed
}
