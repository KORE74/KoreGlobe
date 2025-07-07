// using System;
// using System.Collections.Generic;
// using System.Linq;

// public static class GloFloatDataSetOperations
// {
//     public static GloFloat1DArray DataSetToArray(GloFloatDataSet dataSet, int arraySize)
//     {
//         GloFloat1DArray array = new GloFloat1DArray(arraySize);

//         for (int i = 0; i < arraySize; i++)
//         {
//             float fraction = (float)i / (float)(arraySize - 1);
//             array[i] = dataSet.GetValue(fraction);
//         }

//         return array;
//     }

//     // Simple initial implementation to interpolate across the array and produce an output dataset.
//     public static GloFloatDataSet ArrayToDataSet_Interpolation(GloFloat1DArray array, int numberOfPoints)
//     {
//         var dataSet = new GloFloatDataSet();

//         for (int i = 0; i < numberOfPoints; i++)
//         {
//             float fraction = (float)i / (float)(numberOfPoints - 1);
//             dataSet.AddDataPoint(fraction, array.InterpolateAtFraction(fraction));
//         }

//         return dataSet;
//     }

//     public static GloFloatDataSet ArrayToDataSet(GloFloat1DArray array, int numberOfPoints, float directionChangeThreshold)
//     {
//         var dataSet = new GloFloatDataSet();
//         GloFloat1DArray firstOrderDifferences  = GloFloat1DArrayOperations.CreateDifferenceList(array);
//         GloFloat1DArray secondOrderDifferences = GloFloat1DArrayOperations.CreateDifferenceList(firstOrderDifferences);
//         float totalChange = secondOrderDifferences.Sum();
//         float averageChangePerPoint = totalChange / numberOfPoints;
//         float accumulatedChange = 0;

//         HashSet<int> keyPointIndices = new HashSet<int>();

//         // Accumulate difference totals and identify key points
//         for (int i = 0; i < array.Length; i++)
//         {
//             accumulatedChange += Math.Abs(array[i] - (i > 0 ? array[i - 1] : 0));

//             if (accumulatedChange >= averageChangePerPoint || secondOrderDifferences[i] > directionChangeThreshold)
//             {
//                 dataSet.AddDataPoint((float)i / (array.Length - 1), array[i]);
//                 keyPointIndices.Add(i);
//                 accumulatedChange = 0;
//             }
//         }

//         // Add key points of significant change in direction
//         for (int i = 0; i < secondOrderDifferences.Length; i++)
//         {
//             if (secondOrderDifferences[i] > directionChangeThreshold && !keyPointIndices.Contains(i))
//             {
//                 dataSet.AddDataPoint((float)i / (array.Length - 1), array[i]);
//             }
//         }

//         return dataSet;
//     }



// }