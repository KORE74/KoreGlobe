using System;
using System.Numerics;

// GloNumericUtils: A static class for common operations on templated numeric types

public static class GloNumericUtils
{
    private static readonly System.Random _random = new System.Random();

    // ---------------------------------------------------------------------------------------------

    public static T Min<T>(T a, T b) where T : INumber<T> => a < b ? a : b;
    public static T Max<T>(T a, T b) where T : INumber<T> => a > b ? a : b;

    public static T Abs<T>(T val) where T : INumber<T> => val < T.Zero ? -val : val;

    // ---------------------------------------------------------------------------------------------

    // Usage: T e = GloNumericUtils.Modulo(5, 3);
    // Usage: T e = GloNumericUtils.Modulo(5.0f, 3.0f);
    public static T Modulo<T>(T value, T modulus) where T : INumber<T>
    {
        if (modulus == T.Zero)
            throw new ArgumentException("Modulus cannot be zero", nameof(modulus));

        T result = value % modulus;
        if (result < T.Zero)
            result += modulus;

        return result;
    }

    // ---------------------------------------------------------------------------------------------

    // Usage: bool e = GloNumericUtils.IsInRange(5, 0, 10);
    // Usage: bool e = GloNumericUtils.IsInRange(5.0f, 0.0f, 10.0f);
    public static bool IsInRange<T>(T val, T rangemin, T rangemax) where T : INumber<T>
    {
        if (rangemin > rangemax) (rangemin, rangemax) = (rangemax, rangemin);
        return val >= rangemin && val <= rangemax;
    }

    // ---------------------------------------------------------------------------------------------

    // Usage: bool e = GloNumericUtils.EqualsWithinTolerance(5, 5.0001f, 0.001f);
    public static bool EqualsWithinTolerance<T>(T value, T compareVal, T tolerance) where T : INumber<T>
    {
        // Check if the two values are equal within a small tolerance
        return T.Abs(value - compareVal) < tolerance;
    }

    // Usage: bool e = GloNumericUtils.EqualsWithinTolerance(5.0f, 5.0001f);
    public static bool EqualsWithinTolerance(float a,  float b)  => EqualsWithinTolerance(a, b, GloConsts.ArbitrarySmallFloat);
    public static bool EqualsWithinTolerance(double a, double b) => EqualsWithinTolerance(a, b, GloConsts.ArbitrarySmallDouble);

    public static T ArbitrarySmallValue<T>() where T : INumber<T>
    {
        if (typeof(T) == typeof(float))  return (T)(object)GloConsts.ArbitrarySmallFloat;
        if (typeof(T) == typeof(double)) return (T)(object)GloConsts.ArbitrarySmallDouble;
        throw new NotSupportedException($"Type {typeof(T)} is not supported.");
    }

    // ---------------------------------------------------------------------------------------------

    // Usage: T e = GloNumericUtils.LimitToRange(5, 0, 10);
    public static T LimitToRange<T>(T val, T min, T max) where T : INumber<T>
    {
        if (min > max) (min, max) = (max, min);

        if (val < min) return min;
        if (val > max) return max;
        return val;
    }

    // Usage: T e = GloNumericUtils.Clamp(5, 0, 10);
    public static T Clamp<T>(T val, T min, T max) where T : INumber<T> => LimitToRange(val, min, max);

    // ---------------------------------------------------------------------------------------------

    // Usage: T e = GloNumericUtils.WrapToRange(5, 0, 3);
    public static T WrapToRange<T>(T val, T rangemin, T rangemax) where T : INumber<T>
    {
        T diff = rangemax - rangemin;
        T wrappedvalue = Modulo(val - rangemin, diff);
        return wrappedvalue + rangemin;
    }

    public static T WrapToRange<T>(T val, GloNumericRange<T> range) where T : INumber<T> =>  WrapToRange(val, range.Min, range.Max);

    // ---------------------------------------------------------------------------------------------

    // Usage: T e = GloNumericUtils.ScaleToRange(5, 0, 10, 0, 100);
    public static T ScaleToRange<T>(T val, T sourcerangemin, T sourcerangemax, T targetrangemin, T targetrangemax) where T : INumber<T>
    {
        // Flip the min max values to validate the ranges
        if (sourcerangemin > sourcerangemax) (sourcerangemin, sourcerangemax) = (sourcerangemax, sourcerangemin);
        if (targetrangemin > targetrangemax) (targetrangemin, targetrangemax) = (targetrangemax, targetrangemin);

        // Check if the input value is in the source range
        if (!IsInRange(val, sourcerangemin, sourcerangemax))
            throw new ArgumentOutOfRangeException(nameof(val), "Value is outside the source range.");

        // Perform and return the scaling
        T sourceRange = sourcerangemax - sourcerangemin;
        T targetRange = targetrangemax - targetrangemin;

        if (sourceRange < GloNumericUtils.ArbitrarySmallValue<T>())
            throw new ArgumentException("Source range too small", nameof(sourceRange));
        if (targetRange < GloNumericUtils.ArbitrarySmallValue<T>())
            throw new ArgumentException("Target range too small", nameof(targetRange));

        return ((val - sourcerangemin) / sourceRange) * targetRange + targetrangemin;
    }

    public static T ScaleToRange<T>(T val, GloNumericRange<T> sourceRange, GloNumericRange<T> targetRange) where T : INumber<T>
    {
        return ScaleToRange(val, sourceRange.Min, sourceRange.Max, targetRange.Min, targetRange.Max);
    }

    // ---------------------------------------------------------------------------------------------

    // Fraction is the fraction of the max value to use: 0..1 (values beyond this extrapolate)
    // Usage: T e = GloNumericUtils.Interpolate(0, 10, 0.5f);
    public static T Interpolate<T>(T min, T max, float fraction) where T : INumber<T>
    {
        T t = T.CreateChecked(fraction); // better than Convert.ChangeType
        return min + ((max - min) * t);
    }

    // Lerp naming for the function
    // Usage: T e = GloNumericUtils.Lerp(0, 10, 0.5f);
    public static T Lerp<T>(T start, T end, float fraction) where T : INumber<T> => Interpolate(start, end, fraction);

    // ---------------------------------------------------------------------------------------------

    // Usage: double val = GloNumericUtils.RandomInRange(0.0, 10.1);
    // Usage: float  val = GloNumericUtils.RandomInRange(0.5f, 9.5f);
    public static T RandomInRange<T>(T min, T max) where T : INumber<T>
    {
        if (min > max) (min, max) = (max, min);

        T t = T.CreateChecked(_random.NextDouble()); // generate [0, 1) in T
        return min + (max - min) * t;
    }

    // ---------------------------------------------------------------------------------------------

}
