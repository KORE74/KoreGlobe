using System;
using System.Collections.Generic;
using System.Linq;

public static partial class GloFloat1DArrayOperations
{
    // --------------------------------------------------------------------------------------------
    // MARK: Bezer functions
    // --------------------------------------------------------------------------------------------

    // CalculatePoint: Determines a point on a Bezier path given a parameter t and control points.
    // This function can handle 3, 4, or 5 control points to calculate the position on the curve.
    // For example, it can be used to find an XY coordinate on a Bezier curve.

    public static float CalculatePoint(float t, GloFloat1DArray controlPoints)
    {
        switch (controlPoints.Length)
        {
            case 3:
                return Calculate3PointBezier(t, controlPoints);
            case 4:
                return Calculate4PointBezier(t, controlPoints);
            case 5:
                return Calculate5PointBezier(t, controlPoints);
            default:
                throw new InvalidOperationException("Unsupported number of control points.");
        }
    }

    // CalculateFirstDerivative: Determines the rate of change of a value on a Bezier path given a parameter t and control points.
    // This function calculates the first derivative, which represents the velocity or speed of a point moving along the curve.
    // For example, it can be used to find the velocity of an XY marker on the Bezier path per unit movement.

    public static GloFloat1DArray CalculateFirstDerivative(float t, GloFloat1DArray controlPoints)
    {
        switch (controlPoints.Length)
        {
            case 3:
                return Calculate3PointBezierFirstDerivative(t, controlPoints);
            case 4:
                return Calculate4PointBezierFirstDerivative(t, controlPoints);
            case 5:
                return Calculate5PointBezierFirstDerivative(t, controlPoints);
            default:
                throw new InvalidOperationException("Unsupported number of control points.");
        }
    }

    // CalculateSecondDerivative: Determines the rate of acceleration of a value on a Bezier path given a parameter t and control points.
    // This function calculates the second derivative, which represents the acceleration or the rate of change of velocity of a point on the curve.
    // For example, it can be used to find the rate of change of velocity of an XY marker on the Bezier path per unit movement.

    public static GloFloat1DArray CalculateSecondDerivative(float t, GloFloat1DArray controlPoints)
    {
        switch (controlPoints.Length)
        {
            case 3:
                return Calculate3PointBezierSecondDerivative(controlPoints);
            case 4:
                return Calculate4PointBezierSecondDerivative(t, controlPoints);
            case 5:
                return Calculate5PointBezierSecondDerivative(t, controlPoints);
            default:
                throw new InvalidOperationException("Unsupported number of control points.");
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: 3 Point Bezer
    // --------------------------------------------------------------------------------------------

    private static float Calculate3PointBezier(float t, GloFloat1DArray controlPoints)
    {
        float u  = 1 - t;
        float tt = t * t;
        float uu = u * u;

        return uu * controlPoints[0] + 2 * u * t * controlPoints[1] + tt * controlPoints[2];
    }

    private static GloFloat1DArray Calculate3PointBezierFirstDerivative(float t, GloFloat1DArray controlPoints)
    {
        float u = 1 - t;
        float dx = 2 * (1 - t) * (controlPoints[1] - controlPoints[0]) + 2 * t * (controlPoints[2] - controlPoints[1]);

        return new GloFloat1DArray(new float[] { dx });
    }

    private static GloFloat1DArray Calculate3PointBezierSecondDerivative(GloFloat1DArray controlPoints)
    {
        float dx = 2 * (controlPoints[2] - 2 * controlPoints[1] + controlPoints[0]);

        return new GloFloat1DArray(new float[] { dx });
    }

    // --------------------------------------------------------------------------------------------
    // MARK: 4 Point Bezer
    // --------------------------------------------------------------------------------------------

    private static float Calculate4PointBezier(float t, GloFloat1DArray controlPoints)
    {
        float u   = 1 - t;
        float tt  = t * t;
        float uu  = u * u;
        float ttt = tt * t;
        float uuu = uu * u;

        return uuu * controlPoints[0] +
                3 * uu * t * controlPoints[1] +
                3 * u * tt * controlPoints[2] +
                ttt * controlPoints[3];
    }

    private static GloFloat1DArray Calculate4PointBezierFirstDerivative(float t, GloFloat1DArray controlPoints)
    {
        float u = 1 - t;
        float dx = 3 * (1 - t) * (1 - t) * (controlPoints[1] - controlPoints[0]) +
                6 * (1 - t) * t * (controlPoints[2] - controlPoints[1]) +
                3 * t * t * (controlPoints[3] - controlPoints[2]);

        return new GloFloat1DArray(new float[] { dx });
    }

    private static GloFloat1DArray Calculate4PointBezierSecondDerivative(float t, GloFloat1DArray controlPoints)
    {
        float u = 1 - t;
        float dx = 6 * (1 - t) * (controlPoints[2] - 2 * controlPoints[1] + controlPoints[0]) +
                   6 * t * (controlPoints[3] - 2 * controlPoints[2] + controlPoints[1]);

        return new GloFloat1DArray(new float[] { dx });
    }

    // --------------------------------------------------------------------------------------------
    // MARK: 5 Point Bezer
    // --------------------------------------------------------------------------------------------

    private static float Calculate5PointBezier(float t, GloFloat1DArray controlPoints)
    {
        float u    = 1 - t;
        float tt   = t * t;
        float uu   = u * u;
        float ttt  = tt * t;
        float uuu  = uu * u;
        float tttt = ttt * t;
        float uuuu = uuu * u;

        return uuuu * controlPoints[0] +
               4 * uuu * t * controlPoints[1] +
               6 * uu * tt * controlPoints[2] +
               4 * u * ttt * controlPoints[3] +
               tttt * controlPoints[4];
    }

    private static GloFloat1DArray Calculate5PointBezierFirstDerivative(float t, GloFloat1DArray controlPoints)
    {
        float u = 1 - t;
        float dx = 4 * (1 - t) * (1 - t) * (1 - t) * (controlPoints[1] - controlPoints[0]) +
                  12 * (1 - t) * (1 - t) * t * (controlPoints[2] - controlPoints[1]) +
                  12 * (1 - t) * t * t * (controlPoints[3] - controlPoints[2]) +
                   4 * t * t * t * (controlPoints[4] - controlPoints[3]);

        return new GloFloat1DArray(new float[] { dx });
    }

    private static GloFloat1DArray Calculate5PointBezierSecondDerivative(float t, GloFloat1DArray controlPoints)
    {
        float u = 1 - t;
        float dx = 12 * (1 - t) * (controlPoints[2] - 2 * controlPoints[1] + controlPoints[0]) +
                   24 * (1 - t) * t * (controlPoints[3] - 2 * controlPoints[2] + controlPoints[1]) +
                   12 * t * t * (controlPoints[4] - 2 * controlPoints[3] + controlPoints[2]);

        return new GloFloat1DArray(new float[] { dx });
    }
}