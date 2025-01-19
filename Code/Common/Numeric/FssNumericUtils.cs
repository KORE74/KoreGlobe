
using System;
using System.Numerics;

public class FssNumericUtils<T> where T : INumber<T>
{

    // Maximum of two numbers
    public static T Max(T a, T b) => a.CompareTo(b) > 0 ? a : b;
    public static T Min(T a, T b) => a.CompareTo(b) < 0 ? a : b;

    // Maximum of three numbers

    // Usage: FssNumericUtils<float>.Max(1, 2, 3);

    public static T Max(T a, T b, T c) => Max(Max(a, b), c);
    public static T Min(T a, T b, T c) => Min(Min(a, b), c);

    public static T Clamp(T value, T min, T max)
    {
        if (value.CompareTo(min) < 0) return min;
        if (value.CompareTo(max) > 0) return max;
        return value;
    }
}

/*

    FssNumericUtils<double>.Clamp(1.0, 0.0, 2.0)


*/