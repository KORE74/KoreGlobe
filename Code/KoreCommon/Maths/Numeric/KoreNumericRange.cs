using System;
using System.Numerics;

#nullable enable

namespace KoreCommon;

public enum RangeBehavior { Wrap, Limit }

public class KoreNumericRange<T> where T : INumber<T>
{
    public T             Min      { get; private set; }
    public T             Max      { get; private set; }
    public T             Range    => Max - Min;
    public RangeBehavior Behavior { get; private set; }

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    public KoreNumericRange(T min, T max, RangeBehavior behavior = RangeBehavior.Limit)
    {
        if (min > max) (min, max) = (max, min); // Flip values to ensure min <= max
        Min      = min;
        Max      = max;
        Behavior = behavior;
    }

    // Predefined constant ranges
    public static readonly KoreNumericRange<T> ZeroToOne            = new KoreNumericRange<T>(T.CreateChecked(0),    T.CreateChecked(1),   RangeBehavior.Limit);
    public static readonly KoreNumericRange<T> ZeroTo360Degrees     = new KoreNumericRange<T>(T.CreateChecked(0),    T.CreateChecked(360), RangeBehavior.Wrap);
    public static readonly KoreNumericRange<T> Minus180To180Degrees = new KoreNumericRange<T>(T.CreateChecked(-180), T.CreateChecked(180), RangeBehavior.Wrap);
    public static readonly KoreNumericRange<T> ZeroToTwoPiRadians   = new KoreNumericRange<T>(T.CreateChecked(0),    KoreConsts<T>.kTwoPi,  RangeBehavior.Wrap);
    public static readonly KoreNumericRange<T> ZeroToPiRadians      = new KoreNumericRange<T>(T.CreateChecked(0),    KoreConsts<T>.kPi,     RangeBehavior.Wrap);
    public static readonly KoreNumericRange<T> MinusPiToPiRadians   = new KoreNumericRange<T>(-KoreConsts<T>.kPi,     KoreConsts<T>.kPi,     RangeBehavior.Limit);

    // Usage Examples:
    // - double newRads = KoreNumericRange.ZeroToTwoPiRadians.Apply(oldRads);

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

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is KoreNumericRange<T> other)
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
