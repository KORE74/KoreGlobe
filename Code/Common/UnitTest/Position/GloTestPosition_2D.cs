using System;

public static partial class GloTestPosition
{
    private static void TestGloXYPoint(GloTestLog testLog)
    {
        // Example: Test creation of GloXYPoint points and basic operations
        var pointA = new GloXYPoint(1, 2);
        var pointB = new GloXYPoint(4, 5);

        testLog.Add("GloXYZPoint Creation", pointA.X == 1 && pointA.Y == 2);
        testLog.Add("GloXYZPoint Distance", Math.Abs(pointA.DistanceTo(pointB) - 5.196) < 0.001); // Example threshold for floating point comparison

        // Add more tests for GloXYZPoint
    }

    private static void TestGloXYLine(GloTestLog testLog)
    {

    }



}
