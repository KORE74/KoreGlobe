// using System;

// public class FssFloatRange
// {
//     // Attributes, read-only after creation, and calc a range.
//     public float Min { get; private set; }
//     public float Max { get; private set; }
//     public float Range { get { return Max - Min; } }

//     public FssFloatRange(float min, float max)
//     {
//         if (min > max)
//         {
//             throw new ArgumentException("Min value cannot be greater than Max value.");
//         }

//         Min = min;
//         Max = max;
//     }

//     // Predefined constant ranges
//     public static readonly FssFloatRange ZeroToOne            = new FssFloatRange(0f, 1f);
//     public static readonly FssFloatRange ZeroTo360Degrees     = new FssFloatRange(0f, 360f);
//     public static readonly FssFloatRange Minus180To180Degrees = new FssFloatRange(-180f, 180f);
//     public static readonly FssFloatRange ZeroToTwoPiRadians   = new FssFloatRange(0f, (float)(Math.PI * 2));
//     public static readonly FssFloatRange MinusPiToPiRadians   = new FssFloatRange((float)(-Math.PI), (float)Math.PI);

//     public bool IsInRange(float val)
//     {
//         if (val > Max) return false;
//         if (val < Min) return false;
//         return true;
//     }

//     public float FractionInRange(float val)
//     {
//         if (val < Min) return 0f;
//         if (val > Max) return 1f;
//         return (val - Min) / Range;
//     }

//     public override string ToString()
//     {
//         return $"Range: [{Min:F3}, {Max:F3}]";
//     }
// }
