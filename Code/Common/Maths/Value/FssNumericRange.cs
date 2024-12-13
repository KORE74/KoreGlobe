

global using FssFloatRange  = FssNumericRange<float>;
global using FssDoubleRange = FssNumericRange<double>;

using System;
using System.Numerics;

#nullable enable

public enum RangeBehavior { Wrap, Limit }

public class FssNumericRange<T> where T : INumber<T>
{
    public T Min { get; private set; }
    public T Max { get; private set; }
    public T Range => Max - Min;
    public RangeBehavior Behavior { get; private set; }

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    public FssNumericRange(T min, T max, RangeBehavior behavior = RangeBehavior.Limit)
    {
        if (min > max) (min, max) = (max, min); // Flip values to ensure min <= max
        Min      = min;
        Max      = max;
        Behavior = behavior;
    }

    // Predefined constant ranges
    public static readonly FssNumericRange<T> ZeroToOne            = new FssNumericRange<T>(T.CreateChecked(0),        T.CreateChecked(1));
    public static readonly FssNumericRange<T> ZeroTo360Degrees     = new FssNumericRange<T>(T.CreateChecked(0),        T.CreateChecked(360));
    public static readonly FssNumericRange<T> Minus180To180Degrees = new FssNumericRange<T>(T.CreateChecked(-180),     T.CreateChecked(180));
    public static readonly FssNumericRange<T> ZeroToTwoPiRadians   = new FssNumericRange<T>(T.CreateChecked(0),        T.CreateChecked(Math.PI * 2));
    public static readonly FssNumericRange<T> MinusPiToPiRadians   = new FssNumericRange<T>(T.CreateChecked(-Math.PI), T.CreateChecked(Math.PI));

    // --------------------------------------------------------------------------------------------
    // MARK: Range Checking
    // --------------------------------------------------------------------------------------------

    public bool IsInRange(T val)
    {
        return val >= Min && val <= Max;
    }

    public bool Contains(T val) => IsInRange(val);

    public double FractionInRange(T val)
    {
        if (val < Min) return 0.0;
        if (val > Max) return 1.0;
        return double.CreateChecked(val - Min) / double.CreateChecked(Range);
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
        {
            return true;
        }

        if (obj is FssNumericRange<T> other)
        {
            return this.Min.Equals(other.Min) &&
                this.Max.Equals(other.Max) &&
                this.Behavior == other.Behavior;
        }

        return false;
    }

    public override string ToString()
    {
        return $"[Min:{Min}, Max:{Max}, Behavior:{Behavior}]";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Min, Max, Behavior);
    }
}
