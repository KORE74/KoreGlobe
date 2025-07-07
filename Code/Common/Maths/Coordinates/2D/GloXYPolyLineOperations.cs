using System;

#nullable enable

public static class GloXYPolyLineOperations
{
    // --------------------------------------------------------------------------------------------
    // 3 point Bezier curve
    // --------------------------------------------------------------------------------------------

    public static GloXYPoint Calculate3PointBezierPos(double t, GloXYPoint start, GloXYPoint control, GloXYPoint end)
    {
        double u = 1 - t;
        double tt = t * t;
        double uu = u * u;

        double x = uu * start.X + 2 * u * t * control.X + tt * end.X;
        double y = uu * start.Y + 2 * u * t * control.Y + tt * end.Y;

        return new GloXYPoint(x, y);
    }

    public static double Calculate3PointBezierTangentAngle(double t, GloXYPoint start, GloXYPoint control, GloXYPoint end)
    {
        // Calculate the derivative of the quadratic Bézier curve at t
        double dx = 2 * (1 - t) * (control.X - start.X) + 2 * t * (end.X - control.X);
        double dy = 2 * (1 - t) * (control.Y - start.Y) + 2 * t * (end.Y - control.Y);

        // Calculate the angle of the tangent in radians
        double angleRadians = Math.Atan2(dy, dx);

        // Convert radians to degrees, if desired
        double angleDegrees = angleRadians * (180.0 / Math.PI);

        return angleDegrees;
    }

    public static GloXYPoint Calculate3PointBezierFirstDerivative(GloXYPoint start, GloXYPoint control, GloXYPoint end, double t)
    {
        // Calculate the components of the derivative
        double dx = 2 * (1 - t) * (control.X - start.X) + 2 * t * (end.X - control.X);
        double dy = 2 * (1 - t) * (control.Y - start.Y) + 2 * t * (end.Y - control.Y);

        // Return the derivative as a GloXYPoint
        return new GloXYPoint(dx, dy);
    }

    // Calculate the second derivative for a 3-point Bezier curve
    public static GloXYPoint Calculate3PointBezierSecondDerivative(GloXYPoint start, GloXYPoint control, GloXYPoint end, double t)
    {
        // For a quadratic Bezier curve, the second derivative is constant
        return new GloXYPoint(2 * (end.X - 2 * control.X + start.X), 2 * (end.Y - 2 * control.Y + start.Y));
    }

    public static GloXYPolyLine? Create3PointBezier(GloXYPoint p1, GloXYPoint p2, GloXYPoint p3, int numSegments)
    {
        GloXYPolyLine? bezier = null;

        if (numSegments > 0)
        {
            double t = 0;
            double dt = 1.0 / numSegments;
            GloXYPoint[] points = new GloXYPoint[numSegments + 1];

            for (int i = 0; i <= numSegments; i++)
            {
                double fraction = (double)i / numSegments;

                points[i] = GloXYPolyLineOperations.Calculate3PointBezierPos(fraction, p1, p2, p3);
                t += dt;
            }

            bezier = new GloXYPolyLine(new System.Collections.Generic.List<GloXYPoint>(points));
        }

        return bezier;
    }

    public static double Calculate3PointBezierCurvature(double t, GloXYPoint start, GloXYPoint control, GloXYPoint end)
    {
        var firstDerivative = Calculate3PointBezierFirstDerivative(start, control, end, t);
        var secondDerivative = Calculate3PointBezierSecondDerivative(start, control, end, t);
        return CalculateCurvature(firstDerivative, secondDerivative);
    }

    // --------------------------------------------------------------------------------------------
    // 4 point Bezier curve
    // --------------------------------------------------------------------------------------------

    public static GloXYPoint Calculate4PointBezierPos(double fraction, GloXYPoint start, GloXYPoint control1, GloXYPoint control2, GloXYPoint end)
    {
        double u = 1 - fraction;
        double tt = fraction * fraction;
        double uu = u * u;
        double uuu = uu * u;
        double ttt = tt * fraction;

        double x = uuu * start.X; // first term
        x += 3 * uu * fraction * control1.X; // second term
        x += 3 * u * tt * control2.X; // third term
        x += ttt * end.X; // fourth term

        double y = uuu * start.Y; // first term
        y += 3 * uu * fraction * control1.Y; // second term
        y += 3 * u * tt * control2.Y; // third term
        y += ttt * end.Y; // fourth term

        return new GloXYPoint(x, y);
    }

