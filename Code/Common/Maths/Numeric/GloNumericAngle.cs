
using System;
using System.Numerics;

public struct GloNumericAngle<T> where T : INumber<T>
{
    private static readonly T Pi        = T.CreateChecked(Math.PI);
    private static readonly T OneEighty = T.CreateChecked(180.0);

    // --------------------------------------------------------------------------------------------

    public static T DegsToRads(T deg) => deg * Pi / OneEighty;
    public static T RadsToDegs(T rad) => rad * OneEighty / Pi;

    // --------------------------------------------------------------------------------------------

    // Usage: double ang = GloNumericAngle<double>.NormalizeRads(ang);
    public static T NormalizeRads(T angleRads)
    {
        return GloNumericUtils.Modulo(angleRads, T.CreateChecked(2 * Math.PI));
    }

    public static T NormalizeDegs(T angleDegs)
    {
         return GloNumericUtils.Modulo(angleDegs, T.CreateChecked(360));
    }

    // --------------------------------------------------------------------------------------------


    public static T DiffWrappedDegs(T deg1, T deg2)
    {
        T diff = GloNumericUtils.Modulo(deg1 - deg2, T.CreateChecked(360));
        return diff > T.CreateChecked(180)
            ? T.CreateChecked(360) - diff
            : diff;
    }

    public static T DiffWrappedRads(T rad1, T rad2)
    {
        T diff = GloNumericUtils.Modulo(rad1 - rad2, T.CreateChecked(2 * Math.PI));
        return diff > Pi
            ? T.CreateChecked(2 * Math.PI) - diff
            : diff;
    }

    // --------------------------------------------------------------------------------------------

}