// using System;

// public struct GloFloatRange
// {
//     // Attributes, read-only after creation, and calc a range.
//     public float Min   { get; private set; }
//     public float Max   { get; private set; }
//     public float Range { get { return Max - Min; } }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Constructors and standard ranges
//     // --------------------------------------------------------------------------------------------

//     public GloFloatRange(float min, float max)
//     {
//         if (min > max) { (min, max) = (max, min); }

//         Min = min;
//         Max = max;
//     }

//     // Predefined constant ranges
//     public static readonly GloFloatRange ZeroToOne            = new GloFloatRange(0f, 1f);
//     public static readonly GloFloatRange ZeroTo360Degrees     = new GloFloatRange(0f, 360f);
//     public static readonly GloFloatRange Minus180To180Degrees = new GloFloatRange(-180f, 180f);
//     public static readonly GloFloatRange ZeroToTwoPiRadians   = new GloFloatRange(0f, (float)(Math.PI * 2));
//     public static readonly GloFloatRange MinusPiToPiRadians   = new GloFloatRange((float)(-Math.PI), (float)Math.PI);

//     // --------------------------------------------------------------------------------------------

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

//     // --------------------------------------------------------------------------------------------

//     // Returns a subrange from teh current object's range, given the "GridElem"th element within GridCount elements.
//     public GloFloatRange SubRangeGrid(int GridCount, int GridElem)
//     {
//         if (GridCount <= 0)
//         {
//             throw new ArgumentException("GridCount must be greater than zero.");
//         }
//         if (GridElem < 0 || GridElem >= GridCount)
//         {
//             throw new ArgumentOutOfRangeException("GridElem must be within the range of 0 to GridCount - 1.");
//         }

//         float subRangeSize = Range / GridCount;
//         float newMin       = Min + (subRangeSize * GridElem);
//         float newMax       = newMin + subRangeSize;

//         return new GloFloatRange(newMin, newMax);
//     }

//     // --------------------------------------------------------------------------------------------

//     public override string ToString()
//     {
//         return $"Range: [{Min:F3}, {Max:F3}]";
//     }
// }
