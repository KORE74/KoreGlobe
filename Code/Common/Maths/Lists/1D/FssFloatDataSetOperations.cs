using System;
using System.Collections.Generic;
using System.Linq;

public static class FssFloatDataSetOperations
{
    public static FssFloat1DArray DataSetToArray(FssFloatDataSet dataSet, int arraySize)
    {
        FssFloat1DArray array = new FssFloat1DArray(arraySize);

        for (int i = 0; i < arraySize; i++)
        {
            float fraction = (float)i / (float)(arraySize - 1);
            array[i] = dataSet.GetValue(fraction);
        }

        return array;
    }

    // Simple initial implementation to interpolate across the array and produce an output dataset.
    public static FssFloatDataSet ArrayToDataSet_Interpolation(FssFloat1DArray array, int numberOfPoints)
    {
        var dataSet = new FssFloatDataSet();

        for (int i = 0; i < numberOfPoints; i++)
        {
            float fraction = (float)i / (float)(numberOfPoints - 1);
            dataSet.AddDataPoint(fraction, array.InterpolateAtFraction(fraction));
        }

        return dataSet;
    }

    public static FssFloatDataSet ArrayToDataSet(FssFloat1DArray array, int numberOfPoints, float directionChangeThreshold)
    {
        var dataSet = new FssFloatDataSet();
        FssFloat1DArray firstOrderDifferences  = FssFloat1DArrayOperations.CreateDifferenceList(array);
        FssFloat1DArray secondOrderDifferences = FssFloat1DArrayOperations.CreateDifferenceList(firstOrderDifferences);
        float totalChange = secondOrderDifferences.Sum();
        float averageChangePerPoint = totalChange / numberOfPoints;
        float accumulatedChange = 0;

        HashSet<int> keyPointIndices = new HashSet<int>();

        // Accumulate difference totals and identify key points
        for (int i = 0; i < array.Length; i++)
        {
            accumulatedChange += Math.Abs(array[i] - (i > 0 ? array[i - 1] : 0));

            if (accumulatedChange >= averageChangePerPoint || secondOrderDifferences[i] > directionChangeThreshold)
            {
                dataSet.AddDataPoint((float)i / (array.Length - 1), array[i]);
                keyPointIndices.Add(i);
                accumulatedChange = 0;
            }
        }

        // Add key points of significant change in direction
        for (int i = 0; i < secondOrderDifferences.Length; i++)
        {
            if (secondOrderDifferences[i] > directionChangeThreshold && !keyPointIndices.Contains(i))
            {
                dataSet.AddDataPoint((float)i / (array.Length - 1), array[i]);
            }
        }

        return dataSet;
    }



}