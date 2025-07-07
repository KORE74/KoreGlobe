// using System;

// #nullable enable

// public enum RangeBehavior { Wrap, Limit }

// public class GloDoubleRange
// {
//     public double        Min      { get; private set; }
//     public double        Max      { get; private set; }
//     public double        Range    { get { return Max - Min; } }
//     public RangeBehavior Behavior { get; private set; }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Constructors
//     // --------------------------------------------------------------------------------------------

//     public GloDoubleRange(double min, double max, RangeBehavior behavior = RangeBehavior.Limit)
//     {
//         if (min > max) (min, max) = (max, min); // Flip values to ensure min <= max
//         Min      = min;
//         Max      = max;
//         Behavior = behavior;
//     }

//     // Predefined constant ranges
//     public static readonly GloDoubleRange ZeroToOne            = new GloDoubleRange(0.0, 1.0);
//     public static readonly GloDoubleRange ZeroTo360Degrees     = new GloDoubleRange(0.0, 360.0);
//     public static readonly GloDoubleRange Minus180To180Degrees = new GloDoubleRange(-180.0, 180.0);
//     public static readonly GloDoubleRange ZeroToTwoPiRadians   = new GloDoubleRange(0.0, Math.PI * 2);
//     public static readonly GloDoubleRange MinusPiToPiRadians   = new GloDoubleRange(-Math.PI, Math.PI);

//     // --------------------------------------------------------------------------------------------
//     // MARK: Range Checking
//     // --------------------------------------------------------------------------------------------

//     public bool IsInRange(double val)
//     {
//         return val >= Min && val <= Max;
//     }
//     public bool Contains(double val) => IsInRange(val);

//     public double FractionInRange(double val)
//     {
//         if (val < Min) return 0.0;
//         if (val > Max) return 1.0;
//         return (val - Min) / Range;
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Range Application
//     // --------------------------------------------------------------------------------------------

//     public double Apply(double value)
//     {
//         if (IsInRange(value))
//             return value;

//         switch (Behavior)
//         {
//             case RangeBehavior.Wrap:
//                 double offset = (value - Min) % Range;
//                 return (offset >= 0) ? Min + offset : Max + offset;
//             case RangeBehavior.Limit:
//                 return (value < Min) ? Min : Max;
//             default:
//                 throw new InvalidOperationException("Unsupported range behavior.");
//         }
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: General functions
//     // --------------------------------------------------------------------------------------------

//     public override bool Equals(object? obj)
//     {
//         if (ReferenceEquals(this, obj))
//         {
//             return true;
//         }

//         if (obj is GloDoubleRange other)
//         {
//             return this.Min.Equals(other.Min) &&
//                 this.Max.Equals(other.Max) &&
//                 this.Behavior == other.Behavior;
//         }

//         return false;
//     }

//     public override string ToString()
//     {
//         return $"[Min:{Min:F3}, Max:{Max:F3}, Behavior:{Behavior}]";
//     }

//     public override int GetHashCode()
//     {
//         throw new NotImplementedException();
//     }
// }
