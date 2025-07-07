

global using GloFloatRange  = GloNumericRange<float>;
global using GloDoubleRange = GloNumericRange<double>;

using System;
using System.Numerics;

public enum RangeBehavior { Wrap, Limit }

public class GloNumericRange<T> where T : INumber<T>
{
    public T             Min      { get; private set; }
    public T             Max      { get; private set; }
    public T             Range    => Max - Min;
    public RangeBehavior Behavior { get; private set; }

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    public GloNumericRange(T min, T max, RangeBehavior behavior = RangeBehavior.Limit)
    {
        if (min > max) (min, max) = (max, min); // Flip values to ensure min <= max
        Min      = min;
        Max      = max;
        Behavior = behavior;
    }

    // Predefined constant ranges
    public static readonly GloNumericRange<T> ZeroToOne            = new GloNumericRange<T>(T.CreateChecked(0),    T.CreateChecked(1),   RangeBehavior.Limit);
    public static readonly GloNumericRange<T> ZeroTo360Degrees     = new GloNumericRange<T>(T.CreateChecked(0),    T.CreateChecked(360), RangeBehavior.Wrap);
    public static readonly GloNumericRange<T> Minus180To180Degrees = new GloNumericRange<T>(T.CreateChecked(-180), T.CreateChecked(180), RangeBehavior.Wrap);
    public static readonly GloNumericRange<T> ZeroToTwoPiRadians   = new GloNumericRange<T>(T.CreateChecked(0),    GloConsts<T>.kTwoPi,  RangeBehavior.Wrap);
    public static readonly GloNumericRange<T> MinusPiToPiRadians   = new GloNumericRange<T>(-GloConsts<T>.kPi,     GloConsts<T>.kPi,     RangeBehavior.Limit);

    // --------------------------------------------------------------------------------------------
    // MARK: Range Checking
    // --------------------------------------------------------------------------------------------

    public bool IsInRange(T val)
    {
        return val >= Min && val <= Max;
    }
    public bool Contains(T val) => IsInRange(val);

    // Limit range clamps the val fraction to 0 to 1. Otherwise, it can go into a wider positive/negative range to provide extrapolation.
    public T FractionInRange(T val, bool limitRange = true)
    {
        if (limitRange)
        {
            if (val < Min) return T.CreateChecked(0.0);
            if (val > Max) return T.CreateChecked(1.0);
        }
        return T.CreateChecked(val - Min) / T.CreateChecked(Range);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Range Application
    // --------------------------------------------------------------------------------------------

    public T Apply(T value)
    {
        if (IsInRange(value))
            return value;

        switch (Behavior)
        {
            case RangeBehavior.Wrap:
                T offset = (value - Min) % Range;
                return (offset >= T.Zero) ? Min + offset : Max + offset;
            case RangeBehavior.Limit:
                return (value < Min) ? Min : Max;
            default:
                throw new InvalidOperationException("Unsupported range behavior.");
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: General functions
    // --------------------------------------------------------------------------------------------

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is GloNumericRange<T> other)
        {
            return this.Min.Equals(other.Min) &&
                   this.Max.Equals(other.Max) &&
                   this.Behavior == other.Behavior;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Min, Max, Behavior);
    }

    public T IncrementForSize(int size)
    {
        return Range / T.CreateChecked(size - 1);
    }

    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return $"[Min:{Min}, Max:{Max}, Behavior:{Behavior}]";
    }

}