    public static double Calculate4PointBezierTangentAngle(double t, GloXYPoint start, GloXYPoint control1, GloXYPoint control2, GloXYPoint end)
    {
        // Calculate the derivative of the Bézier curve at t
        double dx = 3 * (1 - t) * (1 - t) * (control1.X - start.X) +
                    6 * (1 - t) * t * (control2.X - control1.X) +
                    3 * t * t * (end.X - control2.X);

        double dy = 3 * (1 - t) * (1 - t) * (control1.Y - start.Y) +
                    6 * (1 - t) * t * (control2.Y - control1.Y) +
                    3 * t * t * (end.Y - control2.Y);

        // Calculate the angle of the tangent in radians
        double angleRadians = Math.Atan2(dy, dx);

        // Convert radians to degrees, if desired
        double angleDegrees = angleRadians * (180.0 / Math.PI);

        return angleDegrees;
    }

    public static GloXYPoint Calculate4PointBezierFirstDerivative(GloXYPoint start, GloXYPoint control1, GloXYPoint control2, GloXYPoint end, double t)
    {
        double dx = 3 * (1 - t) * (1 - t) * (control1.X - start.X) +
                    6 * (1 - t) * t * (control2.X - control1.X) +
                    3 * t * t * (end.X - control2.X);
        double dy = 3 * (1 - t) * (1 - t) * (control1.Y - start.Y) +
                    6 * (1 - t) * t * (control2.Y - control1.Y) +
                    3 * t * t * (end.Y - control2.Y);

        return new GloXYPoint(dx, dy);
    }

    // Calculate the second derivative for a 4-point Bezier curve
    public static GloXYPoint Calculate4PointBezierSecondDerivative(GloXYPoint start, GloXYPoint control1, GloXYPoint control2, GloXYPoint end, double t)
    {
        double dx = 6 * (1 - t) * (control2.X - 2 * control1.X + start.X) + 6 * t * (end.X - 2 * control2.X + control1.X);
        double dy = 6 * (1 - t) * (control2.Y - 2 * control1.Y + start.Y) + 6 * t * (end.Y - 2 * control2.Y + control1.Y);
        return new GloXYPoint(dx, dy);
    }

    public static GloXYPolyLine? Create4PointBezier(GloXYPoint p1, GloXYPoint p2, GloXYPoint p3, GloXYPoint p4, int numSegments)
    {
        GloXYPolyLine? bezier = null;

        if (numSegments > 0)
        {
            GloXYPoint[] points = new GloXYPoint[numSegments + 1];

            for (int i = 0; i <= numSegments; i++)
            {
                double fraction = (double)i / numSegments;
                points[i] = Calculate4PointBezierPos(fraction, p1, p2, p3, p4);
            }

            bezier = new GloXYPolyLine(new System.Collections.Generic.List<GloXYPoint>(points));
        }

        return bezier;
    }

    // --------------------------------------------------------------------------------------------
    // Commmon
    // --------------------------------------------------------------------------------------------

    // CalculateCurvature now takes GloXYPoint arguments for first and second derivatives
    // Calculations of first and second derivatives inside curvature methods
    // (Implement Calculate3PointBezierFirstDerivative and Calculate4PointBezierFirstDerivative as needed)

    // Common method to calculate curvature from first and second derivatives
    private static double CalculateCurvature(GloXYPoint firstDerivative, GloXYPoint secondDerivative)
    {
        double numerator = Math.Abs(firstDerivative.X * secondDerivative.Y - firstDerivative.Y * secondDerivative.X);
        double denominator = Math.Pow(Math.Pow(firstDerivative.X, 2) + Math.Pow(firstDerivative.Y, 2), 1.5);
        return denominator != 0 ? numerator / denominator : 0;
    }
}