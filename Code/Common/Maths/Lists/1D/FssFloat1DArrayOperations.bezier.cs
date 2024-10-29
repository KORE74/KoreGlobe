using System;
using System.Collections.Generic;
using System.Linq;

public static partial class FssFloat1DArrayOperations
{
    // --------------------------------------------------------------------------------------------
    // MARK: Bezer functions
    // --------------------------------------------------------------------------------------------

    // CalculatePoint: Calculate the point on the Bezier curve at time t. Example uses include finding
    // the position of a XY point on a curve at a given time.

    public static float CalculatePoint(float t, FssFloat1DArray controlPoints)
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

    // CalculateFirstDerivative: Calculate the first derivative of the Bezier curve at time t. Example uses
    // include finding the velocity of a XY point on a curve at a given time.

    public static FssFloat1DArray CalculateFirstDerivative(float t, FssFloat1DArray controlPoints)
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

    // CalculateSecondDerivative: Calculate the second derivative of the Bezier curve at time t. Example uses
    // include finding the acceleration of a XY point on a curve at a given time.

    public static FssFloat1DArray CalculateSecondDerivative(float t, FssFloat1DArray controlPoints)
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

    private static float Calculate3PointBezier(float t, FssFloat1DArray controlPoints)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        return uu * controlPoints[0] + 2 * u * t * controlPoints[1] + tt * controlPoints[2];
    }

    private static FssFloat1DArray Calculate3PointBezierFirstDerivative(float t, FssFloat1DArray controlPoints)
    {
        float u = 1 - t;
        float dx = 2 * (1 - t) * (controlPoints[1] - controlPoints[0]) + 2 * t * (controlPoints[2] - controlPoints[1]);

        return new FssFloat1DArray(new float[] { dx });
    }

    private static FssFloat1DArray Calculate3PointBezierSecondDerivative(FssFloat1DArray controlPoints)
    {
        float dx = 2 * (controlPoints[2] - 2 * controlPoints[1] + controlPoints[0]);

        return new FssFloat1DArray(new float[] { dx });
    }

    // --------------------------------------------------------------------------------------------
    // MARK: 4 Point Bezer
    // --------------------------------------------------------------------------------------------

    private static float Calculate4PointBezier(float t, FssFloat1DArray controlPoints)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float ttt = tt * t;
        float uuu = uu * u;

        return uuu * controlPoints[0] + 3 * uu * t * controlPoints[1] + 3 * u * tt * controlPoints[2] + ttt * controlPoints[3];
    }

    private static FssFloat1DArray Calculate4PointBezierFirstDerivative(float t, FssFloat1DArray controlPoints)
    {
        float u = 1 - t;
        float dx = 3 * (1 - t) * (1 - t) * (controlPoints[1] - controlPoints[0]) +
                6 * (1 - t) * t * (controlPoints[2] - controlPoints[1]) +
                3 * t * t * (controlPoints[3] - controlPoints[2]);

        return new FssFloat1DArray(new float[] { dx });
    }

    private static FssFloat1DArray Calculate4PointBezierSecondDerivative(float t, FssFloat1DArray controlPoints)
    {
        float u = 1 - t;
        float dx = 6 * (1 - t) * (controlPoints[2] - 2 * controlPoints[1] + controlPoints[0]) +
                6 * t * (controlPoints[3] - 2 * controlPoints[2] + controlPoints[1]);

        return new FssFloat1DArray(new float[] { dx });
    }

    // --------------------------------------------------------------------------------------------
    // MARK: 5 Point Bezer
    // --------------------------------------------------------------------------------------------

    private static float Calculate5PointBezier(float t, FssFloat1DArray controlPoints)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float ttt = tt * t;
        float uuu = uu * u;
        float tttt = ttt * t;
        float uuuu = uuu * u;

        return uuuu * controlPoints[0] + 4 * uuu * t * controlPoints[1] + 6 * uu * tt * controlPoints[2] + 4 * u * ttt * controlPoints[3] + tttt * controlPoints[4];
    }

    private static FssFloat1DArray Calculate5PointBezierFirstDerivative(float t, FssFloat1DArray controlPoints)
    {
        float u = 1 - t;
        float dx = 4 * (1 - t) * (1 - t) * (1 - t) * (controlPoints[1] - controlPoints[0]) +
                12 * (1 - t) * (1 - t) * t * (controlPoints[2] - controlPoints[1]) +
                12 * (1 - t) * t * t * (controlPoints[3] - controlPoints[2]) +
                4 * t * t * t * (controlPoints[4] - controlPoints[3]);

        return new FssFloat1DArray(new float[] { dx });
    }

    private static FssFloat1DArray Calculate5PointBezierSecondDerivative(float t, FssFloat1DArray controlPoints)
    {
        float u = 1 - t;
        float dx = 12 * (1 - t) * (controlPoints[2] - 2 * controlPoints[1] + controlPoints[0]) +
                24 * (1 - t) * t * (controlPoints[3] - 2 * controlPoints[2] + controlPoints[1]) +
                12 * t * t * (controlPoints[4] - 2 * controlPoints[3] + controlPoints[2]);

        return new FssFloat1DArray(new float[] { dx });
    }

}
