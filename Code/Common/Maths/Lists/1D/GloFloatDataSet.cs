// using System;
// using System.Collections.Generic;
// using System.Linq;

// // Datasets have a floating point index value from 0 to 1 that we can interpolate across, and a set of values to define a
// // curve against that axis. This is a sparsely populated data set, or one focussable around key features in the range.

// public class GloFloatDataSet
// {
//     private List<(float Fraction, float Value)> dataPoints;

//     public IReadOnlyList<(float Fraction, float Value)> DataPoints => dataPoints;

//     // --------------------------------------------------------------------------------------------
//     // MARK: Initialization
//     // --------------------------------------------------------------------------------------------

//     public GloFloatDataSet()
//     {
//         dataPoints = new List<(float Fraction, float Value)>();
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Edit
//     // --------------------------------------------------------------------------------------------

//     public void AddDataPoint(float fraction, float value)
//     {
//         dataPoints.Add((fraction, value));
//         dataPoints.Sort((a, b) => a.Fraction.CompareTo(b.Fraction)); // Ensure list is sorted
//     }

//     public void RemoveDataPoint(float fraction)
//     {
//         dataPoints.RemoveAll(dp => dp.Fraction == fraction);
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Query
//     // --------------------------------------------------------------------------------------------

//     public float GetValue(float fraction)
//     {
//         if (!dataPoints.Any())
//             throw new InvalidOperationException("Data set is empty.");

//         if (fraction <= dataPoints.First().Fraction)
//             return dataPoints.First().Value;

//         if (fraction >= dataPoints.Last().Fraction)
//             return dataPoints.Last().Value;

//         int index = dataPoints.BinarySearch((fraction, 0f), new FractionComparer());
//         if (index < 0)
//             index = ~index;

//         var lowerPoint = index == 0 ? dataPoints[0] : dataPoints[index - 1];
//         var upperPoint = index == dataPoints.Count ? dataPoints[index - 1] : dataPoints[index];

//         return LinearInterpolate(lowerPoint, upperPoint, fraction);
//     }

//     private float LinearInterpolate((float Fraction, float Value) lowerPoint, (float Fraction, float Value) upperPoint, float fraction)
//     {
//         if (upperPoint.Fraction == lowerPoint.Fraction)
//             return lowerPoint.Value;

//         float fractionDiff = (fraction - lowerPoint.Fraction) / (upperPoint.Fraction - lowerPoint.Fraction);
//         return lowerPoint.Value + fractionDiff * (upperPoint.Value - lowerPoint.Value);
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Sort
//     // --------------------------------------------------------------------------------------------

//     // Custom comparer for binary search
//     private class FractionComparer : IComparer<(float Fraction, float Value)>
//     {
//         public int Compare((float Fraction, float Value) x, (float Fraction, float Value) y)
//         {
//             return x.Fraction.CompareTo(y.Fraction);
//         }
//     }

//     // Additional methods for modification, serialization, etc.
// }
