using System;

public static partial class FssTestPosition
{
    private static void TestFssXYPoint(FssTestLog testLog)
    {
        // Example: Test creation of FssXYPoint points and basic operations
        var pointA = new FssXYPoint(1, 2);
        var pointB = new FssXYPoint(4, 5);

        testLog.Add("FssXYZPoint Creation", pointA.X == 1 && pointA.Y == 2);
        testLog.Add("FssXYZPoint Distance", Math.Abs(pointA.DistanceTo(pointB) - 5.196) < 0.001); // Example threshold for floating point comparison

        // Add more tests for FssXYZPoint
    }

    private static void TestFssXYLine(FssTestLog testLog)
    {
        
    }



}
